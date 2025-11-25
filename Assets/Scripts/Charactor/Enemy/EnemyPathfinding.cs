using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    // Encapsulation
    public float MoveSpeed => moveSpeed;
    public Vector2 CurrentDirection => moveDir;
    public bool IsMoving => moveDir.magnitude > 0.01f;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D not found on {gameObject.name}");
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // Single Responsibility: แยก logic การเคลื่อนที่
    private void HandleMovement()
    {
        if (rb == null) return;

        Vector2 newPosition = rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    // Public API
    public void MoveTo(Vector2 direction)
    {
        // Normalize เพื่อให้ movement สม่ำเสมอ
        moveDir = direction.normalized;
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - rb.position).normalized;
        MoveTo(direction);
    }

    public void Stop()
    {
        moveDir = Vector2.zero;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0, newSpeed);
    }

    // Helper method สำหรับการคำนวณ
    public float GetDistanceToPosition(Vector2 targetPosition)
    {
        return Vector2.Distance(rb.position, targetPosition);
    }

    public bool HasReachedPosition(Vector2 targetPosition, float threshold = 0.1f)
    {
        return GetDistanceToPosition(targetPosition) <= threshold;
    }

    // Visualization
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !IsMoving) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDir * 2f);
    }
}