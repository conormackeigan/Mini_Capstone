using UnityEngine;
using System.Collections;

public class LongRangeExpertBuff : Buff
{
    public LongRangeExpertBuff(Unit u) : base(u)
    {
        type = BuffType.Combat;

        unit.physAtkBuff += Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) - 3, 0);
        unit.energyAtkBuff += Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) - 3, 0);
        unit.speedBuff += Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) - 3, 0);
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove longrange buff
        unit.physAtkBuff -= Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) -3, 0);
        unit.energyAtkBuff -= Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) - 3, 0);
        unit.speedBuff -= Mathf.Max(CombatSequence.Instance.attacker.pos.Distance(CombatSequence.Instance.defender.pos) - 3, 0);
    }
}
