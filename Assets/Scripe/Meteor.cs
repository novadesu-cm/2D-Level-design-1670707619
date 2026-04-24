using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 30f;
    public float lifeTime = 5f;
    public GameObject explosionEffect; // เอฟเฟกต์ระเบิดตอนถึงพื้น
    public AudioClip explosionSound;

    void Start()
    {
        // ถ้าตกไม่โดนอะไรเลย ให้ทำลายตัวเองทิ้งใน 5 วิ
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // ให้อุกกาบาตพุ่งลงข้างล่างตลอดเวลา
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // มี OnTriggerEnter แค่อันเดียวเท่านั้น
    private void OnTriggerEnter(Collider other)
    {
        // 1. ถ้าชนผู้เล่น ให้สั่งตายและกลับจุดเซฟ!
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Die();
                Debug.Log("โดนอุกกาบาตทับหัว! กลับจุดเซฟ!");
            }
        }

        // 2. เอฟเฟกต์ระเบิดเมื่อชนพื้นหรือชนผู้เล่น
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 3. ทำลายตัวอุกกาบาตทิ้งทันทีที่ชน
        Destroy(gameObject);
    }
}