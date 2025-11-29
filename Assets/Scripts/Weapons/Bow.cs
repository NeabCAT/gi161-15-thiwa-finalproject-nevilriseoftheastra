using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        myAnimator.SetTrigger(FIRE_HASH);

        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapons.Instance.transform.rotation);
        Projectile projectile = newArrow.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.UpdateProjectileRange(weaponInfo.weaponRange);
            projectile.SetDamage(weaponInfo.weaponDamage);
        }
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}