﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// handles combat sequences after units' combat stats have been calculated
public class CombatSequence : MonoBehaviour
{
    public bool active = false; // can't call Find on inactive game objects so handle activity manually
    public bool preCombat = true; // precombat phase, set to false once combat is initiated
    public bool retaliation = false; // set to true if the defending unit is able to counter attack

    public GameObject crosshairs; // combat sequences starts by sending a crosshair lockon then waiting for its signal
    private GameObject lockon; // instantiation of crosshairs

    private Unit attacker;
    private Unit defender;

    public bool attackerHit; // whether attacker/defender's attacks were hits (true) or misses (false)
    public bool defenderHit;

    public int distance; // range of the instigating attack

    public int attackerDamage;
    public float attackerHitrate; // hit rate is stored and calculated as float but displayed as int (floor)
    public int defenderDamage;
    public float defenderHitrate;

    public Weapon attackerOrigWep; // store weapons both units are equipping at the beginning of the sequence in case of Cancel()
    public Weapon defenderOrigWep;

    // UI variables (temp)
    public GameObject UI; // parent UI gameobject
    public GameObject attackerUI;
    public GameObject defenderUI;
    public Text attackerDamageText;
    public Text attackerAccuracyText;
    public Text defenderDamageText;
    public Text defenderAccuracyText;

    public GameObject wepSelect; // weapon select UI parent gameobject

    //====================
    // Real-time Combat:
    //====================
    Vector3 displacement; // the displacement vector between the units, used for travelling along

    // these states control real-time movement.  this is super janky and hopefully we can just get/make animations that contain the movements
    public enum CombatPhase
    {
        attackerLunge,
        attackerRetreat,
        defenderLunge,
        defenderRetreat
    }

    CombatPhase phase;
    public float timer; // combat phases are controlled by a timer because we lack animations


    public void Update()
    {
        if (!preCombat)
        {
            Combat();
        }
    }

    // called manually on enable
    public void Enable (Unit a, Unit d)
    {
        attacker = a;
        defender = d;

        attackerOrigWep = attacker.equipped;
        defenderOrigWep = defender.equipped;

        distance = attacker.pos.Distance(defender.pos);

        // TODO: Weapon Select Window
        // Attacker selects weapon and presses CONFIRM button, then Defender selects weapon and presses CONFIRM calling InitiateSequence()
        // Note all UI is temporary.  These variables may have to be modified or nuked to accommodate whatever you decide to implement.

        // TODO: GENERATE CONFIRM AND CANCEL BUTTON. CURRENTLY USING TEMP STATES TO HANDLE LOGIC

        // crosshair lockon
        lockon = Instantiate(crosshairs, GLOBAL.gridToWorld(attacker.pos), Quaternion.identity) as GameObject;
        lockon.GetComponent<CrosshairsController>().target = defender.pos;

        // check if units are equipping proper weapons (if available)
        CheckWeapons();

        // calculate damage and hit rate for each unit
        CalculateStats(); 
	}
	
    
    public void Begin()
    {
        CalculateStats();
        Display();      
    }

    // displays combat information and TODO: generates confirm button that fires InitiateSequence()
    public void Display()
    {
        // display information
        attackerDamageText.text = attackerDamage.ToString();
        attackerAccuracyText.text = attackerHitrate.ToString();

        if (retaliation)
        {
            defenderDamageText.text = defenderDamage.ToString();
            defenderAccuracyText.text = defenderHitrate.ToString();
        }
        else
        {
            defenderDamageText.text = "--";
            defenderAccuracyText.text = "--";
        }

        EnableUI();

        // Weapon Select window
        WeaponSelect();
    }

