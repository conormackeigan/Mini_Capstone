using UnityEngine;
using System.Collections;

public  class UnitSpecialCondition
{
    public Unit unit; // the unit this special belongs to

    public bool rval;

    public virtual bool eval() { return false; }

    public UnitSpecialCondition(Unit u)
    {
        unit = u;
    }
}
