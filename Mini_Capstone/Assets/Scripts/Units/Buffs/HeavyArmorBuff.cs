using UnityEngine;
using System.Collections;

public class HeavyArmorBuff : Buff
{
    public HeavyArmorBuff(Unit u) : base(u)
    {
        type = BuffType.Passive;

        // apply (+5def)
        unit.defenseBuff += 5;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove heavy armor buff
        unit.defenseBuff -= 5;
    }
}
