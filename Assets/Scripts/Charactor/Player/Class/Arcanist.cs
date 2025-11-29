using UnityEngine;

public class Arcanist : BaseClass
{
    [Header("Arcanist Settings")]
    [SerializeField] private float spellSpeed = 10f;

    protected override void ApplyClassStats()
    {
        if (player != null)
        {
            player.MaxHealth = 3; // ⚠️ แก้จาก player.Health
            player.Mana = 150;
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