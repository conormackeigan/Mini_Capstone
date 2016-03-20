using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BeamSword : Weapon
{
    public BeamSword(Unit u) : base(u)
    {
        type = WeaponType.Energy;
        //AoE = false;
        name = "Beamsword";
        actionable = true;
        power = 15;
        accuracy = 0.99f;
        rangeMin = 1;
        rangeMax = 1;
        boardSpecials = new List<Special>();
        boardSpecials.Add(new LightweightSpecial(u));
        sfx = Resources.Load("Sound/SFX/sfxShock") as AudioClip;
    }
}
