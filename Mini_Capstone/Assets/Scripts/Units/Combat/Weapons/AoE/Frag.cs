using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//==================
// AoE WEAPON:
//==================
public class Frag : Weapon
{
    public Frag(Unit u) : base(u)
    {
        type = WeaponType.Physical;
        AoE = true; // AoE non-directional (false by default; no need to specify)
        name = "Frag";
        actionable = false;
        power = 10;
        accuracy = 0.85f;
        rangeMin = 0;
        rangeMax = 3;
        boardSpecials = new List<Special>();
        //boardSpecials.Add();
        sfx = Resources.Load("Sound/SFX/sfxFrag") as AudioClip;
        AoEanim = Resources.Load("explosionFrag") as GameObject;
    }

    public override void markAoEPattern(Vector2i root)
    {
        TileMarker tileMarker = TileMarker.Instance;

        //grenade is radius 1, so simply mark the 5 tiles manually rather than iterating
        if (!tileMarker.attackTiles.ContainsKey(root))
            tileMarker.addAttackTile(root);

        if (!tileMarker.attackTiles.ContainsKey(new Vector2i(root.x - 1, root.y)))
            tileMarker.addAttackTile(new Vector2i(root.x - 1, root.y));

        if (!tileMarker.attackTiles.ContainsKey(new Vector2i(root.x + 1, root.y)))
            tileMarker.addAttackTile(new Vector2i(root.x + 1, root.y));

        if (!tileMarker.attackTiles.ContainsKey(new Vector2i(root.x, root.y - 1)))
            tileMarker.addAttackTile(new Vector2i(root.x, root.y - 1));

        if (!tileMarker.attackTiles.ContainsKey(new Vector2i(root.x, root.y + 1)))
            tileMarker.addAttackTile(new Vector2i(root.x, root.y + 1));
    }

    public override void AoESequence(float timer)
    {
        // frag is a simple explosion animation
        if (timer == 0)
        {

        }
    }
}
