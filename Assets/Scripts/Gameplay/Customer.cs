using UnityEngine;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int minItemsToBuy = 1;
    [SerializeField] private int maxItemsToBuy = 3;

    private enum State
    {
        Shopping,
        ApproachingLine,
        InQueue,
        CheckingOut,
        Exiting
    }

    private State currentState;
    private Cashier targetCashier;
    private Transform exitPoint;
    private Vector3 currentMoveTargetPosition;
    private bool hasTargetPosition = false;

    private List<Shelf> shoppingList = new List<Shelf>();
    private List<Product> shoppingCart = new List<Product>();
    private int shelfVisitIndex = 0;

    public void Initialize(Cashier cashier, Transform exit)
    {
        targetCashier = cashier;
        exitPoint = exit;
        CreateShoppingList();

        if (shoppingList.Count > 0)
        {
            currentState = State.Shopping;
            SetNextShoppingDestination();
        }
        else
        {
            LeaveStore(false);
        }
    }

    private void Update()
    {
        if (currentState == State.ApproachingLine)
        {
            currentMoveTargetPosition = targetCashier.GetLineTailPosition();
            transform.position = Vector2.MoveTowards(transform.position, currentMoveTargetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentMoveTargetPosition) < 0.1f)
            {
                currentState = State.InQueue;
                targetCashier.JoinQueue(this);
            }
            return;
        }

        if (!hasTargetPosition) return;

        transform.position = Vector2.MoveTowards(transform.position, currentMoveTargetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentMoveTargetPosition) < 0.1f)
        {
            OnDestinationReached();
        }
    }

    private void OnDestinationReached()
    {
        if (currentState == State.Shopping)
        {
            Shelf currentShelf = shoppingList[shelfVisitIndex];
            Product takenProduct = currentShelf.TakeProduct();

            if (takenProduct != null)
            {
                shoppingCart.Add(takenProduct);
            }

            shelfVisitIndex++;
            SetNextShoppingDestination();
        }
        else if (currentState == State.CheckingOut)
        {
            hasTargetPosition = false;
            targetCashier.OnCustomerArrivedAtCounter(this, shoppingCart);
        }
        else if (currentState == State.Exiting)
        {
            Destroy(gameObject);
        }
        else
        {
            hasTargetPosition = false;
        }
    }

    private void CreateShoppingList()
    {
        Shelf[] allShelves = FindObjectsByType<Shelf>(FindObjectsSortMode.None);
        List<Shelf> availableShelves = new List<Shelf>();

        foreach (var shelf in allShelves)
        {
            if (shelf.HasStock())
            {
                availableShelves.Add(shelf);
            }
        }

        int itemsToGet = Random.Range(minItemsToBuy, maxItemsToBuy + 1);

        for (int i = 0; i < itemsToGet; i++)
        {
            if (availableShelves.Count == 0) break;

            int randomIndex = Random.Range(0, availableShelves.Count);
            shoppingList.Add(availableShelves[randomIndex]);
            availableShelves.RemoveAt(randomIndex);
        }
    }

    private void SetNextShoppingDestination()
    {
        if (shelfVisitIndex < shoppingList.Count)
        {
            currentMoveTargetPosition = shoppingList[shelfVisitIndex].transform.position;
            hasTargetPosition = true;
        }
        else
        {
            currentState = State.ApproachingLine;
        }
    }

    public void SetQueuePosition(Vector3 newPosition)
    {
        if (currentState == State.CheckingOut || currentState == State.Exiting) return;

        currentMoveTargetPosition = newPosition;
        hasTargetPosition = true;
        currentState = State.InQueue;
    }

    public void GoToCheckout(Vector3 checkoutPos)
    {
        currentState = State.CheckingOut;
        currentMoveTargetPosition = checkoutPos;
        hasTargetPosition = true;
    }

    public void LeaveStore(bool didPurchase)
    {
        ReputationManager.Instance.LeaveReview(didPurchase && shoppingCart.Count > 0);

        if (exitPoint != null)
        {
            currentState = State.Exiting;
            currentMoveTargetPosition = exitPoint.position;
            hasTargetPosition = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}