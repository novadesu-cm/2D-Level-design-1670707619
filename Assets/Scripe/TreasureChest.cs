using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject coinPrefab;
    public int coinCount = 8;
    public float interactDistance = 3f;

    private Transform player;
    private bool isPlayerNearby = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            isPlayerNearby = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
        else
        {
            isPlayerNearby = false;
            if (interactUI != null) interactUI.SetActive(false);
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            OpenAndDestroy();
        }
    }

    void OpenAndDestroy()
    {
        // เสกเหรียญตามจำนวนที่ตั้งไว้
        for (int i = 0; i < coinCount; i++)
        {
            // เสกให้สูงจากพื้นนิดนึงเพื่อให้ Rigidbody ทำงาน
            Instantiate(coinPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        // ทำลายตัวกล่องทิ้งไปเลย (หรือจะปิด Object ก็ได้)
        Destroy(gameObject);

        // ปิด UI Interact ด้วยเพื่อความสะอาด
        if (interactUI != null) interactUI.SetActive(false);
    }
}