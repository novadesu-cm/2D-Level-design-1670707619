using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public ParticleSystem saveEffect; // เอาไว้ใส่เอฟเฟกต์ตอนเซฟ (เว้นว่างไว้ได้)

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินมาชนแท่นนี้
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // สั่งให้ผู้เล่นจำตำแหน่งของจุด Checkpoint นี้ไว้!
                player.currentRespawnPosition = transform.position;

                if (saveEffect != null)
                {
                    saveEffect.Play();
                }

                // ข้อความนี้ต้องเด้งใน Console ตอนเดินชน ถ้าไม่เด้งแปลว่าตั้งค่า Collider ผิด
                Debug.Log("เซฟจุดเกิดเรียบร้อย!");
            }
        }
    }
}