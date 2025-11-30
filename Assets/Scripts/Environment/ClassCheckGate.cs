using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCheckGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private GameObject gateCollider; 
    [SerializeField] private SpriteRenderer gateVisual; 

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(1f, 0f, 0f, 0.5f); 
    [SerializeField] private Color unlockedColor = new Color(0f, 1f, 0f, 0.5f); 
    [SerializeField] private GameObject lockIcon; 

    [Header("Warning Settings")]
    [SerializeField] private GameObject warningText; 
    [SerializeField] private float warningDuration = 2f;

    private Collider2D gateCol;
    private Coroutine warningCoroutine;

    private void Start()
    {
        if (gateCollider == null)
        {
            gateCollider = gameObject;
        }

        gateCol = gateCollider.GetComponent<Collider2D>();

        if (gateCol == null)
        {
            Debug.LogError("❌ ไม่พบ Collider2D! ต้องมี Collider เพื่อบล็อกผู้เล่น");
        }

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
                gateCol.enabled = false;
                UpdateVisuals(false);
            }
            else
            {
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
        if (gateVisual != null)
        {
            gateVisual.color = isLocked ? lockedColor : unlockedColor;
        }

        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }
    }

    private void OnDrawGizmos()
    {
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