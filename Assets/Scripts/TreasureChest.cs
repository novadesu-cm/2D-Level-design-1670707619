using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject coinPrefab;
    public int coinCount = 8;
    public float interactDistance = 3f;

    [Header("Audio")]
    public AudioClip openSound; // 🎵 เสียงเปิดกล่อง

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
        // 🔊 เล่นเสียงเปิดกล่อง
        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        for (int i = 0; i < coinCount; i++)
        {
            Instantiate(coinPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        Destroy(gameObject);
        if (interactUI != null) interactUI.SetActive(false);
    }
}