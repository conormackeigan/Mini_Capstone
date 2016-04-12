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
        actionable = true; // for testing; real rifle is actionable + range 1-2
        power = 9;
        accuracy = 0.95f;
        rangeMin = 1;
        rangeMax = 3;
        specials = new List<Special>();
        //specials.Add(new TrooperSpecial(u));
        specials.Add(new HawkeyeSpecial(u)); // debug: for defender combat skill offsets in AI-instigated attacks
        sfx = Resources.Load("Sound/SFX/sfxSingleShot") as AudioClip;
    }
}
