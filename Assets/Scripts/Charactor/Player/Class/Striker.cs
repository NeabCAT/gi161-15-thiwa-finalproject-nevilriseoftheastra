using UnityEngine;

public class Striker : BaseClass
{
    [Header("Striker Settings")]
    [SerializeField] private float critChance = 0.15f;

    private Sword swordComponent;

    public override void Initialize(Player ownerPlayer, Transform weaponTransform)
    {
        base.Initialize(ownerPlayer, weaponTransform);

        // หา Sword component จากอาวุธที่สร้าง
        if (weaponInstance != null)
        {
            swordComponent = weaponInstance.GetComponent<Sword>();
            if (swordComponent != null)
            {
                Debug.Log("⚔️ [Striker] พบ Sword component!");
            }
            else
            {
                Debug.LogWarning("⚠️ [Striker] ไม่พบ Sword component ใน Weapon Prefab!");
            }
        }
    }

    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.Health = 150;
            player.AttackPower = 25f;
            player.AttackSpeed = 1.2f;
            player.AttackRange = 2f;

            Debug.Log("⚔️ [Striker] Stats Applied - HP:150, Power:25");
        }
    }

    public override void UseSkill()
    {
        RapidStrike();
    }

    public override void Attack()
    {
        // ใช้ระบบโจมตีจาก Sword script ที่มีอยู่แล้ว
        // (Sword จะจัดการเองผ่าน PlayerControls)
        Debug.Log("⚔️ [Striker] Attack! (ควบคุมโดย Sword script)");
    }

    public void RapidStrike()
    {
        Debug.Log("⚡ [Striker] Rapid Strike - โจมตีต่อเนื่อง!");
        // เพิ่มโค้ดสกิลพิเศษตรงนี้
    }

    public void CriticalHit()
    {
        bool isCrit = Random.value < critChance;
        float damage = isCrit ? player.AttackPower * 2f : player.AttackPower;

        Debug.Log($"💥 [Striker] Critical Hit! Damage: {damage} {(isCrit ? "(CRIT!)" : "")}");
    }
}