using UnityEngine;
using System.Collections;

public class LongRangeExpertBuff : Buff
{
    public LongRangeExpertBuff(Unit u) : base(u)
    {
        type = BuffType.Combat;

        unit.physAtkBuff += CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
        unit.energyAtkBuff += CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
        unit.speedBuff += CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove longrange buff
        unit.physAtkBuff -= CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
        unit.energyAtkBuff -= CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
        unit.speedBuff -= CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos);
    }
}
