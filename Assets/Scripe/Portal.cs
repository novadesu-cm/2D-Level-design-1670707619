using UnityEngine;
using UnityEngine.SceneManagement; // 👈 จำเป็นต้องมีเพื่อใช้คำสั่งเปลี่ยนด่าน

public class Portal : MonoBehaviour
{
    [Header("ชื่อฉากที่จะวาร์ปไป")]
    public string sceneName = "PLAY 2";

    [Header("เอฟเฟกต์ (ถ้ามี)")]
    public ParticleSystem teleportEffect;

    private void OnTriggerEnter(Collider other)
    {
        // เช็คว่าผู้เล่นเดินมาชนประตูหรือไม่
        if (other.CompareTag("Player"))
        {
            Debug.Log("วาร์ปไปด่าน: " + sceneName);

            if (teleportEffect != null)
            {
                teleportEffect.Play();
            }

            // สั่งโหลดฉากใหม่ทันที
            SceneManager.LoadScene(sceneName);
        }
    }
}