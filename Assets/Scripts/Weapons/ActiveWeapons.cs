using System.Collections;
using UnityEngine;

public class ActiveWeapons : Singleton<ActiveWeapons>
{
    [Header("Weapon Management")]
    [SerializeField] private GameObject currentWeaponObject;
    [SerializeField] private MonoBehaviour currentActiveWeapon;

    [Header("Attack System")]
    private PlayerControls playerControls;
    private float timeBetweenAttacks = 0.5f;
    private bool attackButtonDown = false;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }

    private void Update()
    {
        Attack();
    }

    // ⭐ รับ WeaponInfo มาด้วยตอนเรียก SetWeapon
    public void NewWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo == null || weaponInfo.weaponPrefab == null)
        {
            Debug.LogError("❌ [ActiveWeapons] WeaponInfo หรือ weaponPrefab เป็น null!");
            return;
        }

        // สร้าง weapon จาก prefab
        GameObject weaponObject = Instantiate(weaponInfo.weaponPrefab, transform.position, transform.rotation);
        weaponObject.transform.parent = transform;

        // ตั้งค่าอาวุธ
        SetWeaponInternal(weaponObject, weaponInfo);
    }

    // ⭐ ฟังก์ชันสำหรับโค้ดเก่าที่ส่ง GameObject มา (ไม่แนะนำ แต่รองรับเพื่อ backward compatibility)
    public void SetWeapon(GameObject weapon)
    {
        // พยายามหา WeaponInfo จากอาวุธเอง
        IWeapon weaponInterface = weapon.GetComponent<IWeapon>();
        WeaponInfo info = weaponInterface?.GetWeaponInfo();

        if (info == null)
        {
            Debug.LogWarning("⚠️ [ActiveWeapons] ไม่พบ WeaponInfo! จะใช้ค่า default cooldown = 0.5s");
        }

        SetWeaponInternal(weapon, info);
    }

    private void SetWeaponInternal(GameObject weapon, WeaponInfo weaponInfo)
    {
        ClearWeapon();
        currentWeaponObject = weapon;
        currentActiveWeapon = weapon.GetComponent<IWeapon>() as MonoBehaviour;

        if (currentActiveWeapon == null)
        {
            Debug.LogWarning($"⚠️ อาวุธ {weapon.name} ไม่มี IWeapon component!");
            return;
        }

        // ⭐ ส่ง WeaponInfo ไปให้อาวุธ
        IWeapon weaponInterface = currentActiveWeapon as IWeapon;

        // ถ้าอาวุธมีฟังก์ชัน SetWeaponInfo ให้เรียกมัน
        if (currentActiveWeapon is Sword sword)
        {
            sword.SetWeaponInfo(weaponInfo);
        }
        // สามารถเพิ่มอาวุธอื่นๆ ได้ เช่น
        // else if (currentActiveWeapon is Bow bow)
        // {
        //     bow.SetWeaponInfo(weaponInfo);
        // }

        // ตั้งค่า cooldown
        if (weaponInfo != null)
        {
            timeBetweenAttacks = weaponInfo.weaponCooldown;
            Debug.Log($"⚔️ ติดอาวุธ: {weapon.name} | Cooldown: {timeBetweenAttacks}s");
        }
        else
        {
            timeBetweenAttacks = 0.5f;
            Debug.LogWarning($"⚠️ [{weapon.name}] ไม่พบ WeaponInfo! ใช้ cooldown = 0.5s");
        }
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeaponObject;
    }

    public MonoBehaviour GetCurrentActiveWeapon()
    {
        return currentActiveWeapon;
    }

    public void ClearWeapon()
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            currentActiveWeapon = null;
            timeBetweenAttacks = 0.5f;
            Debug.Log("🗑️ ลบอาวุธแล้ว");
        }
    }

    public void ToggleIsAttacking(bool value)
    {
        isAttacking = value;
    }

    private void AttackCooldown()
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine()
    {
        Debug.Log($"⏱️ [ActiveWeapons] รอ cooldown {timeBetweenAttacks}s...");
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
        Debug.Log("✅ [ActiveWeapons] Cooldown เสร็จแล้ว!");
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking && currentActiveWeapon != null)
        {
            AttackCooldown();
            (currentActiveWeapon as IWeapon).Attack();
        }
    }
}