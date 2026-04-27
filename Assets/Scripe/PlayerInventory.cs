using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Items")]
    public bool hasKey = false; // ระบบกุญแจเดิมของคุณ

    [Header("Currency Settings")]
    public int totalGold = 0;
    public TextMeshProUGUI goldText; // ลาก Text UI (TextMeshPro) มาใส่ช่องนี้

    void Start()
    {
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        totalGold += amount;
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + totalGold.ToString();
        }
    }
}