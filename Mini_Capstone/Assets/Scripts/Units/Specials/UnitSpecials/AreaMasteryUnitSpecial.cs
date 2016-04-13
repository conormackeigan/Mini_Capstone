using UnityEngine;
using System.Collections;
using System;

// area of effect attacks deal 10% more damage
public class AreaMasteryUnitSpecial : UnitSpecial
{
    public AreaMasteryUnitSpecial(Unit u) : base(u)
    {
        condition = new CombatCondition(u);
    }

    //the effect this special grants
    public override void effect()
    {
        // increases damage of AoE attacks by 10%, rounds up instead of down
        if (CombatSequence.Instance.attacker == unit && CombatSequence.Instance.attacker.equipped.AoE)
        {
            CombatSequence.Instance.attackerDamage = (int)(Mathf.Ceil(CombatSequence.Instance.attackerDamage * 1.1f));
        }
    }
}
