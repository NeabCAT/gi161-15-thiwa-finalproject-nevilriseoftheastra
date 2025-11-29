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

    public void NewWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo == null || weaponInfo.weaponPrefab == null)
        {
            Debug.LogError("❌ [ActiveWeapons] WeaponInfo หรือ weaponPrefab เป็น null!");
            return;
        }

        GameObject weaponObject = Instantiate(weaponInfo.weaponPrefab, transform.position, transform.rotation);
        weaponObject.transform.parent = transform;

        SetWeaponInternal(weaponObject);
    }

    public void SetWeapon(GameObject weapon)
    {
        SetWeaponInternal(weapon);
    }

    private void SetWeaponInternal(GameObject weapon)
    {
        ClearWeapon();
        currentWeaponObject = weapon;
        currentActiveWeapon = weapon.GetComponent<IWeapon>() as MonoBehaviour;

        if (currentActiveWeapon == null)
        {
            Debug.LogWarning($"⚠️ อาวุธ {weapon.name} ไม่มี IWeapon component!");
            return;
        }

        // ⭐ ดึง WeaponInfo จากอาวุธโดยตรง
        IWeapon weaponInterface = currentActiveWeapon as IWeapon;
        WeaponInfo info = weaponInterface.GetWeaponInfo();

        // ตั้งค่า cooldown
        if (info != null)
        {
            timeBetweenAttacks = info.weaponCooldown;
            Debug.Log($"⚔️ ติดอาวุธ: {weapon.name} | Damage: {info.weaponDamage}, Cooldown: {timeBetweenAttacks}s");
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
        if (attackButtonDown && !isAttacking && currentActiveWeapon)
        {
            AttackCooldown();
            (currentActiveWeapon as IWeapon).Attack();
        }
    }
}