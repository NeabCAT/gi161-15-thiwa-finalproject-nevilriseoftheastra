using UnityEngine;

public class AstraCharm : BaseClass
{
    //[Header("Astra Charm Settings")]
    //[SerializeField] private float arrowSpeed = 15f;

    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.Health = 100;
            player.AttackPower = 20f;
            player.AttackSpeed = 1.5f;
            player.AttackRange = 8f;

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