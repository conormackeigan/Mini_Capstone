using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharismaUnitSpecial : UnitSpecial
{
    public CharismaUnitSpecial(Unit u) : base(u)
    {
        condition = null;
    }

    public override void effect()
    {
        List<GameObject> units;

        if (unit.playerID == 1)
        {
            units = ObjectManager.Instance.PlayerOneUnits;
        }
        else //if (unit.playerID == 2)
        {
            units = ObjectManager.Instance.PlayerTwoUnits;
        }

        // charisma grants units within 3 spaces stat buffs
        foreach (GameObject go in units)
        {         
            Unit u = go.GetComponent<Unit>();

            if (u.Equals(unit))
            {
                continue; //can't buff yourself
            }

            if (u.Pos.Distance(unit.Pos) <= 3)
            {
                u.buffs.Add(new CharismaBuff(u));
            }

            // Apply additional commander buff to units with Trooper special
            if (u.Pos.Distance(unit.Pos) <= 2)
            {
                foreach (Special s in u.equipped.specials)
                {
                    if (s.GetType() == typeof(TrooperSpecial))
                    {
                        u.buffs.Add(new TrooperBuff(u));
                    }
                }
                foreach (UnitSpecial s in u.specials)
                {
                    if (s.GetType() == typeof(TrooperUnitSpecial))
                    {
                        u.buffs.Add(new TrooperBuff(u));
                    }
                }
            }
        }
    }

}
