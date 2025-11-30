using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // ตรวจสอบว่า Time.timeScale เป็น 1 (กรณีมาจากเกมที่ Pause)
        Time.timeScale = 1f;

        // Setup Buttons
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ Start Button ไม่ได้ลาก Reference!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ Quit Button ไม่ได้ลาก Reference!");
        }
    }

    public void OnStartClicked()
    {
        Debug.Log("🎮 Starting Game...");

        // โหลด Scene_1
        SceneManager.LoadScene("Scene_1");

        // หรือใช้ index: SceneManager.LoadScene(1);
    }

    public void OnQuitClicked()
    {
        Debug.Log("👋 Quit Game");
    }
}