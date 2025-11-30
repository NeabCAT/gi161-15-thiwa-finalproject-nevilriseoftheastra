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
    [SerializeField] protected float attackRange = 5f; // ⭐ เปลี่ยนเป็น protected
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    [Header("Death Animation")]
    [SerializeField] private float deathAnimationDuration = 1f;

    private Knockback knockback;
    private Flash flash;
    protected EnemyPathfinding enemyPathfinding; // ⭐ เปลี่ยนเป็น protected
    protected Animator animator; // ⭐ เปลี่ยนเป็น protected
    protected bool isDead = false; // ⭐ เปลี่ยนเป็น protected

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
        animator = GetComponent<Animator>();

        MaxHealth = startingHealth;
        state = State.Roaming;
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        if (!isDead)
        {
            MovementStateControl();
        }
    }

    // ⭐ เปลี่ยนเป็น protected เพื่อให้ Boss เรียกใช้ได้
    protected void MovementStateControl()
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

        if (PlayerController.Instance != null &&
            Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking()
    {
        if (PlayerController.Instance != null &&
            Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
        {
            state = State.Roaming;
            return;
        }

        if (attackRange != 0 && canAttack)
        {
            canAttack = false;

            if (enemyType != null)
            {
                (enemyType as IEnemy)?.Attack();
            }

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
        if (isDead) return;

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
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 [{gameObject.name}] ตาย!");

        // ปิดการเคลื่อนที่
        if (enemyPathfinding != null)
        {
            enemyPathfinding.StopMoving();
            enemyPathfinding.enabled = false;
        }

        // ปิด Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // เล่น Death Animation
        StartCoroutine(DeathAnimationRoutine());

        // ⭐ บังคับ Destroy (สำรอง) - จะ Destroy หลัง Animation จบแน่นอน
        Destroy(gameObject, deathAnimationDuration + 0.5f);
    }

    /// <summary>
    /// ตรวจสอบว่าตายหรือยัง (สำหรับเรียกจากภายนอก)
    /// </summary>
    public bool IsAlive()
    {
        return !isDead;
    }

    private IEnumerator DeathAnimationRoutine()
    {
        // เล่น Death Animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
            yield return new WaitForSeconds(deathAnimationDuration);
        }
        else
        {
            // Fallback: Code Animation
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Color startColor = sprite != null ? sprite.color : Color.white;

            while (elapsed < deathAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / deathAnimationDuration;

                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                transform.Rotate(0, 0, 360f * Time.deltaTime);

                if (sprite != null)
                {
                    Color newColor = startColor;
                    newColor.a = Mathf.Lerp(1f, 0f, t);
                    sprite.color = newColor;
                }

                yield return null;
            }
        }

        // Spawn VFX
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }

        // Drop Items
        PickUpSpawner pickUpSpawner = GetComponent<PickUpSpawner>();
        if (pickUpSpawner != null)
        {
            pickUpSpawner.DropItems();
        }

        // ⭐ สำคัญ! ทำลาย GameObject ทันที
        Destroy(gameObject);
    }
}