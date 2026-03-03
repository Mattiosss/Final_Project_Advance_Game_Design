using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string MONEY_KEY = "PlayerMoney";
    private const string SHELF_STOCK_PREFIX = "ShelfStock_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SaveMoney(float amount)
    {
        PlayerPrefs.SetFloat(MONEY_KEY, amount);
        PlayerPrefs.Save();
    }

    public float LoadMoney(float defaultAmount)
    {
        return PlayerPrefs.GetFloat(MONEY_KEY, defaultAmount);
    }

    public void SaveShelfStock(string shelfId, int stock)
    {
        PlayerPrefs.SetInt(SHELF_STOCK_PREFIX + shelfId, stock);
        PlayerPrefs.Save();
    }

    public int LoadShelfStock(string shelfId, int defaultStock)
    {
        return PlayerPrefs.GetInt(SHELF_STOCK_PREFIX + shelfId, defaultStock);
    }
}