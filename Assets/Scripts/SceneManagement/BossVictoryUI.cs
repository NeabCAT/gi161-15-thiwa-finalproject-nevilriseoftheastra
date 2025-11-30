using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI แสดงเมื่อชนะบอส
/// </summary>
public class BossVictoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float textAnimationDelay = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip victorySound;

    private CanvasGroup canvasGroup;
    private AudioSource audioSource;

    private void Awake()
    {
        // ซ่อน UI ตอนเริ่มเกม
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        // Setup CanvasGroup สำหรับ Fade
        canvasGroup = victoryPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = victoryPanel.AddComponent<CanvasGroup>();
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
    /// เรียกฟังก์ชันนี้เมื่อบอสตาย
    /// </summary>
    public void ShowVictory(string bossName = "Boss")
    {
        Debug.Log($"?? ShowVictory ถูกเรียก! Boss: {bossName}");

        if (victoryPanel == null)
        {
            Debug.LogError("? Victory Panel เป็น NULL! ลืมลาก Reference?");
            return;
        }

        StartCoroutine(ShowVictoryRoutine(bossName));
    }

    private IEnumerator ShowVictoryRoutine(string bossName)
    {
        Debug.Log("? ShowVictoryRoutine เริ่มต้น");

        // หน่วงเวลาเล็กน้อย
        yield return new WaitForSeconds(0.5f);

        // แสดง Panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("? Victory Panel เปิดแล้ว");
        }
        else
        {
            Debug.LogError("? Victory Panel เป็น NULL!");
            yield break;
        }

        // เล่นเสียง
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // Freeze เกม (ถ้าต้องการ)
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

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
            StartCoroutine(AnimateText(bossNameText));
        }

        if (victoryText != null)
        {
            StartCoroutine(AnimateText(victoryText, 0.3f));
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
        Time.timeScale = 1f;

        Debug.Log("Restart Level!");

        // Restart Scene ปัจจุบัน
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ปุ่ม Main Menu
    /// </summary>
    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;

        Debug.Log("Return to Main Menu");

        // โหลด Scene Main Menu (เปลี่ยนชื่อตามของคุณ)
        SceneManager.LoadScene("MainMenu");
        // หรือใช้ index: SceneManager.LoadScene(0);
    }

    /// <summary>
    /// ซ่อน Victory UI
    /// </summary>
    public void HideVictory()
    {
        Time.timeScale = 1f;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }
}