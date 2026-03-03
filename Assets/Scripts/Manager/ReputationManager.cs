using UnityEngine;
using System;
using System.Collections.Generic;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    public event Action<int> OnReputationChanged;

    public event Action<string> OnNewReview;

    public event Action<int, int, int> OnTalliesUpdated;

    [SerializeField] private int startingReputation = 100;
    [SerializeField] private int maxReputation = 100;
    [SerializeField] private int reputationGain = 5;
    [SerializeField] private int reputationLoss = 10;

    private List<string> reviewsList = new List<string>();
    private int totalReviews;
    private int positiveReviews;
    private int negativeReviews;

    private const string REPUTATION_KEY = "PlayerReputation";
    private const string REVIEWS_LIST_KEY = "StoredReviews";
    private const string TOTAL_KEY = "TotalReviews";
    private const string POSITIVE_KEY = "PositiveReviews";
    private const string NEGATIVE_KEY = "NegativeReviews";
    private const string DELIMITER = "||";

    public int CurrentReputation { get; private set; }
    public List<string> ReviewsList => reviewsList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadData();
    }

    private void Start()
    {
        OnReputationChanged?.Invoke(CurrentReputation);
        OnTalliesUpdated?.Invoke(totalReviews, positiveReviews, negativeReviews);
    }

    public void LeaveReview(bool wasPositive)
    {
        string reviewText;
        totalReviews++;

        if (wasPositive)
        {
            positiveReviews++;
            CurrentReputation += reputationGain;
            reviewText = "Great service! Found what I needed.";
        }
        else
        {
            negativeReviews++;
            CurrentReputation -= reputationLoss;
            reviewText = "Couldn't find what I was looking for.";
        }

        CurrentReputation = Mathf.Clamp(CurrentReputation, 0, maxReputation);

        reviewsList.Add(reviewText);

        OnNewReview?.Invoke(reviewText);
        OnReputationChanged?.Invoke(CurrentReputation);
        OnTalliesUpdated?.Invoke(totalReviews, positiveReviews, negativeReviews);

        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(REPUTATION_KEY, CurrentReputation);
        PlayerPrefs.SetInt(TOTAL_KEY, totalReviews);
        PlayerPrefs.SetInt(POSITIVE_KEY, positiveReviews);
        PlayerPrefs.SetInt(NEGATIVE_KEY, negativeReviews);

        string serializedReviews = string.Join(DELIMITER, reviewsList);
        PlayerPrefs.SetString(REVIEWS_LIST_KEY, serializedReviews);

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        CurrentReputation = PlayerPrefs.GetInt(REPUTATION_KEY, startingReputation);
        totalReviews = PlayerPrefs.GetInt(TOTAL_KEY, 0);
        positiveReviews = PlayerPrefs.GetInt(POSITIVE_KEY, 0);
        negativeReviews = PlayerPrefs.GetInt(NEGATIVE_KEY, 0);

        string savedReviews = PlayerPrefs.GetString(REVIEWS_LIST_KEY, "");
        if (!string.IsNullOrEmpty(savedReviews))
        {
            string[] splitReviews = savedReviews.Split(new[] { DELIMITER }, StringSplitOptions.None);
            reviewsList = new List<string>(splitReviews);
        }
    }
}