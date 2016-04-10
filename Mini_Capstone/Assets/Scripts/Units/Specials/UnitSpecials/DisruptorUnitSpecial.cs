using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DisruptorUnitSpecial : UnitSpecial
{
    public DisruptorUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    public override void effect()
    {
        List<GameObject> units;

        if (unit.playerID == 1)
        {
            units = ObjectManager.Instance.PlayerTwoUnits;
        }
        else //if (unit.playerID == 2)
        {
            units = ObjectManager.Instance.PlayerOneUnits;
        }

        // intimidate debuffs units within radius 2
        foreach (GameObject go in units)
        {
            Unit u = go.GetComponent<Unit>();

            if (u.Pos.Distance(unit.Pos) <= 3)
            {
                u.buffs.Add(new DisruptorDebuff(u));
            }
        }
    }

}
