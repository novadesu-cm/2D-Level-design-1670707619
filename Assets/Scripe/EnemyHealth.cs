using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("ตั้งค่าพลังชีวิต")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthBarSlider;

    [Header("ระบบดรอปไอเทม")]
    public GameObject healthItemPrefab; // ลาก Prefab ขวดยามาใส่ช่องนี้
    [Range(0, 100)]
    public float dropChance = 50f;     // โอกาสดรอป (เช่น 50%)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
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
            healthBarSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        // สุ่มดรอปไอเทม
        float randomRoll = Random.Range(0f, 100f);
        if (randomRoll <= dropChance && healthItemPrefab != null)
        {
            // เสกยาขึ้นมาตรงตำแหน่งที่มอนสเตอร์ตาย
            Instantiate(healthItemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        Debug.Log(gameObject.name + " ตายแล้ว!");
        Destroy(gameObject);
    }
}