using UnityEngine;
using System.Collections;

// exo only 
public class PowerDriveBuff : Buff
{
    public PowerDriveBuff(Unit u) : base(u)
    {
        type = BuffType.Passive;

        // apply (+5 energy atk/+2 speed)
        unit.energyAtkBuff += 5;
        unit.speedBuff += 2;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove power drive buff
        unit.energyAtkBuff -= 5;
        unit.speedBuff -= 2;
    }
}
