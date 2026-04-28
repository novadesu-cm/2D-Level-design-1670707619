using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("ตั้งค่าพลังชีวิต")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthBarSlider;

    [Header("ระบบดรอปไอเทม")]
    public GameObject healthItemPrefab;
    [Range(0, 100)]
    public float dropChance = 50f;

    [Header("ระบบเสียง (Audio)")]
    public AudioClip hurtSound;  // 🎵 เสียงโดนฟัน (ร้องเจ็บ)
    public AudioClip deathSound; // 🎵 เสียงตาย
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // 🔊 เล่นเสียงเจ็บ
        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider != null) healthBarSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        float randomRoll = Random.Range(0f, 100f);
        if (randomRoll <= dropChance && healthItemPrefab != null)
        {
            Instantiate(healthItemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        // 🔊 เล่นเสียงตายแบบปล่อยทิ้งไว้ (เพราะตัวมันจะโดน Destroy ทันที)
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);

        Destroy(gameObject);
    }
}