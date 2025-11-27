using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private string slashAnimSpawnPointName = "SlashAnimationSpawnPoints";
    [SerializeField] private string weaponColliderName = "WeaponCollider";
    [SerializeField] private float swordAttackCD = .5f;

    private Transform slashAnimSpawnPoint;
    private Transform weaponCollider;
    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapons activeWeapon;
    private bool attackButtonDown, isAttacking = false;
    private GameObject slashAnim;

    private void Awake()
    {
        // ⭐ หา slashAnimSpawnPoint (ลองทั้ง 2 แบบ)
        if (slashAnimSpawnPoint == null && transform.parent != null)
        {
            Debug.Log("🔍 [Sword] กำลังหา SlashAnimationSpawnPoints...");

            // ลองหาจาก Parent ก่อน (1 ชั้น)
            slashAnimSpawnPoint = transform.parent.Find("SlashAnimationSpawnPoints");

            // ถ้าไม่เจอ ลองหาจาก Parent ของ Parent (2 ชั้น)
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
                Debug.LogError("❌ [Sword] ไม่พบ SlashAnimationSpawnPoints ทั้ง 2 แบบ!");
            }
        }

        // ⭐ หา weaponCollider (ลองทั้ง 2 แบบ)
        if (weaponCollider == null && transform.parent != null)
        {
            Debug.Log("🔍 [Sword] กำลังหา WeaponCollider...");

            // ลองหาจาก Parent ก่อน (1 ชั้น)
            weaponCollider = transform.parent.Find("WeaponCollider");

            // ถ้าไม่เจอ ลองหาจาก Parent ของ Parent (2 ชั้น)
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
                Debug.LogError("❌ [Sword] ไม่พบ WeaponCollider ทั้ง 2 แบบ!");
            }
        }

        // ส่วนที่เหลือเหมือนเดิม...
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapons>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
            Debug.Log("✅ [Sword] PlayerControls Enabled!");
        }
    }

    void Start()
    {
        if (playerControls != null)
        {
            playerControls.Combat.Attack.started += _ => StartAttacking();
            playerControls.Combat.Attack.canceled += _ => StopAttacking();
            Debug.Log("✅ [Sword] ผูก Attack Input สำเร็จ!");
        }
        else
        {
            Debug.LogError("❌ [Sword] PlayerControls เป็น null!");
        }
    }

    private void Update()
    {
        MouseFollowWithOffset();
        Attack();
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
        Debug.Log("🎮 [Sword] Start Attacking!");
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
        Debug.Log("🎮 [Sword] Stop Attacking!");
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking)
        {
            Debug.Log("⚔️ [Sword] Attack!");
            isAttacking = true;

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

            StartCoroutine(AttackCDRoutine());
        }
    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        isAttacking = false;
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

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Combat.Attack.started -= _ => StartAttacking();
            playerControls.Combat.Attack.canceled -= _ => StopAttacking();
            playerControls.Disable();
        }
    }
}