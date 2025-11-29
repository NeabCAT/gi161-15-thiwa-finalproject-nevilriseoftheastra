using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCheckGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private GameObject gateCollider; // Collider ที่จะเปิด/ปิด
    [SerializeField] private SpriteRenderer gateVisual; // Sprite ของประตู (ถ้ามี)

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(1f, 0f, 0f, 0.5f); // แดงโปร่งใส
    [SerializeField] private Color unlockedColor = new Color(0f, 1f, 0f, 0.5f); // เขียวโปร่งใส
    [SerializeField] private GameObject lockIcon; // ไอคอนล็อก (ถ้ามี)

    [Header("Warning Settings")]
    [SerializeField] private GameObject warningText; // ข้อความเตือน (ถ้ามี)
    [SerializeField] private float warningDuration = 2f;

    private Collider2D gateCol;
    private Coroutine warningCoroutine;

    private void Start()
    {
        // หา Collider ถ้าไม่ได้ใส่
        if (gateCollider == null)
        {
            gateCollider = gameObject;
        }

        gateCol = gateCollider.GetComponent<Collider2D>();

        if (gateCol == null)
        {
            Debug.LogError("❌ ไม่พบ Collider2D! ต้องมี Collider เพื่อบล็อกผู้เล่น");
        }

        // ซ่อนข้อความเตือนตอนเริ่ม
        if (warningText != null)
        {
            warningText.SetActive(false);
        }

        CheckPlayerClass();
    }

    private void Update()
    {
        CheckPlayerClass();
    }

    private void CheckPlayerClass()
    {
        if (Player.Instance == null) return;

        bool hasClass = Player.Instance.SelectedClass != ClassType.None;

        if (gateCol != null)
        {
            if (hasClass)
            {
                // มีคลาส → ปิด Collider (ผ่านได้)
                gateCol.enabled = false;
                UpdateVisuals(false);
            }
            else
            {
                // ไม่มีคลาส → เปิด Collider (ผ่านไม่ได้)
                gateCol.enabled = true;
                UpdateVisuals(true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (Player.Instance != null && Player.Instance.SelectedClass == ClassType.None)
            {
                ShowWarning();
            }
        }
    }

    private void ShowWarning()
    {
        Debug.Log("⚠️ ต้องเลือกอาชีพก่อนถึงจะผ่านได้!");

        if (warningText != null)
        {
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }
            warningCoroutine = StartCoroutine(WarningRoutine());
        }
    }

    private IEnumerator WarningRoutine()
    {
        warningText.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningText.SetActive(false);
    }

    private void UpdateVisuals(bool isLocked)
    {
        // เปลี่ยนสี Sprite
        if (gateVisual != null)
        {
            gateVisual.color = isLocked ? lockedColor : unlockedColor;
        }

        // แสดง/ซ่อน ไอคอนล็อก
        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }
    }

    private void OnDrawGizmos()
    {
        // แสดง Gizmo เพื่อดูว่าอยู่ตรงไหน
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (col is BoxCollider2D box)
            {
                Gizmos.DrawCube(transform.position, box.size);
            }
            else if (col is CircleCollider2D circle)
            {
                Gizmos.DrawSphere(transform.position, circle.radius);
            }
        }
    }
}