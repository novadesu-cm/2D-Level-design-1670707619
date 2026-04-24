using UnityEngine;
using System.Collections;

public class MeteorTrap : MonoBehaviour
{
    [Header("ตั้งค่าอุกกาบาต")]
    public GameObject meteorPrefab;  // ลากโมเดลอุกกาบาตมาใส่ที่นี่
    public float delayTime = 3f;     // เวลาหน่วง (3 วินาที)
    public float spawnHeight = 20f;  // ความสูงที่จะเสกอุกกาบาตขึ้นไปบนฟ้า

    [Header("เอฟเฟกต์เตือน (ถ้ามี)")]
    public GameObject warningSign;   // เช่น วงกลมสีแดงบนพื้นเพื่อเตือนผู้เล่น

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // ถ้าผู้เล่นเดินมาชน และกับดักยังไม่ทำงาน
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true; // ป้องกันไม่ให้ทำงานซ้ำซ้อน
            StartCoroutine(MeteorRoutine(other.transform));
        }
    }

    IEnumerator MeteorRoutine(Transform playerTransform)
    {
        // 1. เปิดเอฟเฟกต์เตือน (ถ้าใส่ไว้)
        if (warningSign != null) warningSign.SetActive(true);

        Debug.Log("อุกกาบาตกำลังจะตกลงมาในอีก 3 วินาที!");

        // 2. รอตามเวลาที่ตั้งไว้ (3 วินาที)
        yield return new WaitForSeconds(delayTime);

        // 3. คำนวณตำแหน่งที่จะเสก (เสกตรงหัวผู้เล่น ณ วินาทีนั้นเลย)
        Vector3 spawnPos = playerTransform.position + Vector3.up * spawnHeight;

        // 4. เสกอุกกาบาตออกมา
        Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        // 5. ปิดเอฟเฟกต์เตือน
        if (warningSign != null) warningSign.SetActive(false);

        // (เลือกได้) ทำลายจุดวางทิ้งไปเลยเพื่อใช้ได้ครั้งเดียว
        // Destroy(gameObject, 1f); 
    }
}