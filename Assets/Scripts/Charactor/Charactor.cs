using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected int health = 100;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float attackPower = 10f;
    [SerializeField] protected float attackRange = 2f;

    protected int maxHealth;
    protected Animator anim;

    protected virtual void Awake()
    {
        maxHealth = health;
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"❤️ [{GetType().Name}] รับดาเมจ {damage} | HP: {health}/{maxHealth}");

        if (health <= 0)
        {
            IsDead();
        }
    }

    public abstract void IsDead();

    // Properties
    public int Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }

    public float AttackSpeed
    {
        get { return attackSpeed; }
        set { attackSpeed = value; }
    }

    public float AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }

    public float AttackRange
    {
        get { return attackRange; }
        set { attackRange = value; }
    }
}