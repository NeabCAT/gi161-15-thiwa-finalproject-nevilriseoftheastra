using UnityEngine;

public class Slime : Enemy
{
    [Header("Slime Settings")]
    [SerializeField] private float chaseSpeed = 3f; // ความเร็วไล่ Player
    [SerializeField] private float attackDistance = 1f; // ระยะโจมตี
    [SerializeField] private float roamChangeDirTime = 2f; // เวลาเปลี่ยนทิศเดิน

    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    protected override void Awake()
    {
        base.Awake();
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        if (!isDead)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (PlayerController.Instance == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

            if (distanceToPlayer <= attackDistance)
            {
                if (enemyPathfinding != null)
                {
                    enemyPathfinding.StopMoving();
                }
            }
            else
            {
                if (enemyPathfinding != null)
                {
                    enemyPathfinding.MoveTo(directionToPlayer);
                }
            }
        }
        else
        {
            Roaming();
        }
    }

    private void Roaming()
    {
        timeRoaming += Time.deltaTime;

        if (enemyPathfinding != null)
        {
            enemyPathfinding.MoveTo(roamPosition);
        }

        if (timeRoaming > roamChangeDirTime)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public override void IsDead()
    {
        base.IsDead();
    }
}