using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//==================
// AoE WEAPON:
//==================

// large laser cannon - 3 cells wide, X cells long (function of energy atk) in any direction
public class EnergyChain : Weapon
{
    int max; // keeps track of maximum distance of attack to know when to administer damage
    GameObject shot; // attack instantiation

    public EnergyChain(Unit u) : base(u)
    {
        type = WeaponType.Energy;
        AoE = true; // AoE non-directional (false by default; no need to specify)
        directional = false;
        name = "EnrgyChn";
        actionable = false;
        power = 14;
        accuracy = 0.9f;
        rangeMin = 0;
        rangeMax = 0;
        boardSpecials = new List<Special>();
        //boardSpecials.Add();
        sfx = Resources.Load("Sound/SFX/sfxFrag") as AudioClip;
        AoEanim = null;
    }

    public override void markAoEPattern(Vector2i root) // root is redundant (always this weapon's unit)
    {
        TileMarker tileMarker = TileMarker.Instance;

        //mark all diagonal tiles from unit

        Vector2i curr = new Vector2i(unit.pos.x - 1, unit.pos.y + 1);

        // up left
        while (curr.x >= 0 && curr.y < MapScript.Instance.mapHeight)
        {
            if (curr.Distance(unit.pos) > max)
            { // keep track of maximum distance attack
                max = curr.Distance(unit.pos);
            }

            tileMarker.addAttackTile(curr);
            curr.x--;
            curr.y++;
        }

        // up right
        curr = new Vector2i(unit.pos.x + 1, unit.pos.y + 1);
        while (curr.x < MapScript.Instance.mapWidth && curr.y < MapScript.Instance.mapHeight)
        {
            if (curr.Distance(unit.pos) > max)
            { // keep track of maximum distance attack
                max = curr.Distance(unit.pos);
            }

            tileMarker.addAttackTile(curr);
            curr.x++;
            curr.y++;
        }

        // down left
        curr = new Vector2i(unit.pos.x - 1, unit.pos.y - 1);
        while (curr.x >= 0 && curr.y >= 0)
        {
            if (curr.Distance(unit.pos) > max)
            { // keep track of maximum distance attack
                max = curr.Distance(unit.pos);
            }

            tileMarker.addAttackTile(curr);
            curr.x--;
            curr.y--;
        }

        // down right
        curr = new Vector2i(unit.pos.x + 1, unit.pos.y - 1);
        while (curr.x < MapScript.Instance.mapWidth && curr.y >= 0)
        {
            if (curr.Distance(unit.pos) > max)
            { // keep track of maximum distance attack
                max = curr.Distance(unit.pos);
            }

            tileMarker.addAttackTile(curr);
            curr.x++;
            curr.y--;
        }

    }

    public override void AoESequence(float timer)
    {
        if (timer == 0)
        {

        }
    }

    // generate initial 1-4 surrounding attacks
    public override void StartAoEAnim()
    {
        shot = Resources.Load("WeaponEffects/EnergyGreen") as GameObject;

        Vector2i dir = new Vector2i(-1, 1);
        Vector2i start = unit.pos + dir;

        if (start.x >= 0 && start.y < MapScript.Instance.mapHeight)
        { // upper left
            GameObject go = GameObject.Instantiate(Resources.Load("WeaponEffects/EnergyGreen") as GameObject, GLOBAL.gridToWorld(start), Quaternion.identity) as GameObject;
            go.GetComponent<EnergyChainShot>().max = max;
            go.GetComponent<EnergyChainShot>().direction = dir;
        }

        dir = new Vector2i(1, 1);
        start = unit.pos + dir;
        if (start.x < MapScript.Instance.mapWidth && start.y < MapScript.Instance.mapHeight)
        { // upper right
            GameObject go = GameObject.Instantiate(Resources.Load("WeaponEffects/EnergyGreen") as GameObject, GLOBAL.gridToWorld(start), Quaternion.identity) as GameObject;
            go.GetComponent<EnergyChainShot>().max = max;
            go.GetComponent<EnergyChainShot>().direction = dir;
        }

        dir = new Vector2i(-1, -1);
        start = unit.pos + dir;
        if (start.x >= 0 && start.y >= 0)
        { // bottom left
            GameObject go = GameObject.Instantiate(Resources.Load("WeaponEffects/EnergyGreen") as GameObject, GLOBAL.gridToWorld(start), Quaternion.identity) as GameObject;
            go.GetComponent<EnergyChainShot>().max = max;
            go.GetComponent<EnergyChainShot>().direction = dir;
        }

        dir = new Vector2i(1, -1);
        start = unit.pos + dir;
        if (start.x < MapScript.Instance.mapWidth && start.y >= 0)
        { // bottom right
            GameObject go = GameObject.Instantiate(Resources.Load("WeaponEffects/EnergyGreen") as GameObject, GLOBAL.gridToWorld(start), Quaternion.identity) as GameObject;
            go.GetComponent<EnergyChainShot>().max = max;
            go.GetComponent<EnergyChainShot>().direction = dir;
        }
    }

}
