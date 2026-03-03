using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CoinBubble : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float lifetime = 4.0f;
    [SerializeField] private TextMeshProUGUI lifetimeText;

    private Cashier cashier;
    private Customer customer;
    private List<Product> products;

    private float currentTimer;
    private bool isExpired = false;

    public void Setup(Cashier sourceCashier, Customer c, List<Product> p)
    {
        cashier = sourceCashier;
        customer = c;
        products = p;

        currentTimer = lifetime;
        lifetimeText.text = Mathf.CeilToInt(lifetime).ToString();
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
        }
    }

    private void Update()
    {
        if (isExpired) return;

        currentTimer -= Time.deltaTime;
        lifetimeText.text = Mathf.CeilToInt(currentTimer).ToString();

        if (fillImage != null)
        {
            fillImage.fillAmount = currentTimer / lifetime;
        }

        if (currentTimer <= 0)
        {
            HandleTimeout();
        }
    }

    private void OnMouseDown()
    {
        if (isExpired) return;
        isExpired = true;

        if (customer != null && products != null && products.Count > 0)
        {
            float totalSale = 0;
            foreach (var product in products)
            {
                totalSale += product.SalePricePerUnit;
            }

            EconomyManager.Instance.AddMoney(totalSale, transform.position);
            AudioManager.Instance.PlayMusic("CashRegisterSFX");
            customer.LeaveStore(true);
        }
        else if (customer != null)
        {
            customer.LeaveStore(false);
        }

        cashier.TransactionComplete();
        Destroy(gameObject);
    }

    private void HandleTimeout()
    {
        isExpired = true;
        AudioManager.Instance.PlayMusic("TimeoutSFX");

        if (customer != null)
        {
            customer.LeaveStore(false);
        }

        cashier.TransactionComplete();
        Destroy(gameObject);
    }
}