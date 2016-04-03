using UnityEngine;
using System.Collections;

// Specials are special weapon attributes.  They can be either static - provide a bonus upon equipping which remains as is until the weapon is unequipped,
// and dynamic - provide a bonus depending on a condition
public  class Special
{
    public Unit unit; // unit this special belongs to
    public SpecialCondition condition;

    public virtual void effect() { }

    public Special(Unit u)
    {
        unit = u;
    }
}
