using UnityEngine;
using System.Collections;

// this condition is used for all specials that activate when combat starts
public class CombatCondition : SpecialCondition
{
    public CombatCondition(Unit u) : base(u)
    {

    }

    public override bool eval()
    {
        if (unit.state != Unit.UnitState.Combat)
        {
            return true;
        }

        return false;
    }
}
