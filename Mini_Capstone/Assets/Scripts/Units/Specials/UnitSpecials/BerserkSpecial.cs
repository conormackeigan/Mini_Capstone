using UnityEngine;
using System.Collections;
using System;


public class BerserkSpecial : Special
{
    public BerserkSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    //the effect this special grants
    public override void effect()
    {
        // adds a buff to the unit equipping this
        unit.buffs.Add(new BerserkBuff(unit));
    }
}
