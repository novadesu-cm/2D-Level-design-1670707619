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

    // 👇 เพิ่มตั้งค่าพลังโจมตีของกับดักตรงนี้
    [Header("ความเสียหาย (Damage)")]
    public float trapDamage = 15f;

    [Header("เอฟเฟกต์และเสียง")]
    public ParticleSystem hitEffect;
    public AudioClip bounceSound;    // 🎵 ช่องสำหรับใส่ไฟล์เสียง
    private AudioSource audioSource; // 🔊 ตัวลำโพง (โค้ดจะสร้างให้เอง)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // สร้าง Component ลำโพงอัตโนมัติ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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
                // 1. สั่งให้ผู้เล่นกระเด็น
                player.ApplyBounce(bounceDirection, bounceForce);

                // 2. 👇 สั่งให้ลดเลือดผู้เล่น (เพิ่มใหม่)
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(trapDamage);
                    Debug.Log("โดนกับดักเด้ง! พลังชีวิตลดลง: " + trapDamage);
                }

                // 3. เล่นเสียง
                if (bounceSound != null)
                {
                    audioSource.PlayOneShot(bounceSound);
                }

                // 4. เล่นเอฟเฟกต์
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