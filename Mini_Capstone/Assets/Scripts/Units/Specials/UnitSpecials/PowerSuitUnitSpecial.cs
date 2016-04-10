using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


// powersuit is an infantry only armor special that provides +2def/+1atk
public class PowerSuitUnitSpecial : UnitSpecial
{
    public PowerSuitUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    public override void effect()
    {
        unit.buffs.Add(new PowerSuitBuff(unit));
    }

}
