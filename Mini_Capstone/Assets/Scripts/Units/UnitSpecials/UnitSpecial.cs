using UnityEngine;
using System.Collections;

public abstract class UnitSpecial
{
    public Unit unit; // the unit this special belongs to
    public UnitSpecialCondition condition;

    public abstract void effect();

    public UnitSpecial(Unit u)
    {
        unit = u;
    }
}
