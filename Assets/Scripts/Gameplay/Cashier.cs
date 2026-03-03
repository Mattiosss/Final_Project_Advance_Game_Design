using UnityEngine;
using System.Collections.Generic;

public class Cashier : MonoBehaviour
{
    [SerializeField] private GameObject coinBubblePrefab;
    [SerializeField] private Vector3 coinBubbleOffset = new Vector3(0, 1, 0);
    [SerializeField] private Transform checkoutPosition;
    [SerializeField] private Vector3 queueDirection = new Vector3(0, -1.5f, 0);

    private List<Customer> customerQueue = new List<Customer>();
    private bool isBusy = false;
    private Customer currentCustomer;

    public Vector3 GetLineTailPosition()
    {
        if (checkoutPosition == null) return transform.position;

        int spotsTaken = customerQueue.Count + (currentCustomer != null ? 1 : 0);
        return checkoutPosition.position + (queueDirection * spotsTaken);
    }

    public void JoinQueue(Customer customer)
    {
        customerQueue.Add(customer);

        if (!isBusy)
        {
            ProcessNextInQueue();
        }
        else
        {
            UpdateQueuePositions();
        }
    }

    private void ProcessNextInQueue()
    {
        if (customerQueue.Count > 0 && !isBusy)
        {
            isBusy = true;
            currentCustomer = customerQueue[0];
            customerQueue.RemoveAt(0);

            currentCustomer.GoToCheckout(checkoutPosition.position);
            UpdateQueuePositions();
        }
    }

    public void OnCustomerArrivedAtCounter(Customer customer, List<Product> products)
    {
        if (coinBubblePrefab != null && checkoutPosition != null)
        {
            Vector3 spawnPos = checkoutPosition.position + coinBubbleOffset;
            GameObject bubble = Instantiate(coinBubblePrefab, spawnPos, Quaternion.identity);
            CoinBubble coinBubble = bubble.GetComponent<CoinBubble>();
            if (coinBubble != null)
            {
                coinBubble.Setup(this, customer, products);
            }
        }
    }

    public void TransactionComplete()
    {
        isBusy = false;
        currentCustomer = null;
        ProcessNextInQueue();
    }

    private void UpdateQueuePositions()
    {
        if (checkoutPosition == null) return;

        for (int i = 0; i < customerQueue.Count; i++)
        {
            Vector3 targetPos = checkoutPosition.position + (queueDirection * (i + 1));
            customerQueue[i].SetQueuePosition(targetPos);
        }
    }
}