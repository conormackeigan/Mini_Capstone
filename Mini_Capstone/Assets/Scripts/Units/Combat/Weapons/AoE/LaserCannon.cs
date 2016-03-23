using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//==================
// AoE WEAPON:
//==================

// large laser cannon - 3 cells wide, X cells long (function of energy atk) in any direction
public class LaserCannon : Weapon
{
    public LaserCannon(Unit u) : base(u)
    {
        type = WeaponType.Energy;
        AoE = true; // AoE non-directional (false by default; no need to specify)
        directional = true;
        name = "LsrCannon";
        actionable = false;
        power = 20;
        accuracy = 0.9f;
        rangeMin = 0;
        rangeMax = 3;
        boardSpecials = new List<Special>();
        //boardSpecials.Add();
        sfx = Resources.Load("Sound/SFX/sfxFrag") as AudioClip;
        AoEanim = Resources.Load("explosionFrag") as GameObject;
    }

    public override void markAoEAim(Vector2i root)
    {     
        TileMarker tileMarker = TileMarker.Instance;

        unit.calcCombatStats();
        int range = (int)(unit.combatEnergyAtk * 0.5f);

        // mark left beam
        for (int i = -1; i <= 1; i++)
        {
            for (int j = root.x - range; j < root.x; j++)
            {
                Vector2i mark = new Vector2i(j, root.y + i);

                if (mark.Distance(unit.pos) == 2 && mark.x != unit.pos.x && mark.y != unit.pos.y)
                {
                    if (!tileMarker.attackTiles.ContainsKey(mark))
                    {
                        //tiles directly diagonal to unit are ambiguous; mark with attack tile instead of AoE
                        tileMarker.addAttackTile(mark);
                    }
                }
                else
                {
                    tileMarker.addAoETile(mark);
                }
            }
        }
        // mark right beam
        for (int i = -1; i <= 1; i++)
        {
            for (int j = root.x + range; j > root.x; j--)
            {
                Vector2i mark = new Vector2i(j, root.y + i);

                if (mark.Distance(unit.pos) == 2 && mark.x != unit.pos.x && mark.y != unit.pos.y)
                {
                    //tiles directly diagonal to unit are ambiguous; mark with attack tile instead of AoE
                    if (!tileMarker.attackTiles.ContainsKey(mark))
                    {
                        //tiles directly diagonal to unit are ambiguous; mark with attack tile instead of AoE
                        tileMarker.addAttackTile(mark);
                    }
                }
                else
                {
                    tileMarker.addAoETile(mark);
                }
            }
        }
        // mark up beam
        for (int i = -1; i <= 1; i++) // i represents x now instead of y
        {
            for (int j = root.y + 1; j < root.y + range; j++)
            {
                Vector2i mark = new Vector2i(root.x + i, j);

                if (mark.Distance(unit.pos) == 2 && mark.x != unit.pos.x && mark.y != unit.pos.y)
                {
                    if (!tileMarker.attackTiles.ContainsKey(mark))
                    {
                        //tiles directly diagonal to unit are ambiguous; mark with attack tile instead of AoE
                        tileMarker.addAttackTile(mark);
                    }                  
                }
                else
                {
                    tileMarker.addAoETile(mark);
                }
            }
        }
        // mark down beam
        for (int i = -1; i <= 1; i++) // i represents x now instead of y
        {
            for (int j = root.y - 1; j > root.y - range; j--)
            {
                Vector2i mark = new Vector2i(root.x + i, j);

                if (mark.Distance(unit.pos) == 2 && mark.x != unit.pos.x && mark.y != unit.pos.y)
                {
                    if (!tileMarker.attackTiles.ContainsKey(mark))
                    {
                        //tiles directly diagonal to unit are ambiguous; mark with attack tile instead of AoE
                        tileMarker.addAttackTile(mark);
                    }
                }
                else
                {
                    tileMarker.addAoETile(mark);
                }
            }
        }

    }

    public override void markAoEPattern(Vector2i root)
    {
        TileMarker tileMarker = TileMarker.Instance;

        int range = (int)(unit.combatEnergyAtk * 0.5f);

        if (root == new Vector2i(unit.pos.x - 1, unit.pos.y) || unit.pos.x - root.x > 1)
        { // mark beam to left of unit
            root = unit.pos;
            for (int i = root.x - range; i < root.x; i++)
            {
                tileMarker.addAttackTile(new Vector2i(i, root.y + 1));
                tileMarker.addAttackTile(new Vector2i(i, root.y));
                tileMarker.addAttackTile(new Vector2i(i, root.y - 1));
            }
        }
        else if (root == new Vector2i(unit.pos.x + 1, unit.pos.y) || unit.pos.x - root.x < -1)
        { // mark beam to right of unit
            root = unit.pos;
            for (int i = root.x + range; i > root.x; i--)
            {
                tileMarker.addAttackTile(new Vector2i(i, root.y + 1));
                tileMarker.addAttackTile(new Vector2i(i, root.y));
                tileMarker.addAttackTile(new Vector2i(i, root.y - 1));
            }
        }
        else if (root == new Vector2i(unit.pos.x, unit.pos.y + 1) || unit.pos.y - root.y < -1)
        { // mark beam above unit
            root = unit.pos;
            for (int i = root.y + range; i > root.y; i--)
            {
                tileMarker.addAttackTile(new Vector2i(root.x - 1, i));
                tileMarker.addAttackTile(new Vector2i(root.x, i));
                tileMarker.addAttackTile(new Vector2i(root.x + 1, i));
            }
        }
        else if (root == new Vector2i(unit.pos.x, unit.pos.y - 1) || unit.pos.y - root.y > 1)
        { // mark beam below unit
            root = unit.pos;
            for (int i = root.y - range; i < root.y; i++)
            {
                tileMarker.addAttackTile(new Vector2i(root.x - 1, i));
                tileMarker.addAttackTile(new Vector2i(root.x, i));
                tileMarker.addAttackTile(new Vector2i(root.x + 1, i));
            }
        }

    }

    public override void AoESequence(float timer)
    {
        if (timer == 0)
        {

        }
    }
}
