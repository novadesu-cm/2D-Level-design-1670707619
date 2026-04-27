using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    public float bounceForce = 7f;
    public int goldValue = 10;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // สุ่มทิศทางให้เหรียญกระเด็นออกแบบกระจายๆ
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;
        rb.AddForce(randomDir * bounceForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddGold(goldValue);
                // เพิ่มเสียงเก็บเหรียญตรงนี้ได้
                Destroy(gameObject);
            }
        }
    }
}