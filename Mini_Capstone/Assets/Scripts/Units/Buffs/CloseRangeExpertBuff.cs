using UnityEngine;
using System.Collections;

public class CloseRangeExpertBuff : Buff
{
    public CloseRangeExpertBuff(Unit u) : base(u)
    {
        type = BuffType.Combat;

        // apply (+2 to all stats if adjacent to unit)
        if (CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) == 1)
        {
            unit.physAtkBuff += 2;
            unit.energyAtkBuff += 2;
            unit.defenseBuff += 2;
            unit.speedBuff += 2;
        }
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove closerange buff
        if (CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) == 1)
        {
            unit.physAtkBuff -= 2;
            unit.energyAtkBuff -= 2;
            unit.defenseBuff -= 2;
            unit.speedBuff -= 2;
        }
    }
}
