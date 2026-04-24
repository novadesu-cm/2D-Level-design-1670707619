using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public GameObject wall;
    public ParticleSystem smokeEffect;
    public float fadeSpeed = 2f;

    private bool isOpening = false;

    void Update()
    {
        if (isOpening)
        {
            FadeWall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. เช็คว่ามีวัตถุอะไรมาชนประตูไหม
        Debug.Log("มีวัตถุมาชนประตูชื่อ: " + other.name);

        if (other.CompareTag("Player"))
        {
            // 2. เช็คว่าแท็กตรงไหม
            Debug.Log("ระบบตรวจพบว่าผู้เล่น (Player) เดินมาชนประตูแล้ว!");
            TryUnlock(other.gameObject);
        }
    }

    public void TryUnlock(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        // 3. ป้องกันบัคกรณีที่หา PlayerInventory ไม่เจอ
        if (inventory == null)
        {
            Debug.LogError("หาสคริปต์ PlayerInventory ในตัวละครไม่เจอ!");
            return;
        }

        if (inventory.hasKey)
        {
            // 4. ถ้ามีกุญแจ ระบบจะทำงานตรงนี้
            Debug.Log("ผู้เล่นมีกุญแจ! ประตูกำลังเปิด...");
            isOpening = true;

            // ป้องกันบัคถ้าลืมใส่เอฟเฟกต์ควันใน Inspector
            if (smokeEffect != null)
            {
                smokeEffect.Play();
            }
            else
            {
                Debug.LogWarning("แจ้งเตือน: คุณลืมลาก Smoke Effect มาใส่ในหน้าต่าง Inspector");
            }
        }
        else
        {
            Debug.Log("เปิดไม่ได้: ผู้เล่นยังไม่มีกุญแจ!");
        }
    }

    void FadeWall()
    {
        // ป้องกันบัคถ้าลืมใส่กำแพงใน Inspector
        if (wall == null)
        {
            Debug.LogError("Error: คุณลืมลากกำแพง (Wall) มาใส่ในหน้าต่าง Inspector");
            return;
        }

        Renderer rend = wall.GetComponent<Renderer>();
        Color color = rend.material.color;

        color.a -= Time.deltaTime * fadeSpeed;
        rend.material.color = color;

        if (color.a <= 0)
        {
            Destroy(wall);
        }
    }
}