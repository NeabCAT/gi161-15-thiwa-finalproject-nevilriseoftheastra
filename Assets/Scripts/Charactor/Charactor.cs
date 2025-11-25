using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float attackRange = 1f;

    // Encapsulation: ใช้ Properties แทนการเข้าถึงตัวแปรโดยตรง
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;
    public float AttackPower => attackPower;
    public float AttackRange => attackRange;
    public bool IsDead => CurrentHealth <= 0;

    // Protected Components
    protected Animator Anim { get; private set; }
    protected Rigidbody2D Rb { get; private set; }

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeStats();
    }

    // แยก method ตามหน้าที่ชัดเจน
    private void InitializeComponents()
    {
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }

    private void InitializeStats()
    {
        CurrentHealth = maxHealth;
    }

    // Polymorphism: method ที่สามารถ override ได้
    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamageTaken(amount);

        if (IsDead)
        {
            Die();
        }
    }

    // Template Method Pattern: แยก logic ย่อย
    protected virtual void OnDamageTaken(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {CurrentHealth}/{MaxHealth}");
    }

    public virtual void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealed(amount);
    }

    protected virtual void OnHealed(int amount)
    {
        Debug.Log($"{gameObject.name} healed {amount}. Health: {CurrentHealth}/{MaxHealth}");
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        OnDeath();
        Destroy(gameObject);
    }

    // Hook method สำหรับ subclass
    protected virtual void OnDeath() { }
}