using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    protected int health;
    protected int maxHealth;
    protected Animator anim;

    // Properties
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            health = maxHealth; 
        }
    }
    protected virtual void Awake()
    {
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

}