using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public GameObject wall;
    public ParticleSystem smokeEffect;
    public float fadeSpeed = 2f;

    [Header("Sound Settings")]
    public AudioClip unlockSound; // 🎵 เสียงตอนปลดล็อกประตู
    private AudioSource audioSource;

    private bool isOpening = false;

    void Start()
    {
        // สร้างลำโพงไว้ที่ตัวประตู
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isOpening)
        {
            FadeWall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryUnlock(other.gameObject);
        }
    }

    public void TryUnlock(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null) return;

        if (inventory.hasKey && !isOpening) // เพิ่ม !isOpening เพื่อไม่ให้เสียงเล่นซ้ำรัวๆ
        {
            isOpening = true;

            // 🔊 เล่นเสียงปลดล็อก
            if (unlockSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(unlockSound);
            }

            if (smokeEffect != null) smokeEffect.Play();
        }
    }

    void FadeWall()
    {
        if (wall == null) return;

        Renderer rend = wall.GetComponent<Renderer>();
        Color color = rend.material.color;

        color.a -= Time.deltaTime * fadeSpeed;
        rend.material.color = color;

        if (color.a <= 0)
        {
            Destroy(wall);
            // ทำลายตัวประตู (Object นี้) ทิ้งไปด้วยเมื่อกำแพงหายหมดแล้ว เพื่อล้าง Error
            Destroy(gameObject, 0.1f);
        }
    }
}