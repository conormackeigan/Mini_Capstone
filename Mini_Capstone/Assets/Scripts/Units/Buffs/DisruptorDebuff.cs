using UnityEngine;
using System.Collections;

public class DisruptorDebuff : Buff
{
    public DisruptorDebuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (-2 to energy atk)
        unit.energyAtkBuff -= 2;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove debuff
        unit.energyAtkBuff += 2;
    }
}
