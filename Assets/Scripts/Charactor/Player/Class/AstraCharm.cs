using UnityEngine;

public class AstraCharm : BaseClass
{
    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.MaxHealth = 3; // ⚠️ แก้จาก player.Health
            Debug.Log("🏹 [AstraCharm] Stats Applied - HP:100, Power:20");
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