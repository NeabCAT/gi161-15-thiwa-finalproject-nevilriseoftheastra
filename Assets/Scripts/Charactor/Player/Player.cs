using UnityEngine;

public class Player : Character
{
    [Header("Player Specific")]
    [SerializeField] private ClassType selectedClass = ClassType.None;
    [SerializeField] private int mana = 100;

    [Header("Weapon Holder")]
    [SerializeField] private Transform weaponHolder;

    [Header("Class Prefabs")]
    [SerializeField] private GameObject strikerPrefab;
    [SerializeField] private GameObject arcanistPrefab;
    [SerializeField] private GameObject astraCharmPrefab;

    private BaseClass currentClassInstance;
    private int maxMana;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        maxMana = mana;
        playerController = GetComponent<PlayerController>();

        if (weaponHolder == null)
        {
            Debug.LogError("❌ [Player] Weapon Holder ยังไม่ได้ลากใส่!");
        }
    }

    public void SelectClass(ClassType classType)
    {
        Debug.Log($"🎯 [Player] กำลังเลือกคลาส: {classType}");

        selectedClass = classType;

        // ลบคลาสเก่า
        if (currentClassInstance != null)
        {
            Destroy(currentClassInstance.gameObject);
        }

        if (weaponHolder == null)
        {
            Debug.LogError("❌ [Player] ไม่มี Weapon Holder!");
            return;
        }

        // ⭐ สร้างคลาสจาก Prefab
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
            // Instantiate จาก Prefab
            GameObject classObject = Instantiate(classPrefab, transform);
            currentClassInstance = classObject.GetComponent<BaseClass>();

            if (currentClassInstance != null)
            {
                Debug.Log($"✅ [Player] สร้างคลาส {classType} จาก Prefab!");
                currentClassInstance.Initialize(this, weaponHolder);
            }
            else
            {
                Debug.LogError($"❌ [Player] Prefab ไม่มี BaseClass component!");
            }
        }
        else
        {
            Debug.LogError($"❌ [Player] ไม่มี Prefab สำหรับคลาส {classType}!");
        }
    }

    public void UseSkill()
    {
        if (currentClassInstance != null)
        {
            currentClassInstance.UseSkill();
        }
    }

    public void Attack()
    {
        if (currentClassInstance != null)
        {
            currentClassInstance.Attack();
        }
    }

    public void Dodge()
    {
        Debug.Log("🏃 [Player] Dodge!");
    }

    public void OnHitWithEnemy()
    {
        Debug.Log("⚔️ [Player] ชนกับศัตรู!");
        TakeDamage(10);
    }

    public void Shoot()
    {
        Debug.Log("🎯 [Player] ยิง!");
    }

    public override void IsDead()
    {
        Debug.Log("💀 [Player] ตาย!");
        gameObject.SetActive(false);
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