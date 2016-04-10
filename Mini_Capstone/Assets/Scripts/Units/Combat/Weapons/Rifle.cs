using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Rifle : Weapon
{
    public Rifle(Unit u) : base(u)
    {
        type = WeaponType.Physical;
        //AoE = false;
        name = "Rifle";
        actionable = false; // for testing; real rifle is actionable + range 1-2
        power = 8;
        accuracy = 0.99f;
        rangeMin = 1;
        rangeMax = 3;
        specials = new List<Special>();
        specials.Add(new TrooperSpecial(u));
        sfx = Resources.Load("Sound/SFX/sfxSingleShot") as AudioClip;
    }
}
