using UnityEngine;

public class EnemyTriggerZone : MonoBehaviour
{
    [Header("มอนสเตอร์ที่จะสั่งให้ไล่ล่า")]
    public EnemyChargeAI[] enemiesToWake; // ทำเป็น Array เพื่อให้สั่งปลุกมอนสเตอร์ได้หลายตัวพร้อมกัน!

    private bool hasTriggered = false; // เอาไว้เช็คว่าเคยสั่งปลุกไปหรือยัง จะได้ไม่รันซ้ำ

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินเข้ามา และยังไม่เคยสั่งปลุกมอนสเตอร์
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // บันทึกไว้ว่าสั่งปลุกแล้ว
            Debug.Log("เข้าเขตซุ่มโจมตี! สั่งมอนสเตอร์ออกล่า!");

            // วนลูปสั่งมอนสเตอร์ทุกตัวที่อยู่ในรายชื่อให้ตื่น
            foreach (EnemyChargeAI enemy in enemiesToWake)
            {
                if (enemy != null)
                {
                    enemy.ForceAggro(); // เรียกใช้คำสั่งบังคับไล่ล่า
                }
            }
        }
    }
}