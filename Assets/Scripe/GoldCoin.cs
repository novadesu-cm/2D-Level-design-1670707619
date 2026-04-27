using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public float bounceForce = 7f; // แรงกระเด็นตอนออกมาจากกล่อง
    public int goldValue = 10;     // มูลค่าเหรียญ

    private Rigidbody rb;
    private bool iscollected = false; // ตัวแปรกันบัคเก็บเหรียญซ้ำซ้อนในเฟรมเดียว

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 💥 ระบบสุ่มแรงกระเด็นตอนเหรียญถูกเสกออกมา
        // สุ่มทิศทาง X และ Z เพื่อให้เหรียญกระจายตัวออกรอบๆ
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;

        if (rb != null)
        {
            rb.AddForce(randomDir * bounceForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าถูกเก็บไปแล้ว หรือไม่ใช่ Player ไม่ต้องทำงานต่อ
        if (iscollected) return;

        if (other.CompareTag("Player")) // ⚠️ ต้องตั้ง Tag ที่ตัวผู้เล่นเป็น Player ด้วย!
        {
            // 🔍 ค้นหาสคริปต์ Inventory จากตัวที่ชน หรือตัว Parent (ในกรณีที่ Collider อยู่ลูก)
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory == null)
                inventory = other.GetComponentInParent<PlayerInventory>();

            if (inventory != null)
            {
                iscollected = true;

                // 💰 เพิ่มเงินเข้ากระเป๋าผู้เล่น
                inventory.AddGold(goldValue);

                Debug.Log("เก็บเหรียญสำเร็จ! ได้รับ: " + goldValue);

                // 🗑️ ทำลายเหรียญทิ้ง
                Destroy(gameObject);
            }
            else
            {
                // ถ้าขึ้นข้อความนี้ แปลว่าที่ตัว Player ลืมใส่สคริปต์ PlayerInventory
                Debug.LogError("Error: ชนผู้เล่นแล้วแต่ไม่เจอสคริปต์ PlayerInventory! ตรวจสอบที่ตัวละครด้วย");
            }
        }
    }
}