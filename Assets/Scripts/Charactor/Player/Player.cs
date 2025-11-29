using System.Collections;
using UnityEngine;

public class Player : Character
{
    // ⭐ เพิ่ม Singleton Pattern
    public static Player Instance { get; private set; }

    [Header("Player Specific")]
    [SerializeField] private ClassType selectedClass = ClassType.None;
    [SerializeField] private int mana = 100;

    [Header("Combat Settings")]
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    [Header("Weapon Holder")]
    [SerializeField] private Transform weaponHolder;

    [Header("Class Prefabs")]
    [SerializeField] private GameObject strikerPrefab;
    [SerializeField] private GameObject arcanistPrefab;
    [SerializeField] private GameObject astraCharmPrefab;

    private BaseClass currentClassInstance;
    private int maxMana;
    private PlayerController playerController;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    protected override void Awake()
    {
        // ⭐ Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        base.Awake();
        maxMana = mana;
        playerController = GetComponent<PlayerController>();
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    public void SelectClass(ClassType classType)
    {
        selectedClass = classType;

        // ลบคลาสเก่า
        if (currentClassInstance != null)
        {
            Destroy(currentClassInstance.gameObject);
        }

        if (weaponHolder == null)
        {
            return;
        }

        // เลือก Prefab ตามอาชีพ
        GameObject classPrefab = null;
        switch (classType)
        {
            case ClassType.Striker:
                classPrefab = strikerPrefab;
                break;
            case ClassType.Arcanist:
                classPrefab = arcanistPrefab;
                break;
            case ClassType.AstraCharm:
                classPrefab = astraCharmPrefab;
                break;
        }

        // สร้างคลาสใหม่
        if (classPrefab != null)
        {
            GameObject classObject = Instantiate(classPrefab, transform);
            currentClassInstance = classObject.GetComponent<BaseClass>();

            if (currentClassInstance != null)
            {
                currentClassInstance.Initialize(this, weaponHolder);
                Debug.Log($"เลือกอาชีพ {classType} | HP: {health}/{maxHealth}");
            }
        }
    }

    public void UseSkill()
    {
        if (currentClassInstance != null)
        {
            currentClassInstance.UseSkill();
        }
        else
        {
            Debug.LogWarning("ยังไม่ได้เลือกอาชีพ!");
        }
    }

    public void Attack()
    {
        if (currentClassInstance != null)
        {
            currentClassInstance.Attack();
        }
        else
        {
            Debug.LogWarning("ยังไม่ได้เลือกอาชีพ!");
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        ScreenShakeManager.Instance.ShakeScreen();
        base.TakeDamage(damage);
        StartCoroutine(DamageRecoveryRoutine());
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy && canTakeDamage)
        {
            TakeDamage(1);

            if (knockback != null)
            {
                knockback.GetKnockedBack(other.gameObject.transform, knockBackThrustAmount);
            }

            if (flash != null)
            {
                StartCoroutine(flash.FlashRoutine());
            }
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    public override void IsDead()
    {
        Debug.Log("💀 Player ตาย!");
        gameObject.SetActive(false);
    }

    // ⭐ ปรับปรุง HealPlayer ให้ดีขึ้น
    public void HealPlayer(int amount = 1)
    {
        int oldHealth = health;
        health = Mathf.Min(health + amount, maxHealth);
        int actualHealed = health - oldHealth;

        if (actualHealed > 0)
        {
            Debug.Log($"💚 Healed +{actualHealed} HP | Current: {health}/{maxHealth}");
        }
        else
        {
            Debug.Log("❤️ HP เต็มอยู่แล้ว!");
        }
    }

    // Properties
    public int Mana
    {
        get { return mana; }
        set { mana = Mathf.Clamp(value, 0, maxMana); }
    }

    public ClassType SelectedClass
    {
        get { return selectedClass; }
    }
}