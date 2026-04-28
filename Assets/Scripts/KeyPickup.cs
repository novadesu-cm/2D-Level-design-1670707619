using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Hover Settings (ตั้งค่าการลอย)")]
    public float hoverSpeed = 2f;    // ความเร็วในการลอยขึ้นลง
    public float hoverHeight = 0.3f; // ระยะความสูงในการลอย (ยิ่งเยอะยิ่งลอยสูง)

    [Header("Audio (ระบบเสียง)")]
    public AudioClip pickupSound; // 🎵 ใส่ไฟล์เสียงเก็บของ

    private Vector3 startPosition;

    void Start()
    {
        // 📌 จำตำแหน่งเริ่มต้นของกุญแจไว้ก่อน เพื่อให้มันลอยอยู่กับที่ ไม่ลอยหนีไปไหน
        startPosition = transform.position;
    }

    void Update()
    {
        // 🌊 สั่งให้แกน Y ขยับขึ้นลงเป็นรูปคลื่นแบบนุ่มนวล
        float newY = startPosition.y + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // เล่นเสียงตรงตำแหน่งกุญแจก่อนจะ Destroy ตัวเอง
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            other.GetComponent<PlayerInventory>().hasKey = true;
            Destroy(gameObject);
        }
    }
}