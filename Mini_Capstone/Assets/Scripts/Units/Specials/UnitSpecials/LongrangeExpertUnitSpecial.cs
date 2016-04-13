using UnityEngine;
using System.Collections;
using System;

// provides +1atk/speed for every unit of distance between combatants
public class LongrangeExpertUnitSpecial : UnitSpecial
{
    public LongrangeExpertUnitSpecial(Unit u) : base(u)
    {
        condition = new CombatCondition(u);
    }

    //the effect this special grants
    public override void effect()
    {
        // increases accuracy by 10% and final damage by 1
        if (CombatSequence.Instance.attacker == unit && unit.state == Unit.UnitState.Combat)
        {
            CombatSequence.Instance.attacker.buffs.Add(new LongRangeExpertBuff(CombatSequence.Instance.attacker));          
        }
        else if (CombatSequence.Instance.defender == unit && unit.state == Unit.UnitState.Combat)
        {
            CombatSequence.Instance.defender.buffs.Add(new LongRangeExpertBuff(CombatSequence.Instance.defender));
        }
    }
}
