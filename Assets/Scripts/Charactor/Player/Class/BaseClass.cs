using UnityEngine;

public abstract class BaseClass : MonoBehaviour
{
    protected Player player;
    protected GameObject weaponInstance;
    protected ActiveWeapons activeWeaponsHolder;

    [Header("Class Weapon")]
    [SerializeField] protected GameObject weaponPrefab;

    public virtual void Initialize(Player ownerPlayer, Transform weaponTransform)
    {
        player = ownerPlayer;

        // หา ActiveWeapons component
        activeWeaponsHolder = weaponTransform.GetComponent<ActiveWeapons>();

        if (activeWeaponsHolder == null)
        {
            Debug.LogError("❌ [BaseClass] ไม่พบ ActiveWeapons component!");
            return;
        }

        // ลบอาวุธเก่า
        activeWeaponsHolder.ClearWeapon();

        // สร้างอาวุธใหม่
        if (weaponPrefab != null)
        {
            weaponInstance = Instantiate(weaponPrefab, activeWeaponsHolder.transform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;

            // บอก ActiveWeapons ว่าอาวุธคืออะไร
            activeWeaponsHolder.SetWeapon(weaponInstance);

            Debug.Log($"✅ [BaseClass] สร้างอาวุธ: {weaponPrefab.name}");
        }
        else
        {
            Debug.LogError("❌ [BaseClass] ไม่มี Weapon Prefab ใน Inspector!");
        }

        ApplyClassStats();
    }

    protected abstract void ApplyClassStats();
    public abstract void UseSkill();
    public abstract void Attack();

    protected virtual void OnDestroy()
    {
        if (activeWeaponsHolder != null)
        {
            activeWeaponsHolder.ClearWeapon();
        }
    }
}