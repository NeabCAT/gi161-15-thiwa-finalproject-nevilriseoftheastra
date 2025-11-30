using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


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
    private void OnRestartClicked()
    {
        Debug.Log("🔄 Restart Level!");

        // รีเซ็ต Time.timeScale
        Time.timeScale = 1f;

        // ปิด UI ก่อน
        HideDeadUI();

        // ล้าง Class และอาวุธก่อนโหลดซีน
        if (Player.Instance != null)
        {
            Player.Instance.ResetPlayer();
        }

        // โหลดซีนใหม่และรอให้โหลดเสร็จ
        StartCoroutine(RestartSceneRoutine());
    }

    private IEnumerator RestartSceneRoutine()
    {
        // โหลดซีน
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene_1");

        // รอให้โหลดเสร็จ
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // รอ 1 frame ให้ทุกอย่างพร้อม
        yield return new WaitForEndOfFrame();

        // ⭐ Reset กล้องหลังซีนโหลดเสร็จ
        if (CameraController.Instance != null)
        {
            CameraController.Instance.RefreshCamera();
        }

        // ⭐ Reset EnemyManager (ถ้ามี)
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.ManualReset();
            Debug.Log("✅ Reset EnemyManager");
        }

        Debug.Log("✅ Restart สำเร็จ!");
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("🏠 Return to Main Menu");

        // รีเซ็ต Time.timeScale
        Time.timeScale = 1f;

        // ⭐ ปิด UI ก่อน
        HideDeadUI();

        // ⭐ รีเซ็ต Player ก่อน
        if (Player.Instance != null)
        {
            Player.Instance.ResetPlayer();
        }

        // กลับไปหน้าเมนู (เปลี่ยนชื่อตามซีนของคุณ)
        SceneManager.LoadScene("MainMenu");
    }

    public void HideDeadUI()
    {
        Time.timeScale = 1f;

        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }
    }
}