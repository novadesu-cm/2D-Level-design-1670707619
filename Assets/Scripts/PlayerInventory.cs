using UnityEngine;
using TMPro; // 👈 ต้องมีบรรทัดนี้!

public class PlayerInventory : MonoBehaviour
{
    [Header("Items")]
    public bool hasKey = false;

    [Header("Money Settings")]
    public int totalGold = 0;
    public TextMeshProUGUI goldText; // ลาก Text (TMP) มาใส่ตรงนี้

    void Start()
    {
        // อัปเดตเลขครั้งแรกตอนเริ่มเกม
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        totalGold += amount;
        UpdateGoldUI();
        Debug.Log("เก็บเงินได้แล้ว! ตอนนี้มี: " + totalGold);
    }

    public void UpdateGoldUI()
    {
        if (goldText != null)
        {
            // บรรทัดนี้จะไปเขียนทับข้อความเก่าบนหน้าจอ UI ทันที
            goldText.text = "Gold: " + totalGold.ToString();
        }
        else
        {
            // ถ้าเลขไม่ขึ้น ให้มาดูใน Console มันจะบอกว่าลืมลากอะไร
            Debug.LogWarning("⚠️ เตือน: คุณลืมลาก UI Text มาใส่ในช่อง Gold Text บนตัว Player นะ!");
        }
    }
}