using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Rifle : Weapon
{
    public Rifle(Unit u) : base(u)
    {
        type = WeaponType.Physical;
        name = "Rifle";
        actionable = true; // for testing; real rifle is actionable + range 1-2
        power = 9;
        accuracy = 0.98f;
        rangeMin = 1;
        rangeMax = 3;
        specials = new List<Special>();
        specials.Add(new TrooperSpecial(u));
        sfx = Resources.Load("Sound/SFX/sfxSingleShot") as AudioClip;
    }
}
