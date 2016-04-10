using UnityEngine;
using System.Collections;

public class BerserkBuff : Buff
{
    public BerserkBuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply
        unit.movementBuff += 1;
        unit.physAtkBuff += 3;
        unit.energyAtkBuff += 3;
        unit.defenseBuff -= 2;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove berserk buff
        unit.movementBuff -= 1;
        unit.physAtkBuff -= 3;
        unit.energyAtkBuff -= 3;
        unit.defenseBuff += 2;
    }
}
