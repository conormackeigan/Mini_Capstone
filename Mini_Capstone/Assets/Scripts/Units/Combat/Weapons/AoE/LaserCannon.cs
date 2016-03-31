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
        AoEanim = null;
    }

    public override void markAoEAim(Vector2i root)
    {     
        TileMarker tileMarker = TileMarker.Instance;

        unit.calcCombatStats();
        int range = Mathf.Max((int)(unit.combatEnergyAtk * 0.5f), 3); // minimum beam size is 3x3 (length extendable)

        // mark left beam
        for (int i = -1; i <= 1; i++)
        {
            if (unit.pos.y + i < 0 || unit.pos.y + i > MapScript.Instance.mapHeight)
            {
                continue; // don't process tiles outside of map
            }

            for (int j = Mathf.Max(root.x - range, 0); j < root.x; j++)
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
            if (unit.pos.y + i < 0 || unit.pos.y + i > MapScript.Instance.mapHeight)
            {
                continue; // don't process tiles outside of map
            }

            for (int j = Mathf.Min(root.x + range, MapScript.Instance.mapWidth); j > root.x; j--)
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
            if (unit.pos.x + i < 0 || unit.pos.x + i > MapScript.Instance.mapWidth)
            {
                continue; // don't process tiles outside of map
            }

            for (int j = Mathf.Min(root.y + 1, MapScript.Instance.Width); j <= root.y + range; j++)
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
            if (unit.pos.x + i < 0 || unit.pos.x + i > MapScript.Instance.mapWidth)
            {
                continue; // don't process tiles outside of map
            }

            for (int j = Mathf.Max(root.y - 1, 0); j >= root.y - range; j--)
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

    public override void markAoEPattern(Vector2i root) // root is redundant (always this weapon's unit)
    {
        TileMarker tileMarker = TileMarker.Instance;

int range = Mathf.Max((int)(unit.combatEnergyAtk * 0.5f), 3); // minimum beam size is 3x3 (length extendable)

        if (root == new Vector2i(unit.pos.x - 1, unit.pos.y) || unit.pos.x - root.x > 1)
        { // mark beam to left of unit
            root = unit.pos;
            for (int j = -1; j <= 1; j++)
            {
                if (unit.pos.y + j < 0 || unit.pos.y + j > MapScript.Instance.mapHeight)
                {
                    continue; // don't process tiles outside of map
                }

                for (int i = Mathf.Max(root.x - range, 0); i < root.x; i++)
                {
                    tileMarker.addAttackTile(new Vector2i(i, root.y + j));
                }
            }
        }
        else if (root == new Vector2i(unit.pos.x + 1, unit.pos.y) || unit.pos.x - root.x < -1)
        { // mark beam to right of unit
            root = unit.pos;
            for (int j = -1; j <= 1; j++)
            {
                if (unit.pos.y + j < 0 || unit.pos.y + j > MapScript.Instance.mapHeight)
                {
                    continue; // don't process tiles outside of map
                }

                for (int i = Mathf.Min(root.x + range, MapScript.Instance.mapHeight); i > root.x; i--)
                {
                    tileMarker.addAttackTile(new Vector2i(i, root.y + j));
                }
            }
        }
        else if (root == new Vector2i(unit.pos.x, unit.pos.y + 1) || unit.pos.y - root.y < -1)
        { // mark beam above unit
            root = unit.pos;
            for (int j = -1; j <= 1; j++)
            {
                if (unit.pos.x + j < 0 || unit.pos.x + j > MapScript.Instance.mapWidth)
                {
                    continue; // don't process tiles outside of map
                }

                for (int i = Mathf.Min(root.y + range, MapScript.Instance.mapHeight); i > root.y; i--)
                {
                    tileMarker.addAttackTile(new Vector2i(root.x + j, i));
                }
            }
        }
        else if (root == new Vector2i(unit.pos.x, unit.pos.y - 1) || unit.pos.y - root.y > 1)
        { // mark beam below unit
            root = unit.pos;
            for (int j = -1; j <= 1; j++)
            {
                if (unit.pos.x + j < 0 || unit.pos.x + j > MapScript.Instance.mapHeight)
                {
                    continue; // don't process tiles outside of map
                }

                for (int i = Mathf.Max(root.y - range, 0); i < root.y; i++)
                {
                    tileMarker.addAttackTile(new Vector2i(root.x + j, i));
                }
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
