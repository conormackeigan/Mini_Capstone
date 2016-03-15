using UnityEngine;
using System.Collections;

public class CharismaBuff : Buff
{
    public CharismaBuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (+2 to all stats)
        unit.healthBuff += 2;
        unit.physAtkBuff += 2;
        unit.energyAtkBuff += 2;
        unit.defenseBuff += 2;
        unit.speedBuff += 2;
        unit.movementBuff += 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        // remove charisma buff
        unit.healthBuff -= 2;
        unit.physAtkBuff -= 2;
        unit.energyAtkBuff -= 2;
        unit.defenseBuff -= 2;
        unit.speedBuff -= 2;
        unit.movementBuff -= 1;

        unit.buffs.Remove(this);
    }
}