    // generates weapon select windows for both units (come up w/ badass chart format)
    // TODO: case is hardcoded with attacker variable. expand sequence for defender too
    public void WeaponSelect()
    {
        // INFO: max of 5 weapons so make the select menu static. if the unit has < 5 weps, the bottom entries of the list will read "---" (see eustrath combat screen)

        // Start new weapon select screen by disabling all wep slots that aren't used
        for (int i = 1; i <= 5; i++)
        {
            GameObject ui = GameObject.Find("Attack" + i);
            if (i > attacker.weapons.Count)
            { // don't have a weapon to fill this slot, leave it blank
                for (int j = 0; j < ui.transform.childCount; j++)
                {
                    ui.transform.GetChild(j).gameObject.SetActive(false);
                }

                continue;
            }

            // if this is the default selected weapon, highlight it
            if (attacker.equipped == attacker.weapons[i - 1])
            {
                ChangeSelection(i, attacker);                
            }

            ui.transform.GetChild(0).gameObject.SetActive(true);
            ui.transform.GetChild(0).GetComponent<Text>().text = attacker.weapons[i - 1].name; // NAME
            ui.transform.GetChild(1).gameObject.SetActive(true);
            ui.transform.GetChild(1).GetComponent<Text>().text = attacker.weapons[i - 1].power.ToString(); // WEAPON POWER
            ui.transform.GetChild(2).gameObject.SetActive(true);
            ui.transform.GetChild(2).GetComponent<Text>().text = attacker.weapons[i - 1].rangeMin + "-" + attacker.weapons[i - 1].rangeMax; // RANGE
            ui.transform.GetChild(3).gameObject.SetActive(true);
            ui.transform.GetChild(3).GetComponent<Text>().text = (attacker.weapons[i - 1].accuracy * 100).ToString(); // ACCURACY
            ui.transform.GetChild(4).gameObject.SetActive(true);

            if (attacker.weapons[i - 1].actionable)
                ui.transform.GetChild(4).GetComponent<Text>().text = "Yes";
            else
                ui.transform.GetChild(4).GetComponent<Text>().text = "No";

            //if this weapon is not actionable and unit has moved, or the weapon is not in range, disable button and grey out
            ui.transform.GetChild(5).gameObject.SetActive(true);
            if ((!attacker.weapons[i - 1].actionable && attacker.pos != attacker.selectPos) ||
                 (!attacker.weapons[i - 1].ContainsRange(distance)))
            {
                ui.transform.GetChild(5).gameObject.SetActive(false); // disable button (4th child)
            }
            
        }

    }

    // unhighlights whatever weapon was previously selected and highlights the newly selected one
    public void ChangeSelection(int selection, Unit u)
    {
        // selection = slot number of selected weapon
        for (int i = 0; i < u.weapons.Count; i++)
        {
            GameObject ui = GameObject.Find("Attack" + (i + 1));

            if (i + 1 == selection)
            {
                ui.transform.GetChild(ui.transform.childCount - 1).gameObject.SetActive(true);
            }
            else
            {
                ui.transform.GetChild(ui.transform.childCount - 1).gameObject.SetActive(false);
            }
        }
    }

    // method fired from weapon select buttons
    public void SelectWeapon(int index)
    {
        attacker.Equip(attacker.weapons[index - 1]);
        ChangeSelection(index, attacker);
        CalculateStats();
        Display(); // refresh values in combat window
    }

    void CalculateStats()
    {
        Calculate(attacker, defender, ref attackerDamage, ref attackerHitrate);
        Calculate(defender, attacker, ref defenderDamage, ref defenderHitrate);
    }

    void Calculate(Unit unit, Unit other, ref int dmg, ref float acc)
    {
        if (unit == defender && !retaliation)
        {
            return;
        }

        unit.calcCombatStats(); // in case something changed recalculate combat stats
        //=================
        // Accuracy:
        //=================
        // temp accuracy equation, will require balancing/overhauling. currently 100% @ +1 spd, 95% acc @ even, 45% @ -10, 0% @ -19 before weapon acc
        acc = ((19 + (unit.combatSpeed - other.combatSpeed)) * 5) * unit.equipped.accuracy;
        if (acc < 0)
            acc = 0;
        else if (acc > 100)
            acc = 100;

        //=================
        // Damage:
        //=================
        if (unit.equipped.type == Weapon.WeaponType.Energy)
        { // ENERGY:
            dmg = unit.combatEnergyAtk + unit.equipped.power - other.combatDefense; // energy damage is a consistent funciton of atk/def disparity
        }
        else 
        { // PHYSICAL:
            // physical dmg calculation is less straightforward than energy. get the dmg/def disparity, and apply it relative to parabola w/ wep power
            int delta = unit.combatPhysAtk + unit.equipped.power - other.combatDefense;

            // let's assume approximate def spectrum is 1-50 for now (reduce if too unbalanced)

            // TEMP EQUATION, TODO: figure out parabola normalizing
            dmg = unit.combatPhysAtk - other.combatDefense;
        }
    }

