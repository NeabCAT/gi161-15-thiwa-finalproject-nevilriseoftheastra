using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Public enum เพื่อให้ class อื่นเข้าถึงได้
    public enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking
    }

    [Header("AI Settings")]
    [SerializeField] private float roamingInterval = 2f;
    [SerializeField] private float detectionRange = 5f;

    // Properties สำหรับเข้าถึง state
    public State CurrentState { get; private set; }

    // Dependencies
    private EnemyPathfinding pathfinding;
    private Character character;
    private Coroutine currentRoutine;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        character = GetComponent<Character>();

        if (pathfinding == null)
        {
            Debug.LogError($"EnemyPathfinding not found on {gameObject.name}");
        }
    }

    private void Start()
    {
        ChangeState(State.Roaming);
    }

    // PUBLIC Method สำหรับเปลี่ยน state จากภายนอก
    public void ChangeState(State newState)
    {
        if (CurrentState == newState) return;

        ExitState(CurrentState);
        CurrentState = newState;
        EnterState(newState);
    }

    private void ExitState(State state)
    {
        // หยุด coroutine เดิม
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        switch (state)
        {
            case State.Roaming:
                OnExitRoaming();
                break;
            case State.Chasing:
                OnExitChasing();
                break;
        }
    }

    private void EnterState(State state)
    {
        switch (state)
        {
            case State.Idle:
                OnEnterIdle();
                break;
            case State.Roaming:
                OnEnterRoaming();
                break;
            case State.Chasing:
                OnEnterChasing();
                break;
            case State.Attacking:
                OnEnterAttacking();
                break;
        }
    }

    // State Behaviors - แยกเป็น method ชัดเจน
    private void OnEnterIdle()
    {
        if (pathfinding != null)
            pathfinding.Stop();
    }

    private void OnEnterRoaming()
    {
        currentRoutine = StartCoroutine(RoamingRoutine());
    }

    private void OnExitRoaming()
    {
        // Cleanup if needed
    }

    private void OnEnterChasing()
    {
        currentRoutine = StartCoroutine(ChasingRoutine());
    }

    private void OnExitChasing()
    {
        // Cleanup if needed
    }

    private void OnEnterAttacking()
    {
        if (pathfinding != null)
            pathfinding.Stop();
    }

    // Coroutines for each state
    private IEnumerator RoamingRoutine()
    {
        while (CurrentState == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            if (pathfinding != null)
                pathfinding.MoveTo(roamPosition);

            yield return new WaitForSeconds(roamingInterval);
        }
    }

    private IEnumerator ChasingRoutine()
    {
        while (CurrentState == State.Chasing)
        {
            // Logic for chasing player
            // ตัวอย่าง: หา Player และเคลื่อนที่ไปหา
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && pathfinding != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                pathfinding.MoveTo(direction);
            }

            yield return null;
        }
    }

    // Helper Methods
    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // Public API สำหรับ external systems
    public bool IsInRange(Vector2 targetPosition)
    {
        return Vector2.Distance(transform.position, targetPosition) <= detectionRange;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}