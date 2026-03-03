using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shelf : MonoBehaviour
{
    [SerializeField] private string shelfID;
    [SerializeField] private Product product;
    [SerializeField] private int maxStock = 100;
    [SerializeField] private TextMeshProUGUI productText;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private Button restockButton;
    [SerializeField] private TextMeshProUGUI restockButtonText;

    private int currentStock;

    private void Start()
    {
        EconomyManager.Instance.OnMoneyChanged += _ => UpdateRestockUI();

        currentStock = SaveManager.Instance.LoadShelfStock(shelfID, maxStock);
        productText.text = product.ProductName;
        UpdateStockDisplay();
        UpdateRestockUI();

        if (restockButton != null)
        {
            restockButton.onClick.AddListener(Restock);
        }
    }

    private void OnDisable()
    {
        EconomyManager.Instance.OnMoneyChanged -= _ => UpdateRestockUI();
    }

    public void Restock()
    {
        int neededStock = maxStock - currentStock;
        if (neededStock <= 0) return;

        float totalCost = neededStock * product.RestockCostPerUnit;

        if (EconomyManager.Instance.CanAfford(totalCost))
        {
            EconomyManager.Instance.SpendMoney(totalCost);
            currentStock = maxStock;
            SaveManager.Instance.SaveShelfStock(shelfID, currentStock);
            UpdateStockDisplay();
            UpdateRestockUI();
        }
    }

    public bool HasStock() => currentStock > 0;

    public Product TakeProduct()
    {
        if (HasStock())
        {
            currentStock--;
            SaveManager.Instance.SaveShelfStock(shelfID, currentStock);
            UpdateStockDisplay();
            UpdateRestockUI();
            return product;
        }
        return null;
    }

    private void UpdateStockDisplay()
    {
        if (stockText != null) stockText.text = $"Stock: {currentStock}";
    }

    private void UpdateRestockUI()
    {
        if (restockButton == null || restockButtonText == null) return;
        int needed = maxStock - currentStock;
        float restockCost = needed * product.RestockCostPerUnit;
        restockButton.interactable = needed > 0 && EconomyManager.Instance.CanAfford(restockCost);
        restockButtonText.text = needed > 0 ? $"Restock: PHP {restockCost}" : "Full";
    }
}