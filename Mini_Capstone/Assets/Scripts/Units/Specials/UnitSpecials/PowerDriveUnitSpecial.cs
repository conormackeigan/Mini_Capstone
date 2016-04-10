using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


// exo only special provides +5energy atk/+2speed
public class PowerDriveUnitSpecial : UnitSpecial
{
    public PowerDriveUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    public override void effect()
    {
        unit.buffs.Add(new PowerDriveBuff(unit));
    }

}
