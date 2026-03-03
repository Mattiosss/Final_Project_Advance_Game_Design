using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public event Action<float> OnMoneyChanged;

    [SerializeField] private float startingMoney = 100f;
    [SerializeField] private GameObject moneyPopupPrefab;

    private float currentMoney;
    public float CurrentMoney => currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentMoney = SaveManager.Instance.LoadMoney(startingMoney);
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void AddMoney(float amount, Vector3 position)
    {
        currentMoney += amount;

        if (moneyPopupPrefab != null)
        {
            GameObject popup = Instantiate(moneyPopupPrefab, position, Quaternion.identity);
            popup.GetComponent<MoneyPopup>().Setup(amount);
        }

        OnMoneyChanged?.Invoke(currentMoney);
        SaveManager.Instance.SaveMoney(currentMoney);
    }

    public void SpendMoney(float amount)
    {
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
        SaveManager.Instance.SaveMoney(currentMoney);
    }

    public bool CanAfford(float amount)
    {
        return currentMoney >= amount;
    }
}