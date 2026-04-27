using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public ParticleSystem saveEffect;
    public AudioClip saveSound; // 🎵 เสียงตอนเซฟจุดเกิด
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // ถ้าเซฟตำแหน่งเดิมซ้ำ ไม่ต้องเล่นเสียง (เพื่อไม่ให้หนวกหู)
                if (player.currentRespawnPosition != transform.position)
                {
                    player.currentRespawnPosition = transform.position;

                    if (saveSound != null)
                    {
                        audioSource.PlayOneShot(saveSound);
                    }

                    if (saveEffect != null) saveEffect.Play();
                    Debug.Log("เซฟจุดเกิดและเล่นเสียงเรียบร้อย!");
                }
            }
        }
    }
}