using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
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

    [Header("Dead UI")]
    [SerializeField] private PlayerDeadUI playerDeadUI; // ⭐ เพิ่มนี้

    private BaseClass currentClassInstance;
    private int maxMana;
    private PlayerController playerController;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Slider healthSlider;
    private Animator animator;
    private bool isDead = false;

    private void Start()
    {
        UpdateHealthSlider();
    }

    protected override void Awake()
    {
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
        animator = GetComponent<Animator>();
    }

    public void SelectClass(ClassType classType)
    {
        selectedClass = classType;

        if (currentClassInstance != null)
        {
            Destroy(currentClassInstance.gameObject);
        }

        if (weaponHolder == null)
        {
            return;
        }

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
        if (!canTakeDamage || isDead) return;

        ScreenShakeManager.Instance.ShakeScreen();
        base.TakeDamage(damage);

        UpdateHealthSlider();

        StartCoroutine(DamageRecoveryRoutine());
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (isDead) return;

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
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player ตาย!");

        // ปิดการเคลื่อนที่
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // ปิด Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // เล่น Death Animation แล้วแสดง UI
        StartCoroutine(DeathAnimationRoutine());
    }

    private IEnumerator DeathAnimationRoutine()
    {
        // ⭐ ใช้ Animator Trigger
        if (animator != null)
        {
            animator.SetTrigger("Die");

            // รอให้ Animation เล่นจบ
            yield return new WaitForSeconds(1f);
        }
        else
        {
            // Fallback: ใช้ Code Animation
            float duration = 1f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Color startColor = sprite != null ? sprite.color : Color.white;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                transform.Rotate(0, 0, 360f * Time.deltaTime);

                if (sprite != null)
                {
                    Color newColor = startColor;
                    newColor.a = Mathf.Lerp(1f, 0f, t);
                    sprite.color = newColor;
                }

                yield return null;
            }
        }

        Debug.Log("🎬 Death Animation จบแล้ว");

        // ⭐ แสดง Dead UI
        if (playerDeadUI != null)
        {
            playerDeadUI.ShowDeadUI();
            Debug.Log("✅ เรียก Dead UI สำเร็จ");
        }
        else
        {
            Debug.LogError("❌ Player Dead UI เป็น NULL! ลืมลาก Reference?");
        }

        // ⭐ ไม่ต้อง SetActive(false) เพราะจะโชว์ UI
        // gameObject.SetActive(false);
    }

    public void HealPlayer(int amount = 1)
    {
        if (isDead) return;

        int oldHealth = health;
        health = Mathf.Min(health + amount, maxHealth);
        int actualHealed = health - oldHealth;

        if (actualHealed > 0)
        {
            Debug.Log($"💚 Healed +{actualHealed} HP | Current: {health}/{maxHealth}");
            UpdateHealthSlider();
        }
        else
        {
            Debug.Log("❤️ HP เต็มอยู่แล้ว!");
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("Health Slider").GetComponent<Slider>();
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

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