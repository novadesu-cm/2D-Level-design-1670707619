using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public GameObject wall;
    public ParticleSystem smokeEffect;

    // 👇 ปรับลดตัวเลขตรงนี้ให้น้อยลง (ยิ่งน้อยยิ่งหายช้า)
    public float fadeSpeed = 0.5f;

    [Header("Sound Settings")]
    public AudioClip unlockSound;
    private AudioSource audioSource;

    private bool isOpening = false;

    void Start()
    {
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

        if (inventory.hasKey && !isOpening)
        {
            isOpening = true;

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
            Destroy(gameObject, 0.1f);
        }
    }
}