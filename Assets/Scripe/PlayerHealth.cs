using UnityEngine;
using UnityEngine.UI; // 👈 สำคัญมาก: ต้องมีบรรทัดนี้เพื่อเรียกใช้งาน UI

public class PlayerHealth : MonoBehaviour
{
    [Header("ตั้งค่าพลังชีวิต")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI หลอดเลือด")]
    public Slider healthBar; // 👈 ช่องสำหรับลาก UI Slider มาใส่

    private PlayerMovement movement;

    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<PlayerMovement>();

        // อัปเดตหลอดเลือดให้เต็มตอนเริ่มเกม
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // ป้องกันไม่ให้เลือดติดลบ
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
        Debug.Log("เลือดผู้เล่นเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        // ถ้าใส่ UI ไว้ ให้คำนวณค่าเลือดส่งไปที่หลอด
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("ผู้เล่นตายแล้ว!");

        // รีเซ็ตเลือดให้เต็มและอัปเดตหลอดเลือดเพื่อเริ่มใหม่ที่จุด Checkpoint
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (movement != null)
        {
            movement.Die();
        }
    }
}