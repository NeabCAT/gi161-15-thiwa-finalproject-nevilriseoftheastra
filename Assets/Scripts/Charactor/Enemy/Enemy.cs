using UnityEngine;

// Enemy class ที่สืบทอดมาจาก Character
public class Enemy : Character
{
    [Header("Enemy Specific")]
    [SerializeField] private int damageOnTouch = 10;
    [SerializeField] private float knockbackForce = 5f;

    private EnemyAI ai;
    private EnemyPathfinding pathfinding;

    protected override void Awake()
    {
        base.Awake(); // เรียก parent's Awake ก่อน

        ai = GetComponent<EnemyAI>();
        pathfinding = GetComponent<EnemyPathfinding>();
    }

    // Override การรับดาเมจ
    protected override void OnDamageTaken(int amount)
    {
        base.OnDamageTaken(amount);

        // เพิ่ม behavior เฉพาะ Enemy
        FlashRed();

        // เปลี่ยน state เมื่อถูกโจมตี
        if (ai != null && !IsDead)
        {
            ai.ChangeState(EnemyAI.State.Chasing);
        }
    }

    // Override การตาย
    protected override void OnDeath()
    {
        base.OnDeath();

        // Drop items, play death animation, etc.
        SpawnDeathEffect();
        GivePlayerExperience();
    }

    private void FlashRed()
    {
        // Animation or sprite flash
        if (Anim != null)
        {
            Anim.SetTrigger("Hit");
        }
    }

    private void SpawnDeathEffect()
    {
        Debug.Log($"{gameObject.name} spawned death effect");
        // Instantiate death particles, etc.
    }

    private void GivePlayerExperience()
    {
        Debug.Log("Player gained experience!");
        // Add experience to player
    }

    // Collision detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Character player = collision.gameObject.GetComponent<Character>();
            if (player != null)
            {
                player.TakeDamage(damageOnTouch);
                ApplyKnockback(collision);
            }
        }
    }

    private void ApplyKnockback(Collision2D collision)
    {
        Rigidbody2D targetRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            targetRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}