    void CheckWeapons()
    {
        // if attacker's currently equipped weapon is not in range of defender (or not actionable post-move), equip first wep in inventory within range
        
        if (!attacker.equipped.ContainsRange(distance) || (!attacker.equipped.actionable && attacker.pos != attacker.selectPos))
        { // equipped weapon not in range, find one that is (attacker must have one for the sequence to initialize)
            foreach (Weapon w in attacker.weapons)
            {
                if (w.ContainsRange(distance))
                { 
                    if (!w.actionable && attacker.pos != attacker.selectPos)
                    {
                        continue; // can't use non-actionable weapons after moving
                    }

                    attacker.Equip(w);
                    break;
                }
            }
        }

        // unlike attacker, defender may not have a weapon within range. if they do equip it, if none tell game defender isn't attacking w/ equipped wep

        if (!defender.equipped.ContainsRange(distance))
        { // equipped weapon not in range, try to find one, if not flip retaliation flag
            foreach (Weapon w in defender.weapons)
            {
                if (w.ContainsRange(distance))
                { // found an appropriate weapon, equip and break out
                    defender.Equip(w);
                    retaliation = true;
                    break;
                }
            }
        }
        else
        {
            retaliation = true; // equipped weapon is already in range
        }
    }


    public void InitiateSequence()
    {
        // destroy crosshairs
        Destroy(lockon);

        // disable UI components
        DisableUI();

        // TODO: action sequence, applying health loss and death checks upon end of movement sequence, breaking if the opponent dies
        preCombat = false;

        // calculate battle results before depicting (TEMP: needs to be greatly expanded, especially if we implement multiple attacks)
        attackerHit = false;
        defenderHit = false;

        if (Random.Range(0, 100) <= attackerHitrate)
            attackerHit = true;
        if (retaliation && Random.Range(0, 100) <= defenderHitrate)
            defenderHit = true;

        // TEMP movement stuff
        phase = CombatPhase.attackerLunge;
        timer = 0;
        displacement = (GLOBAL.gridToWorld(defender.pos) - GLOBAL.gridToWorld(attacker.pos)).normalized;

        //Finish();
    }

    void Combat()
    {
        timer += Time.deltaTime;

        // this is super temporary, TODO: implement strike animations (single image w/ movement)
        switch(phase)
        {
            case CombatPhase.attackerLunge:
                {
                    if (timer > 0.25f && timer < 0.5f)
                    {
                        attacker.transform.position += displacement * Time.deltaTime * 56;
                    }
                    else if (timer >= 0.5f)
                    {
                        if (attackerHit)
                        {
                            defender.Damage(attackerDamage);
                        }

                        phase = CombatPhase.attackerRetreat;
                    }
                    break;
                }

            case CombatPhase.attackerRetreat:
                {
                    if (timer < 0.75f)
                    {
                        attacker.transform.position -= displacement * Time.deltaTime * 56;
                    }
                    else if (timer < 1.25f)
                    {
                        attacker.snapToGridPos();
                    }
                    else
                    {                       
                        phase = CombatPhase.defenderLunge;
                    }

                    break;
                }

            case CombatPhase.defenderLunge:
                {
                    if (defender.isDead)
                    {
                        Finish();
                    }
                    else
                    {
                        if (timer < 1.5f)
                        {
                            defender.transform.position -= displacement * Time.deltaTime * 56;
                        }
                        else
                        {
                            if (defenderHit)
                            {
                                attacker.Damage(defenderDamage);
                            }

                            phase = CombatPhase.defenderRetreat;
                        }
                    }

                    break;
                }

            case CombatPhase.defenderRetreat:
                {
                    if (timer < 1.75f)
                    {
                        defender.transform.position += displacement * Time.deltaTime * 56;
                    }
                    else
                    {
                        defender.snapToGridPos();

                        Finish();
                    }

                    break;
                }
        }
        
    }

    // cleans up when combat sequence has finished
    public void Finish()
    {
        active = false;
        preCombat = true;
        retaliation = false;

        if (!attacker.isDead)
        {
            attacker.Deactivate();
        }
        else
        {
            PlayerManager.Instance.getCurrentPlayer().selectedObject = null;
        }
    }

    // cleans up when combat sequence was cancelled before intialization
    public void Cancel()
    {
        active = false;
        preCombat = true;
        retaliation = false;

        DisableUI();
        Destroy(lockon);

        // reset weapons
        if (attacker.equipped != attackerOrigWep)
        {
            attacker.Equip(attackerOrigWep);
        }
        if (defender.equipped != defenderOrigWep)
        {
            defender.Equip(defenderOrigWep);
        }
      
    }

    public void EnableUI()
    {
        for (int i = 0; i < 3; i++)
        {
            UI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void DisableUI()
    {
        for (int i = 0; i < 3; i++)
        {
            UI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
