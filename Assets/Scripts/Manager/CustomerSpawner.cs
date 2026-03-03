using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Cashier[] cashiers;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float baseSpawnInterval = 10f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnCustomer();
            spawnTimer = baseSpawnInterval * Mathf.Clamp(1f / (ReputationManager.Instance.CurrentReputation / 10f), 0.2f, 2f);
        }
    }

    private void SpawnCustomer()
    {
        if (cashiers.Length == 0) return;
        GameObject go = Instantiate(customerPrefab, transform.position, Quaternion.identity);
        go.GetComponent<Customer>().Initialize(cashiers[Random.Range(0, cashiers.Length)], exitPoint);
    }
}