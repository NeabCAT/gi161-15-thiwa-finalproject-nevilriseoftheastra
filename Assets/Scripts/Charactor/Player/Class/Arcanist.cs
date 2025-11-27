using UnityEngine;

public class Arcanist : BaseClass
{
    [Header("Arcanist Settings")]
    [SerializeField] private float spellSpeed = 10f;

    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.Health = 80;
            player.AttackPower = 40f;
            player.AttackSpeed = 0.8f;
            player.AttackRange = 6f;
            player.Mana = 150;

            Debug.Log("🔮 [Arcanist] Stats Applied - HP:80, Power:40, Mana:150");
        }
    }

    public override void UseSkill()
    {
        ArcaneBlast();
    }

    public override void Attack()
    {
        CastSpell();
    }

    public void CastSpell()
    {
        if (player.Mana >= 10)
        {
            player.Mana -= 10;
            Debug.Log($"✨ [Arcanist] Cast Spell! Mana: {player.Mana}");
            // ยิงเวทย์
        }
        else
        {
            Debug.Log("⚠️ [Arcanist] ไม่พอ Mana!");
        }
    }

    public void ArcaneBlast()
    {
        if (player.Mana >= 30)
        {
            player.Mana -= 30;
            Debug.Log("💫 [Arcanist] Arcane Blast - ระเบิดพลังเวทย์!");
        }
    }
}