using UnityEngine;
using System.Collections;
using System;

// Grants +1 to all unit stats when within 2 tiles of commander unit
// This class is used as a tag for commander units to distribute buffs to
public class TrooperUnitSpecial : Special
{
    public TrooperUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    //the effect this special grants
    public override void effect()
    {
    }
}
