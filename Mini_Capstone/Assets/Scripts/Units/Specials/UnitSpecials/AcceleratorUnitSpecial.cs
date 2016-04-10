using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


// commander provides trooper buffs to units with the trooper special and constant +1 to atk/movement
public class AcceleratorUnitSpecial : UnitSpecial
{
    public AcceleratorUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    public override void effect()
    {
        unit.buffs.Add(new AcceleratorBuff(unit));
    }

}
