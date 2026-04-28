using UnityEngine;

public class ZoneAudioTrigger : MonoBehaviour
{
    [Header("ตั้งค่าเสียงคนป่า")]
    public AudioClip tribalChantSound; // 🎵 ไฟล์เสียงคนป่าร้อง
    public bool loopSound = true;      // วนลูปเสียงไหม? (แนะนำให้เปิดไว้)
    [Range(0f, 1f)]
    public float volume = 0.5f;        // ความดังของเสียง

    private AudioSource audioSource;
    private bool isPlayerInZone = false;

    void Start()
    {
        // สร้างลำโพงล่องหนซ่อนไว้ในโซนนี้
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = tribalChantSound;
        audioSource.loop = loopSound;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;

        // ทำให้เป็นเสียง 2D (เวลาอยู่ในโซนจะได้ยินเท่ากันหมด ซ้าย-ขวา)
        audioSource.spatialBlend = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินเข้ามาในเขต และเสียงยังไม่ได้เล่น
        if (other.CompareTag("Player") && !isPlayerInZone)
        {
            isPlayerInZone = true;
            if (tribalChantSound != null)
            {
                audioSource.Play();
                Debug.Log("เข้าเขตอันตราย! เสียงคนป่าเริ่มดังขึ้น");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ถ้าผู้เล่นเดินหนีออกจากเขต
        if (other.CompareTag("Player") && isPlayerInZone)
        {
            isPlayerInZone = false;
            audioSource.Stop();
            Debug.Log("หนีพ้นเขตคนป่าแล้ว เสียงเงียบลง");
        }
    }

    // วาดกล่องสีเขียวในหน้า Scene ให้เรากะระยะโซนได้ง่ายๆ
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.DrawCube(transform.position + box.center, box.size);
        }
    }
}