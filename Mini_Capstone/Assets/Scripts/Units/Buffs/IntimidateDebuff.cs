using UnityEngine;
using System.Collections;

public class IntimidateDebuff : Buff
{
    public IntimidateDebuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (-1 to all stats)
        unit.physAtkBuff -= 1;
        unit.energyAtkBuff -= 1;
        unit.defenseBuff -= 1;
        unit.speedBuff -= 1;
        unit.movementBuff -= 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove debuff
        unit.physAtkBuff += 1;
        unit.energyAtkBuff += 1;
        unit.defenseBuff += 1;
        unit.speedBuff += 1;
        unit.movementBuff += 1;
    }
}
