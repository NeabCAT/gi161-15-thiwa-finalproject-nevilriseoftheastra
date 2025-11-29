using UnityEngine;

public class AstraCharm : BaseClass
{
    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.MaxHealth = 3;
            player.Health = player.MaxHealth; // ⭐ เพิ่มบรรทัดนี้
            Debug.Log($"🏹 [AstraCharm] Stats Applied - HP:{player.Health}/{player.MaxHealth}");
        }
    }

    public override void UseSkill()
    {
        ChargedShot();
    }

    public override void Attack()
    {
        ShootArrow();
    }

    public void ShootArrow()
    {
        Debug.Log("🎯 [AstraCharm] Shoot Arrow!");
    }

    public void ChargedShot()
    {
        Debug.Log("⚡ [AstraCharm] Charged Shot - ลูกศรชาร์จพลัง!");
    }
}