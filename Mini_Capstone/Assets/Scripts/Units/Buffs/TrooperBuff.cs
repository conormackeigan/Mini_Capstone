using UnityEngine;
using System.Collections;

public class TrooperBuff : Buff
{
    public TrooperBuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (+2 to all stats)
        unit.healthBuff += 1;
        unit.physAtkBuff += 1;
        unit.energyAtkBuff += 1;
        unit.defenseBuff += 1;
        unit.speedBuff += 1;
        unit.movementBuff += 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove trooper buff
        unit.healthBuff -= 1;
        unit.physAtkBuff -= 1;
        unit.energyAtkBuff -= 1;
        unit.defenseBuff -= 1;
        unit.speedBuff -= 1;
        unit.movementBuff -= 1;
    }
}
