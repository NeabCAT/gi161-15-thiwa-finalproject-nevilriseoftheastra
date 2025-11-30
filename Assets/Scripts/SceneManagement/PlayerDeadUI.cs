using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI แสดงเมื่อ Player ตาย
/// </summary>
public class PlayerDeadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI deadText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float textAnimationDelay = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;

    private CanvasGroup canvasGroup;
    private AudioSource audioSource;

    private void Awake()
    {
        // ซ่อน UI ตอนเริ่มเกม
        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }

        // Setup CanvasGroup สำหรับ Fade
        canvasGroup = deadPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = deadPanel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;

        // Setup Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup Buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    /// <summary>
    /// เรียกฟังก์ชันนี้เมื่อ Player ตาย
    /// </summary>
    public void ShowDeadUI()
    {
        Debug.Log("💀 ShowDeadUI ถูกเรียก!");

        if (deadPanel == null)
        {
            Debug.LogError("❌ Dead Panel เป็น NULL! ลืมลาก Reference?");
            return;
        }

        StartCoroutine(ShowDeadRoutine());
    }

    private IEnumerator ShowDeadRoutine()
    {
        Debug.Log("⏳ ShowDeadRoutine เริ่มต้น");

        // หน่วงเวลาเล็กน้อย
        yield return new WaitForSeconds(0.5f);

        // แสดง Panel
        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
            Debug.Log("✅ Dead Panel เปิดแล้ว");
        }

        // เล่นเสียง
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Freeze เกม
        Time.timeScale = 0f;

        // Fade In
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // แสดงข้อความ
        yield return new WaitForSecondsRealtime(textAnimationDelay);

        if (deadText != null)
        {
            StartCoroutine(AnimateText(deadText));
        }
    }

    /// <summary>
    /// Animate Text แบบทีละตัวอักษร
    /// </summary>
    private IEnumerator AnimateText(TextMeshProUGUI textComponent, float delay = 0f)
    {
        yield return new WaitForSecondsRealtime(delay);

        string fullText = textComponent.text;
        textComponent.text = "";

        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    /// <summary>
    /// ปุ่ม Restart
    /// </summary>
    private void OnRestartClicked()
    {
        Debug.Log("🔄 Restart Level!");

        Time.timeScale = 1f;

        // Restart Scene ปัจจุบัน
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ปุ่ม Main Menu
    /// </summary>
    private void OnMainMenuClicked()
    {
        Debug.Log("🏠 Return to Main Menu");

        Time.timeScale = 1f;

        // โหลด Scene Main Menu (เปลี่ยนชื่อตามของคุณ)
        SceneManager.LoadScene("MainMenu");
        // หรือใช้ index: SceneManager.LoadScene(0);
    }

    /// <summary>
    /// ซ่อน Dead UI
    /// </summary>
    public void HideDeadUI()
    {
        Time.timeScale = 1f;

        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }
    }
}