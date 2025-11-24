using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    public int Health = 100;
    public float MoveSpeed = 5f;
    public float AttackSpeed = 1f;
    public float AttackPower = 10f;
    public float AttackRange = 1f;

    [Header("Components")]
    protected Animator anim;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {Health}");

        if (IsDead())
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}