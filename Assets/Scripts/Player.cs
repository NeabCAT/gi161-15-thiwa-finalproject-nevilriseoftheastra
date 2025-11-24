using UnityEngine;
using static UnityEditor.Progress;

public enum ClassType
{
    Striker,
    Arcanist,
    AstraChanneler
}

public class Player : Character
{
    [Header("Player Specific")]
    public ClassType SelectedClass;
    public int mana = 100;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
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
        SelectedClass = classType;
        Debug.Log($"Selected class: {classType}");
    }

    public void Attack()
    {
        Debug.Log($"{SelectedClass} attacks!");
        if (anim != null)
            anim.SetTrigger("Attack");
    }

    public void UseSkill()
    {
        if (mana >= 20)
        {
            mana -= 20;
            Debug.Log($"{SelectedClass} uses skill! Mana remaining: {mana}");
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void Dodge()
    {
        Debug.Log("Player dodges!");
        if (anim != null)
            anim.SetTrigger("Dodge");
    }

    public void OnHitWithEnemy()
    {
        Debug.Log("Player hit by enemy!");
    }

    public void Shoot()
    {
        Debug.Log($"{SelectedClass} shoots!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                item.OnPickup(this);
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            OnHitWithEnemy();
        }
    }
}