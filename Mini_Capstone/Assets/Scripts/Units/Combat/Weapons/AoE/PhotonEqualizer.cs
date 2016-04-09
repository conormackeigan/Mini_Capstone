using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//==================
// AoE WEAPON:
//==================

// Infantry AoE Weapon: 1 cell wide, 3 cells long in any direction
public class PhotonEqualizer : Weapon
{
    public PhotonEqualizer(Unit u) : base(u)
    {
        type = WeaponType.Energy;
        AoE = true; // AoE non-directional (false by default; no need to specify)
        directional = true;
        name = "PhtnEqualzr";
        actionable = false;
        power = 10;
        energy = 5;
        accuracy = 0.85f;
        rangeMin = 0;
        rangeMax = 3;
        boardSpecials = new List<Special>();
        //boardSpecials.Add();
        sfx = Resources.Load("Sound/SFX/sfxFrag") as AudioClip;
        AoEanim = Resources.Load("explosionFrag") as GameObject;
    }

    public override void markAoEAim(Vector2i root)
    {
        for (int i = Mathf.Max(unit.pos.x - 3, 0); i < unit.pos.x; i++)
        { // left
            TileMarker.Instance.addAoETile(new Vector2i(i, unit.pos.y));
        }
        for (int i = Mathf.Min(unit.pos.x + 3, MapScript.Instance.mapWidth); i > unit.pos.x; i--)
        { // right
            TileMarker.Instance.addAoETile(new Vector2i(i, unit.pos.y));
        }
        for (int i = Mathf.Max(unit.pos.y + 3, 0); i > unit.pos.y; i--)
        { // up
            TileMarker.Instance.addAoETile(new Vector2i(unit.pos.y, i));
        }
        for (int i = Mathf.Min(unit.pos.y - 3, MapScript.Instance.mapHeight); i < unit.pos.y; i++)
        { // down
            TileMarker.Instance.addAoETile(new Vector2i(unit.pos.y, i));
        }
    }

    public override void markAoEPattern(Vector2i root)
    {
        if (root.x < unit.pos.x)
        { // left
            for (int i = Mathf.Max(unit.pos.x - 3, 0); i < unit.pos.x; i++)
            {
                TileMarker.Instance.addAttackTile(new Vector2i(i, unit.pos.y));
            }
        }
        else if (root.x > unit.pos.x)
        { // right
            for (int i = Mathf.Min(unit.pos.x + 3, MapScript.Instance.mapWidth); i > unit.pos.x; i--)
            {
                TileMarker.Instance.addAttackTile(new Vector2i(i, unit.pos.y));
            }
        }
        else if (root.y > unit.pos.y)
        { // up
            for (int i = Mathf.Max(unit.pos.y + 3, 0); i > unit.pos.y; i--)
            {
                TileMarker.Instance.addAttackTile(new Vector2i(unit.pos.x, i));
            }
        }
        else if (root.y < unit.pos.y)
        { // down
            for (int i = Mathf.Min(unit.pos.y - 3, MapScript.Instance.mapHeight); i < unit.pos.y; i++)
            { 
                TileMarker.Instance.addAttackTile(new Vector2i(unit.pos.x, i));
            }
        }
    }

    public override void AoESequence(float timer)
    {
        if (timer == 0)
        {

        }
    }

    public override void StartAoEAnim()
    {
        
    }
}
