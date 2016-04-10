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
        // increases damage of AoE attacks by 10%
        if (CombatSequence.Instance.attacker == unit && CombatSequence.Instance.attacker.equipped.AoE)
        {
            CombatSequence.Instance.attackerHitrate += 10;
        }
    }
}
