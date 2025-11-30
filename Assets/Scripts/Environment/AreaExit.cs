using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    private float waitToLoadTime = 1f;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private SpriteRenderer doorSprite;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private GameObject lockIcon; // ไอคอนล็อก (ถ้ามี)

    private void Start()
    {
        // ⭐ อัพเดทสีตอนเริ่ม
        UpdateVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            // ⭐ เช็คว่าล็อกหรือไม่
            if (isLocked)
            {
                Debug.Log("🔒 ประตูล็อกอยู่! ต้องกำจัดมอนสเตอร์ก่อน");
                return;
            }

            Debug.Log("🚪 เปิดประตูไปซีนถัดไป: " + sceneToLoad);

            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
            UIFade.Instance.FadeToBlack();
            StartCoroutine(LoadSceneRoutine());
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return new WaitForSeconds(waitToLoadTime);
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// ล็อกประตู
    /// </summary>
    public void LockDoor()
    {
        isLocked = true;
        UpdateVisual();
        Debug.Log("🔒 ประตูถูกล็อก");
    }

    /// <summary>
    /// ปลดล็อกประตู
    /// </summary>
    public void UnlockDoor()
    {
        isLocked = false;
        UpdateVisual();
        Debug.Log("🔓 ประตูถูกปลดล็อก!");
    }

    /// <summary>
    /// อัพเดท Visual Feedback
    /// </summary>
    private void UpdateVisual()
    {
        // เปลี่ยนสี Sprite (ถ้ามี)
        if (doorSprite != null)
        {
            doorSprite.color = isLocked ? lockedColor : unlockedColor;
        }

        // แสดง/ซ่อน ไอคอนล็อก (ถ้ามี)
        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }
    }

    /// <summary>
    /// เช็คสถานะประตู (สำหรับ Debug)
    /// </summary>
    public bool IsLocked()
    {
        return isLocked;
    }

    // ⭐ Debug ใน Inspector
    private void OnDrawGizmos()
    {
        Gizmos.color = isLocked ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>()?.size ?? Vector2.one);
    }
}