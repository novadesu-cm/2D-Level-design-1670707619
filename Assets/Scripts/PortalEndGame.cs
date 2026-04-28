using UnityEngine;

public class PortalEndGame : MonoBehaviour
{
    [Header("ลาก GameManager มาใส่ช่องนี้")]
    public MainMenuInScene menuManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🎉 ผู้เล่นเข้าพอร์ทัลแล้ว! จบเกม!");

            if (menuManager != null)
            {
                menuManager.ShowEndGameMenu(); // สั่งให้เมนูเด้งขึ้นมา
            }
        }
    }
}