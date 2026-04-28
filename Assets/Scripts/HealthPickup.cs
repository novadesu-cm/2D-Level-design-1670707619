using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healthAmount = 20f;
    public float rotationSpeed = 100f;

    [Header("Audio")]
    public AudioClip healSound; // 🎵 เสียงฮีลเลือด

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth += healthAmount;
                playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth, 0, playerHealth.maxHealth);
                playerHealth.healthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;

                // 🔊 เล่นเสียงฮีล
                if (healSound != null)
                {
                    AudioSource.PlayClipAtPoint(healSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}