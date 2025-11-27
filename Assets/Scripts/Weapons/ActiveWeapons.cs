using UnityEngine;

public class ActiveWeapons : MonoBehaviour
{
    [Header("Current Weapon")]
    [SerializeField] private GameObject currentWeapon;

    public void SetWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        Debug.Log($"⚔️ [ActiveWeapons] ติดอาวุธ: {weapon.name}");
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void ClearWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
            Debug.Log("🗑️ [ActiveWeapons] ลบอาวุธ");
        }
    }
}