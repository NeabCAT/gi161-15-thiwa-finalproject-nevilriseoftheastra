using UnityEngine;

public class Striker : BaseClass
{
    [Header("Striker Settings")]
    private Sword swordComponent;

    public override void Initialize(Player ownerPlayer, Transform weaponTransform)
    {
        base.Initialize(ownerPlayer, weaponTransform);
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
            player.MaxHealth = 5;
            player.Health = player.MaxHealth; 
            Debug.Log($"⚔️ [Striker] Stats Applied - HP:{player.Health}/{player.MaxHealth}");
        }
    }

    public override void UseSkill()
    {
        RapidStrike();
    }

    public override void Attack()
    {
        Debug.Log("⚔️ [Striker] Attack! (ควบคุมโดย Sword script)");
    }

    public void RapidStrike()
    {
        Debug.Log("⚡ [Striker] Rapid Strike - โจมตีต่อเนื่อง!");
    }
}