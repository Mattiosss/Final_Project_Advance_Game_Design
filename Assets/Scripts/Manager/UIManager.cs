using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI reputationText; [SerializeField] private TextMeshProUGUI totalReviewsText;
    [SerializeField] private TextMeshProUGUI positiveReviewsText;
    [SerializeField] private TextMeshProUGUI negativeReviewsText;

    [SerializeField] private Transform reviewGridContainer;
    [SerializeField] private GameObject reviewTextPrefab;

    private void OnDisable()
    {
        EconomyManager.Instance.OnMoneyChanged -= UpdateMoneyText;
        ReputationManager.Instance.OnReputationChanged -= UpdateReputationText;
        ReputationManager.Instance.OnNewReview -= AddReview;
        ReputationManager.Instance.OnTalliesUpdated -= UpdateTallies;
    }

    private void Start()
    {
        EconomyManager.Instance.OnMoneyChanged += UpdateMoneyText;
        ReputationManager.Instance.OnReputationChanged += UpdateReputationText;
        ReputationManager.Instance.OnNewReview += AddReview;
        ReputationManager.Instance.OnTalliesUpdated += UpdateTallies;

        UpdateMoneyText(EconomyManager.Instance.CurrentMoney);
        UpdateReputationText(ReputationManager.Instance.CurrentReputation);

        RepopulateReviews();
    }

    private void UpdateMoneyText(float amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {Mathf.FloorToInt(amount)}";
        }
    }

    private void UpdateReputationText(int amount)
    {
        if (reputationText != null)
        {
            reputationText.text = $"Reputation: {amount}";
        }
    }

    private void UpdateTallies(int total, int positive, int negative)
    {
        if (totalReviewsText != null) totalReviewsText.text = $"Total Reviews: {total}";
        if (positiveReviewsText != null) positiveReviewsText.text = $"Positive: {positive}";
        if (negativeReviewsText != null) negativeReviewsText.text = $"Negative: {negative}";
    }

    private void RepopulateReviews()
    {
        if (reviewGridContainer == null) return;

        foreach (Transform child in reviewGridContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string review in ReputationManager.Instance.ReviewsList)
        {
            AddReview(review);
        }
    }

    private void AddReview(string review)
    {
        if (reviewGridContainer != null && reviewTextPrefab != null)
        {
            GameObject reviewObject = Instantiate(reviewTextPrefab, reviewGridContainer);
            TextMeshProUGUI reviewTextComponent = reviewObject.GetComponent<TextMeshProUGUI>();
            if (reviewTextComponent != null)
            {
                reviewTextComponent.text = $"- {review}";
            }
        }
    }
}