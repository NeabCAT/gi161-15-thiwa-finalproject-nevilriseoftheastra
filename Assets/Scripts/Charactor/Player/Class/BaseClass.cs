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

        activeWeaponsHolder = weaponTransform.GetComponent<ActiveWeapons>();

        if (activeWeaponsHolder == null)
        {
            return;
        }

        activeWeaponsHolder.ClearWeapon();

        if (weaponPrefab != null)
        {
            weaponInstance = Instantiate(weaponPrefab, activeWeaponsHolder.transform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;

            activeWeaponsHolder.SetWeapon(weaponInstance);

            Debug.Log($"✅ [BaseClass] สร้างอาวุธ: {weaponPrefab.name}");
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