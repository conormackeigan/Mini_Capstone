using UnityEngine;
using System.Collections;

public abstract class UnitSpecialCondition
{
    public Unit unit; // the unit this special belongs to

    public bool rval;

    public abstract bool eval();

    public UnitSpecialCondition(Unit u)
    {
        unit = u;
    }
}
