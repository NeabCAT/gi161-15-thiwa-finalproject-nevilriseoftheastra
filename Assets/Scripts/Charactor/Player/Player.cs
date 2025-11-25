using UnityEngine;

public enum ClassType
{
    Striker,
    Arcanist,
    AstraChanneler
}

public class Player : Character
{
    [Header("Player Specific")]
    [SerializeField] private ClassType selectedClass;
    [SerializeField] private int maxMana = 100;

    // Properties
    public ClassType SelectedClass => selectedClass;
    public int CurrentMana { get; private set; }
    public int MaxMana => maxMana;

    protected override void Awake()
    {
        base.Awake();
        CurrentMana = maxMana;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Attack();

        if (Input.GetKeyDown(KeyCode.E))
            UseSkill();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Dodge();

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    public void SelectClass(ClassType classType)
    {
        selectedClass = classType;
        Debug.Log($"Selected class: {classType}");
    }

    public void Attack()
    {
        if (IsDead) return;

        Debug.Log($"{SelectedClass} attacks!");

        // ใช้ Anim แทน anim (Property จาก Character class)
        if (Anim != null)
            Anim.SetTrigger("Attack");
    }

    public void UseSkill()
    {
        if (IsDead) return;

        if (CurrentMana >= 20)
        {
            CurrentMana -= 20;
            Debug.Log($"{SelectedClass} uses skill! Mana remaining: {CurrentMana}");

            if (Anim != null)
                Anim.SetTrigger("Skill");
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void RestoreMana(int amount)
    {
        CurrentMana = Mathf.Min(maxMana, CurrentMana + amount);
        Debug.Log($"Mana restored! Current: {CurrentMana}/{MaxMana}");
    }

    public void Dodge()
    {
        if (IsDead) return;

        Debug.Log("Player dodges!");

        if (Anim != null)
            Anim.SetTrigger("Dodge");
    }

    public void Shoot()
    {
        if (IsDead) return;

        Debug.Log($"{SelectedClass} shoots!");

        if (Anim != null)
            Anim.SetTrigger("Shoot");
    }

    // Override เมื่อถูกโจมตี
    protected override void OnDamageTaken(int amount)
    {
        base.OnDamageTaken(amount);

        if (Anim != null)
            Anim.SetTrigger("Hit");
    }

    // Trigger Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            HandleItemPickup(collision);
        }

        if (collision.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision);
        }
    }

    private void HandleItemPickup(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            item.OnPickup(this);
        }
    }

    private void HandleEnemyCollision(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("Player hit by enemy!");
            // Enemy จะจัดการ damage ใน OnCollisionEnter2D ของมันเอง
        }
    }
}