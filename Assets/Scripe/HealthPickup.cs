using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healthAmount = 20f; // ปริมาณเลือดที่จะเติม
    public float rotationSpeed = 100f;

    void Update()
    {
        // หมุนไอเทมให้ดูสวยงาม
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                // เติมเลือดให้ผู้เล่น
                playerHealth.currentHealth += healthAmount;

                // ป้องกันเลือดเกิน Max
                playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth, 0, playerHealth.maxHealth);

                // อัปเดต UI หลอดเลือด
                playerHealth.healthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;

                Debug.Log("เก็บยา! เลือดปัจจุบัน: " + playerHealth.currentHealth);

                Destroy(gameObject); // เก็บแล้วหายไป
            }
        }
    }
}