using UnityEngine;
using UnityEngine.UI; // สำคัญมาก: ต้องมีบรรทัดนี้เพื่อเรียกใช้งาน UI

public class EnemyHealth : MonoBehaviour
{
    [Header("ตั้งค่าพลังชีวิต")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("ส่วนเชื่อมต่อ UI (หลอดเลือด)")]
    public Slider healthBarSlider; // ลาก Slider UI ของศัตรูมาใส่ช่องนี้

    void Start()
    {
        // กำหนดเลือดเริ่มต้นให้เต็ม
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้เล่นโจมตีโดน
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // ป้องกันไม่ให้เลือดติดลบ
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            // Slider ต้องการค่าระหว่าง 0 ถึง 1 เราจึงเอา เลือดปัจจุบัน / เลือดสูงสุด
            healthBarSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ถูกกำจัดแล้ว!");
        // คุณสามารถเพิ่มการดรอปไอเทม หรือเรียกเล่นเอฟเฟกต์ตายตรงนี้ได้
        Destroy(gameObject);
    }
}