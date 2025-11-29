using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected int maxHealth = 2;
    protected int health;

    // Properties
    public int Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            health = Mathf.Min(health, maxHealth); // ⭐ ปรับ health ถ้าเกิน maxHealth ใหม่
        }
    }

    protected virtual void Awake()
    {
        health = maxHealth; // ⭐ เพิ่มบรรทัดนี้ให้ health เริ่มต้นเท่ากับ maxHealth
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {health}/{maxHealth}");

        if (health <= 0)
        {
            IsDead();
        }
    }

    public virtual void IsDead()
    {
        Debug.Log($"{gameObject.name} is dead!");
        Destroy(gameObject);
    }

}