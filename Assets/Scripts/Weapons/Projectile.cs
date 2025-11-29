using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackThrust = 10f; // ⭐ เพิ่ม

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }


    public void SetDamage(int damageAmount)
    {
        this.damage = damageAmount;
    }

    public void SetKnockback(float knockbackAmount)
    {
        this.knockbackThrust = knockbackAmount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        Player player = other.gameObject.GetComponent<Player>();
        Indestructible indestructible = other.gameObject.GetComponent<Indestructible>();

        bool hitSomething = false;

        // ⭐ กระสุนศัตรูชน Player
        if (isEnemyProjectile && player != null)
        {
            player.TakeDamage(damage);

            // ⭐ Knockback Player
            Knockback knockback = player.GetComponent<Knockback>();
            if (knockback != null)
            {
                knockback.GetKnockedBack(transform, knockbackThrust);
            }

            hitSomething = true;
        }
        // กระสุน Player ชน Enemy
        else if (!isEnemyProjectile && enemy != null)
        {
            enemy.TakeDamage(damage);
            hitSomething = true;
        }
        // ชนกับสิ่งที่ทำลายไม่ได้
        else if (indestructible != null)
        {
            hitSomething = true;
        }

        // ถ้าชนอะไรสักอย่าง
        if (hitSomething)
        {
            if (particleOnHitPrefabVFX != null)
            {
                Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}