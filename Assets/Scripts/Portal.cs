using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ต้องใช้สำหรับหน่วงเวลา

public class Portal : MonoBehaviour
{
    [Header("ชื่อฉากที่จะวาร์ปไป")]
    public string sceneName = "PLAY 2";

    [Header("เอฟเฟกต์ & เสียง")]
    public ParticleSystem teleportEffect;
    public AudioClip teleportSound; // 🎵 เสียงวาร์ปหวิวๆ

    private bool isWarping = false; // กันเดินชนซ้ำรัวๆ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isWarping)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        isWarping = true;

        // 🔊 เล่นเสียงวาร์ปและเอฟเฟกต์
        if (teleportSound != null) AudioSource.PlayClipAtPoint(teleportSound, transform.position);
        if (teleportEffect != null) teleportEffect.Play();

        // รอ 1 วินาทีให้เสียงกับเอฟเฟกต์โชว์ก่อนเปลี่ยนฉาก
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }
}