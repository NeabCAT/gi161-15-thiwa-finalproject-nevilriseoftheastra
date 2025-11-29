using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy Settings")]
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    [Header("AI Settings")]
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    private Knockback knockback;
    private Flash flash;
    private EnemyPathfinding enemyPathfinding;

    private bool canAttack = true;
    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    private enum State
    {
        Roaming,
        Attacking
    }
    private State state;

    protected override void Awake()
    {
        base.Awake();

        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        enemyPathfinding = GetComponent<EnemyPathfinding>();

        MaxHealth = startingHealth;
        state = State.Roaming;
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        switch (state)
        {
            case State.Roaming:
                Roaming();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
    }

    private void Roaming()
    {
        timeRoaming += Time.deltaTime;

        if (enemyPathfinding != null)
        {
            enemyPathfinding.MoveTo(roamPosition);
        }

        // เช็คว่า Player อยู่ในระยะโจมตีหรือไม่
        if (PlayerController.Instance != null &&
            Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }

        // เปลี่ยนทิศทาง Roaming
        if (timeRoaming > roamChangeDirFloat)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking()
    {
        // ถ้า Player ออกจากระยะโจมตี → กลับไป Roaming
        if (PlayerController.Instance != null &&
            Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
        {
            state = State.Roaming;
            return;
        }

        // โจมตี Player
        if (attackRange != 0 && canAttack)
        {
            canAttack = false;

            // เรียก Attack() จาก enemyType (เช่น Shooter, Melee)
            if (enemyType != null)
            {
                (enemyType as IEnemy)?.Attack();
            }

            // ควบคุมการเคลื่อนที่ขณะโจมตี
            if (stopMovingWhileAttacking)
            {
                enemyPathfinding?.StopMoving();
            }
            else
            {
                enemyPathfinding?.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (knockback != null && PlayerController.Instance != null)
        {
            knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        }

        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(CheckDetectDeathRoutine());
        }
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (health <= 0)
        {
            IsDead();
        }
    }

    public override void IsDead()
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"💀 [{gameObject.name}] ตาย!");
        Destroy(gameObject);
    }
}