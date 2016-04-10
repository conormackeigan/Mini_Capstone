using UnityEngine;
using System.Collections;

public class PowerSuitBuff : Buff
{
    public PowerSuitBuff(Unit u) : base(u)
    {
        type = BuffType.Passive;

        // apply (+2 def, +1atk)
        unit.defenseBuff += 2;
        unit.physAtkBuff += 1;
        unit.energyAtkBuff += 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove powersuit buff
        unit.defenseBuff -= 2;
        unit.physAtkBuff -= 1;
        unit.energyAtkBuff -= 1;
    }
}
