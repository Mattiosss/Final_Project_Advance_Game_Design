using UnityEngine;

[CreateAssetMenu(fileName = "New Product", menuName = "Gameplay/Product")]
public class Product : ScriptableObject
{
    [SerializeField] private string productName;
    [SerializeField] private float restockCostPerUnit;
    [SerializeField] private float salePricePerUnit;

    public string ProductName => productName;
    public float RestockCostPerUnit => restockCostPerUnit;
    public float SalePricePerUnit => salePricePerUnit;
}