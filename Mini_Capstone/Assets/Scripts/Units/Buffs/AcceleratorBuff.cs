using UnityEngine;
using System.Collections;


// provides +2 to movement speed / speed
public class AcceleratorBuff : Buff
{
    public AcceleratorBuff(Unit u) : base(u)
    {
        type = BuffType.Passive;

        // apply
        unit.speedBuff += 2;
        unit.movementBuff += 2;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove
        unit.speedBuff -= 2;
        unit.movementBuff -= 2; 
    }
}
