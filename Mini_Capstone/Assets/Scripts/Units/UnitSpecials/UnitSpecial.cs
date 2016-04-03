using UnityEngine;
using System.Collections;

public  class UnitSpecial
{
    public Unit unit; // the unit this special belongs to
    public UnitSpecialCondition condition;

    public virtual void effect() { }

    public UnitSpecial(Unit u)
    {
        unit = u;
    }
}
