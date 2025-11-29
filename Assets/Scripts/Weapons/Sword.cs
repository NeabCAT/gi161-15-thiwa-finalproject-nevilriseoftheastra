using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private string slashAnimSpawnPointName = "SlashAnimationSpawnPoints";
    [SerializeField] private string weaponColliderName = "WeaponCollider";
    [SerializeField] private WeaponInfo weaponInfo;

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

        if (weaponInfo != null)
        {
            Debug.Log($"✅ [Sword] มี WeaponInfo | Damage: {weaponInfo.weaponDamage}, Cooldown: {weaponInfo.weaponCooldown}s");
        }
        else
        {
            Debug.LogError("❌ [Sword] ไม่มี WeaponInfo! กรุณา assign ใน Inspector");
        }
    }

    private void Update()
    {
        MouseFollowWithOffset();
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

        // ⭐ เพิ่ม Fallback: ถ้า Animation Event ไม่ทำงาน จะปิด Collider เองหลัง 0.5 วินาที
        StartCoroutine(AttackFallback());
    }

    // ⭐ Coroutine สำรอง
    private IEnumerator AttackFallback()
    {
        yield return new WaitForSeconds(0.5f);

        // ถ้า Collider ยังเปิดอยู่ ให้ปิดเอง
        if (weaponCollider != null && weaponCollider.gameObject.activeSelf)
        {
            Debug.LogWarning("⚠️ [Sword] Animation Event ไม่ทำงาน! ปิด Collider ด้วย Fallback");
            weaponCollider.gameObject.SetActive(false);
        }
    }

    public void DoneAttackingAnimEvent()
    {
        Debug.Log("✅ [Sword] DoneAttackingAnimEvent เรียกแล้ว!");

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
