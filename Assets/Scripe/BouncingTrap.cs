using UnityEngine;
using System.Collections;

public class BouncingTrap : MonoBehaviour
{
    [Header("การเคลื่อนที่ของแท่น")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;

    [Header("ทิศทางและพลังกระเด็น")]
    public Vector3 bounceDirection = new Vector3(0, 1, 0);
    public float bounceForce = 15f;

    [Header("เอฟเฟกต์และเสียง")]
    public ParticleSystem hitEffect;
    public AudioClip bounceSound;    // 🎵 ช่องสำหรับใส่ไฟล์เสียง
    private AudioSource audioSource; // 🔊 ตัวลำโพง (โค้ดจะสร้างให้เอง)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // 👇 สร้าง Component ลำโพง (AudioSource) ให้กับดักแบบอัตโนมัติ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // ป้องกันไม่ให้เสียงดังเองตอนเริ่มเกม
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ApplyBounce(bounceDirection, bounceForce);

                // ==========================================
                // 🔊 ระบบเล่นเสียงเมื่อโดนชน
                // ==========================================
                if (bounceSound != null)
                {
                    // ใช้ PlayOneShot เพื่อให้เสียงเล่นทับซ้อนกันได้โดยไม่ตัดเสียงเก่าทิ้ง
                    audioSource.PlayOneShot(bounceSound);
                }

                if (hitEffect != null)
                {
                    hitEffect.transform.position = other.transform.position;
                    hitEffect.Play();
                    StartCoroutine(HideEffectRoutine());
                }
            }
        }
    }

    private IEnumerator HideEffectRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (hitEffect != null)
        {
            hitEffect.Stop();
            hitEffect.Clear();
        }
    }
}