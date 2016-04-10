using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SniperRifle : Weapon
{
    public SniperRifle(Unit u) : base(u)
    {
        type = WeaponType.Physical;
        name = "SnipeRfl";
        actionable = false; // for testing; real rifle is actionable + range 1-2
        power = 8;
        accuracy = 0.99f;
        rangeMin = 2;
        rangeMax = 6;
        boardSpecials = new List<Special>();
        boardSpecials.Add(new TrooperSpecial(u));
        sfx = Resources.Load("Sound/SFX/sfxSingleShot") as AudioClip;
    }
}
