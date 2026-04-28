using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public float bounceForce = 7f;
    public int goldValue = 10;

    [Header("Audio")]
    public AudioClip pickupSound; // 🎵 เสียงตอนเก็บเหรียญ

    private Rigidbody rb;
    private bool iscollected = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;
        if (rb != null) rb.AddForce(randomDir * bounceForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (iscollected) return;

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory == null) inventory = other.GetComponentInParent<PlayerInventory>();

            if (inventory != null)
            {
                iscollected = true;
                inventory.AddGold(goldValue);

                // 🔊 เล่นเสียง ณ ตำแหน่งที่เก็บ ก่อนทำลายทิ้ง
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}