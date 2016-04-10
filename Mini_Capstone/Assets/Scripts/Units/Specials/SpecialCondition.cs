using UnityEngine;
using System.Collections;

public  class SpecialCondition
{
    public Unit unit; // the unit this special belongs to

    public bool rval;

    public virtual bool eval() { return false; }

    public SpecialCondition(Unit u)
    {
        unit = u;
    }
}
