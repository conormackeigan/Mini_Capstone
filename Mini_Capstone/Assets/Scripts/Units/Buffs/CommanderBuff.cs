using UnityEngine;
using System.Collections;

public class CommanderBuff : Buff
{
    public CommanderBuff(Unit u) : base(u)
    {
        type = BuffType.Board;

        // apply (+1 to atk/movement)
        unit.physAtkBuff += 1;
        unit.energyAtkBuff += 1;
        unit.movementBuff += 1;
    }


    // remove buff effects on destruction
    public override void Destroy()
    {
        unit.buffs.Remove(this);

        // remove commander buff
        unit.physAtkBuff -= 1;
        unit.energyAtkBuff -= 1;
        unit.movementBuff -= 1;
    }
}
