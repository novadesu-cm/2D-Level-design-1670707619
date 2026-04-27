using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public AudioClip pickupSound; // 🎵 ใส่ไฟล์เสียงเก็บของ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // เล่นเสียงตรงตำแหน่งกุญแจก่อนจะ Destroy ตัวเอง
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            other.GetComponent<PlayerInventory>().hasKey = true;
            Destroy(gameObject);
        }
    }
}