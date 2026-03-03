using UnityEngine;
using TMPro;

public class MoneyPopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 1f;

    private float timer;

    public void Setup(float amount)
    {
        textMesh.text = $"+PHP {amount:F0}";
        timer = lifetime;
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}