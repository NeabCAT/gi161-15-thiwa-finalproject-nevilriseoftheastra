using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private string slashAnimSpawnPointName = "SlashAnimationSpawnPoints";
    [SerializeField] private string weaponColliderName = "WeaponCollider";

    // ⭐ ไม่ต้องมี [SerializeField] เพราะจะถูกส่งมาจาก ActiveWeapons
    private WeaponInfo weaponInfo;

    private Transform slashAnimSpawnPoint;
    private Transform weaponCollider;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapons activeWeapon;
    private GameObject slashAnim;

    private void Awake()
    {
        if (slashAnimSpawnPoint == null && transform.parent != null)
        {
            slashAnimSpawnPoint = transform.parent.Find("SlashAnimationSpawnPoints");

            if (slashAnimSpawnPoint == null && transform.parent.parent != null)
            {
                Transform playerTransform = transform.parent.parent;
                slashAnimSpawnPoint = playerTransform.Find("SlashAnimationSpawnPoints");
            }

            if (slashAnimSpawnPoint != null)
            {
                Debug.Log($"✅ [Sword] พบ SlashAnimationSpawnPoints ที่: {slashAnimSpawnPoint.parent.name}");
            }
            else
            {
                Debug.LogError("❌ [Sword] ไม่พบ SlashAnimationSpawnPoints!");
            }
        }

        if (weaponCollider == null && transform.parent != null)
        {
            weaponCollider = transform.parent.Find("WeaponCollider");

            if (weaponCollider == null && transform.parent.parent != null)
            {
                Transform playerTransform = transform.parent.parent;
                weaponCollider = playerTransform.Find("WeaponCollider");
            }

            if (weaponCollider != null)
            {
                Debug.Log($"✅ [Sword] พบ WeaponCollider ที่: {weaponCollider.parent.name}");
            }
            else
            {
                Debug.LogError("❌ [Sword] ไม่พบ WeaponCollider!");
            }
        }

        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapons>();
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    // ⭐ ฟังก์ชันใหม่: ให้ ActiveWeapons เรียกเพื่อส่ง WeaponInfo มาให้
    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo = info;
        if (weaponInfo != null)
        {
            Debug.Log($"✅ [Sword] ได้รับ WeaponInfo | Cooldown: {weaponInfo.weaponCooldown}s");
        }
        else
        {
            Debug.LogWarning("⚠️ [Sword] ไม่มี WeaponInfo!");
        }
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack()
    {
        Debug.Log("⚔️ [Sword] Attack!");

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Attack");
        }

        if (weaponCollider != null)
        {
            weaponCollider.gameObject.SetActive(true);
        }

        if (slashAnimPrefab != null && slashAnimSpawnPoint != null)
        {
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
        }
    }

    public void DoneAttackingAnimEvent()
    {
        if (weaponCollider != null)
        {
            weaponCollider.gameObject.SetActive(false);
        }
    }

    public void SwingUpFlipAnimEvent()
    {
        if (slashAnim != null && playerController != null)
        {
            slashAnim.transform.rotation = Quaternion.Euler(-180, 0, 0);
            if (playerController.FacingLeft)
            {
                slashAnim.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        if (slashAnim != null && playerController != null)
        {
            slashAnim.transform.rotation = Quaternion.Euler(0, 0, 0);
            if (playerController.FacingLeft)
            {
                slashAnim.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    private void MouseFollowWithOffset()
    {
        if (playerController == null || activeWeapon == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
            if (weaponCollider != null)
            {
                weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (weaponCollider != null)
            {
                weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}