using UnityEngine;
using System.Collections;

public class LightweightBuff : Buff
{
    public LightweightBuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (+1 to movement speed and speed)
        unit.movementBuff += 1;
        unit.speedBuff += 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove lightweight buff
        unit.movementBuff -= 1;
        unit.speedBuff -= 1;
    }
}
