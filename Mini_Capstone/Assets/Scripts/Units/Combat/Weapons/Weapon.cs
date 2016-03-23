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

    public AudioClip sfx; // the sfx attached to this weapon

    //=======================
    // AoE Attributes:
    //=======================
    public bool AoE = false; // is this an area of effect weapon?

    // DIRECTIONAL ATTRIBUTES:
    public bool directional = false; // does this AoE have a direction from root node or a radius?

    public GameObject AoEanim; // animation for AoE attacks (or regular attacks if implemented)

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


    public virtual void markAoEPattern(Vector2i root)
    { // marks attack pattern from a root node        
    }

    public virtual void markAoEAim(Vector2i root)
    { // marks all possible attack patterns in purple tiles (ambiguous tiles are red)
    }

    public virtual void AoESequence(float timer)
    { // real-time AoE sequence controlled in CombatSequence.Update()
    }
}
