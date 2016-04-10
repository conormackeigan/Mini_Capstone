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
        rangeMax = 0;
        specials = new List<Special>();
        //specials.Add();
        sfx = Resources.Load("Sound/SFX/sfxFrag") as AudioClip;
        AoEanim = null;
    }

    // marks PURPLE TILES of all possible coverages for selection
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
            direction = new Vector2i(-1, 0);

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
            direction = new Vector2i(1, 0);

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
            direction = new Vector2i(0, 1);

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
            direction = new Vector2i(0, -1);

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

    // draw laser
    public override void StartAoEAnim()
    {
        int range = Mathf.Max((int)(unit.combatEnergyAtk * 0.5f), 3); // minimum beam size is 3x3 (length extendable)

        GameObject laserRoot = GameObject.Instantiate(Resources.Load("WeaponEffects/laser_root") as GameObject);
        laserRoot.transform.position = new Vector3(GLOBAL.gridToWorld(unit.pos).x + (int)IntConstants.TileSize * 0.5f, GLOBAL.gridToWorld(unit.pos).y + (int)IntConstants.TileSize * 0.5f);

        GameObject[] laser = new GameObject[range + 1];
        for (int i = 0; i < laser.Length; i++)
        {
            if (i == 0)
            {
                laser[i] = GameObject.Instantiate(Resources.Load("WeaponEffects/laser_tail")) as GameObject;
            }
            else if (i == laser.Length - 1)
            {
                laser[i] = GameObject.Instantiate(Resources.Load("WeaponEffects/laser_head")) as GameObject;
            }
            else
            {
                laser[i] = GameObject.Instantiate(Resources.Load("WeaponEffects/laser_mid")) as GameObject;
            }

            laser[i].transform.parent = laserRoot.transform;
        }

        // place laser pieces on correct tiles
        for (int i = 0; i < laser.Length; i++)
        {
            laser[i].transform.position = GLOBAL.gridToWorld(unit.pos + (new Vector2i(1, 0) * (i + 1)));           
        }
        laser[laser.Length - 1].transform.Translate(new Vector3(-5, 0, 0)); // offset head of laser by 5 pixels

        // rotate laser according to direction
        foreach (GameObject go in laser)
        {
            go.transform.Translate(new Vector3(0, (int)IntConstants.TileSize * 0.5f, 0)); // move up half a tile due to units' bottomleft anchor
        }

        if (direction.y == 1)
        {
            laserRoot.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (direction.x == -1)
        {         
            laserRoot.transform.eulerAngles = new Vector3(0, 0, 180);          
        }
        else if (direction.y == -1)
        {
            laserRoot.transform.eulerAngles = new Vector3(0, 0, 270);
        }

    }

    public void addPiece(int index)
    {

    }
}
