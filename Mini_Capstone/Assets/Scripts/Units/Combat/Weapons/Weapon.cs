using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon
{
    public enum WeaponType
    {
        Physical,
        Energy
    }

    //===============
    // Attributes:
    //===============
    public Unit unit; // the unit this weapon belongs to

    public WeaponType type; // physical or energy attack?

    public string name;

    public bool actionable; // can this weapon be used after moving or only at beginning of turn?

    public int power; // base damage dealt by this attack
    public float accuracy; // base accuracy of this attack (% multiplier)

    public int rangeMin; // minimum range of this attack
    public int rangeMax; // maximum range of this attack
   
    public List<Special> boardSpecials; // special attributes this weapon grants the player on the board
    public List<Special> combatSpecials; // special effects this weapon grants in combat

    public Weapon(Unit u)
    {
        //===================
        // Initialization:
        //===================
        boardSpecials = new List<Special>();
        combatSpecials = new List<Special>();

        unit = u;
    }

    public bool ContainsRange(int r)
    {
        for (int i = rangeMin; i <= rangeMax; i++)
        {
            if (i == r)
            {
                return true;
            }
        }

        return false;
    }
}
