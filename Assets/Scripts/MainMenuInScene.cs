using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems; // 📌 1. ต้องเพิ่มบรรทัดนี้เข้ามาด้วย!

public class MainMenuInScene : MonoBehaviour
{
    [Header("แผง UI หน้าเมนู")]
    public GameObject menuPanel;

    [Header("ข้อความปุ่มเริ่มเกม")]
    public TMP_Text startButtonText;

    private bool isGameOver = false;

    void Start()
    {
        ShowMenu();
    }

    public void StartGame()
    {
        // 📌 2. เพิ่มโค้ดบรรทัดนี้ เพื่อเคลียร์การโฟกัส คืนปุ่ม Spacebar ให้ตัวละคร!
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (isGameOver)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QuitGame()
    {
        Debug.Log("กำลังออกจากเกม...");
        Application.Quit();
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowEndGameMenu()
    {
        isGameOver = true;
        if (startButtonText != null) startButtonText.text = "Restart";
        ShowMenu();
    }
}