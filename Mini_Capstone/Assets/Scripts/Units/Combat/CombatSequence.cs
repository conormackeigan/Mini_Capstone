using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// handles combat sequences after units' combat stats have been calculated
public class CombatSequence : Singleton<CombatSequence>
{
    public bool active = false; // can't call Find on inactive game objects so handle activity manually
    public bool preCombat = true; // precombat phase, set to false once combat is initiated
    public bool retaliation = false; // set to true if the defending unit is able to counter attack

    public GameObject crosshairs; // combat sequences starts by sending a crosshair lockon then waiting for its signal
    private GameObject lockon; // instantiation of crosshairs

    public Unit attacker;
    public Unit defender;

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
    public Image attackerImage;
    public Text attackerNameText;
    public Text attackerHealthText;
    public Text attackerManaText;
    public Text attackerDamageText;
    public Text attackerAccuracyText;

    public Image defenderImage;
    public Text defenderNameText;
    public Text defenderHealthText;
    public Text defenderManaText;
    public Text defenderDamageText;
    public Text defenderAccuracyText;

    //====================
    // Area of Effect:
    //====================
    public GameObject AoEUI; // weapon select UI parent gameobject
    public Vector2i AoERoot; // the root of the current (or most recently stored) AoE attack
    public Weapon AoEWeapon; // the selected weapon for AoE attack
    public bool AoESequence = false; // if a real-time AoE sequence is occurring

    //DEBUG:
    public float multiplier;
    public float normalized;

    //====================
    // Real-time Combat:
    //====================
    public Vector3 displacement; // the displacement vector between the units, used for travelling along

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
        // regular real-time combat sequence
        if (!preCombat)
        {
            Combat();
        }

        // AoE real-time sequence
        else if (AoESequence)
        {
            AoEWeapon.AoESequence(timer);
            timer += Time.deltaTime;
        }
    }

    // called manually on enable by Unit.Attack()
    public void Enable(Unit a, Unit d)
    {
        attacker = a;
        defender = d;

        attackerOrigWep = attacker.equipped;
        defenderOrigWep = defender.equipped;

        distance = attacker.pos.Distance(defender.pos);

        // Attacker selects weapon and presses CONFIRM button, then Defender selects weapon and presses CONFIRM calling InitiateSequence()
        // Note all UI is temporary.  These variables may have to be modified or nuked to accommodate whatever you decide to implement.

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

    // displays combat information
    public void Display()
    {
        // display information
        attackerImage.sprite = attacker.sprite;
        attackerNameText.text = attacker.unitName;
        attackerHealthText.text = attacker.health.ToString();
        attackerManaText.text = "100";
        attackerDamageText.text = attackerDamage.ToString();
        attackerAccuracyText.text = attackerHitrate.ToString();

        if (retaliation)
        {
            defenderImage.sprite = defender.sprite;
            defenderNameText.text = defender.unitName;
            defenderHealthText.text = defender.health.ToString();
            defenderManaText.text = "100";
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
        WeaponSelect(attacker);
    }

    // generates weapon select windows for both units (come up w/ badass chart format)
    public void WeaponSelect(Unit unit, bool AoE = false)
    {
        // INFO: max of 3 weapons so make the select menu static. if the unit has < 3 weps, the bottom entries of the list will read "---" (see eustrath combat screen)

        // Start new weapon select screen by disabling all wep slots that aren't used
        for (int i = 1; i <= 3; i++)
        {
            GameObject ui;
            if (!AoE)
                ui = GameObject.Find("Attack" + i);
            else
                ui = GameObject.Find("AoEAttack" + i);

            if (i > unit.weapons.Count)
            { // don't have a weapon to fill this slot, leave it blank
                for (int j = 0; j < ui.transform.childCount; j++)
                {
                    ui.transform.GetChild(j).gameObject.SetActive(false);
                }

                continue;
            }

            // if this is the default selected weapon, highlight it (no highlights for AoE menu)
            if (unit.equipped == unit.weapons[i - 1] && !AoE)
            {
                ChangeSelection(i, unit);
            }

            ui.transform.FindChild("WeaponName").GetChild(0).GetComponent<Text>().text = unit.weapons[i - 1].name; // NAME
            ui.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = unit.weapons[i - 1].power.ToString(); // WEAPON POWER
            ui.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = unit.weapons[i - 1].rangeMin + "-" + unit.weapons[i - 1].rangeMax; // RANGE
            ui.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = (unit.weapons[i - 1].accuracy * 100).ToString(); // ACCURACY

            // BUTTON ENABLING:
            // TODO: grey out row when setting button inactive
            ui.transform.GetChild(6).gameObject.SetActive(true);

            if (!AoE)
            {
                ui.transform.GetChild(8).gameObject.SetActive(false);

                //if this weapon is not actionable and unit has moved, or the weapon is not in range, disable button and grey out

                if ((!unit.weapons[i - 1].actionable && unit.pos != unit.selectPos) ||
                     (!unit.weapons[i - 1].ContainsRange(distance)) || (unit.weapons[i - 1].AoE))
                {
                    ui.transform.GetChild(6).gameObject.SetActive(false); // disable button (4th child)
                    ui.transform.GetChild(8).gameObject.SetActive(true); // disable button (4th child)
                }
            }
            else // only add buttons to AoE weapons
            {
                if (!unit.weapons[i - 1].AoE)
                {
                    ui.transform.GetChild(6).gameObject.SetActive(false); // disable button (4th child)
                    ui.transform.GetChild(8).gameObject.SetActive(true); // disable button (4th child)
                }
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
                ui.transform.GetChild(ui.transform.childCount - 2).gameObject.SetActive(true);
            }
            else
            {
                ui.transform.GetChild(ui.transform.childCount - 2).gameObject.SetActive(false);
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

    public void Calculate(Unit unit, Unit other, ref int dmg, ref float acc)
    {
        if (unit == defender && !retaliation)
            return;

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
            // approximate def spectrum is 1-40 for now (reduce if too unbalanced)

            // y = mx^2 + b - use different curve if balance needs tweaking
            float maxMultiplier = 2.0f; // most dmg multiplier physical attacks can get (m in equation)
            float minMultiplier = 0.2f; // least dmg multiplier physical attacks can get (b in equation)
            float x; // final phys damage multiplier

            // TEMP: (move to GLOBAL or Unit)
            int maxStat = 40;
            int minStat = 1;

            float delta = unit.combatPhysAtk - other.combatDefense;

            // plot X on graph and move along curve depending on attacker physAtk/defender def disparity
            if (unit.combatPhysAtk <= maxStat / 2)
            {
                x = other.combatDefense - (delta); // on left side of grid, low def is closer to higher curve than high atk, so use def as anchor
                //x = unit.combatPhysAtk - (delta);  *****use this instead to make lower defense less punishing for infantry*****
                if (x < minStat)
                    x = minStat;
                else if (x > maxStat / 2)
                    x = maxStat / 2;
            }
            else
            {
                x = unit.combatPhysAtk + (delta); // on right side of grid, high atk is closer to the higher curve than low phys def, so use atk as anchor

                if (x < maxStat / 2)
                    x = maxStat / 2;
                else if (x > maxStat)
                    x = maxStat;
            }

            x = ((x - minStat) / (maxStat - minStat) - 0.5f) * 2; // normalize X between -1 and 1
            normalized = x; // DEBUG: public normalized x

            x = maxMultiplier * Mathf.Pow(x, 2) + minMultiplier; // plug normalized X into curve equation for final multiplier

            // I figured the curve equation would automatically clamp between min/max but it seems to go a bit outside...
            if (x < minMultiplier)
                x = minMultiplier;
            else if (x > maxMultiplier)
                x = maxMultiplier;

            dmg = (int)((unit.combatPhysAtk + unit.equipped.power - other.combatDefense) * x);
            //debug: public multiplier 
            multiplier = x;
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

        preCombat = false;

        // calculate battle results before depicting (TEMP: needs to be greatly expanded, especially if we implement multiple attacks)
        attackerHit = false;
        defenderHit = false;

        if (Random.Range(0, 100) <= attackerHitrate)
            attackerHit = true;
        if (retaliation && Random.Range(0, 100) <= defenderHitrate)
            defenderHit = true;

        // TEMP movement stuff       
        timer = 0;
        displacement = (GLOBAL.gridToWorld(defender.pos) - GLOBAL.gridToWorld(attacker.pos)).normalized;

        phase = CombatPhase.attackerLunge;
    }

    // TODO: IMPLEMENT QUICK HEALTH LOSS SEQUENCE (health bar drain)
    void Combat()
    {
        timer += Time.deltaTime;

        // TODO: implement strike animations (single image w/ movement)
        switch (phase)
        {
            case CombatPhase.attackerLunge:
                {
                    if (timer > 0.25f && timer < 0.5f)
                    {
                        attacker.transform.position += displacement * Time.deltaTime * GLOBAL.attackSpeed;
                    }
                    else if (timer >= 0.5f)
                    {
                        if (attackerHit)
                        {
                            defender.Damage(attackerDamage);
                            Camera.main.GetComponent<AudioSource>().PlayOneShot(attacker.equipped.sfx);
                        }
                        else
                        {
                            Camera.main.GetComponent<AudioSource>().PlayOneShot(GameDirector.Instance.sfxCancel); // TODO: miss SFX here
                        }

                        displacement = -displacement;

                        phase = CombatPhase.attackerRetreat;
                    }
                    break;
                }

            case CombatPhase.attackerRetreat:
                {
                    if (timer < 0.75f)
                    {
                        attacker.transform.position += displacement * Time.deltaTime * GLOBAL.attackSpeed;
                    }
                    else if (timer < 1.25f)
                    {
                        attacker.snapToGridPos();
                    }
                    else
                    {
                        if (defender.CheckDead())
                        {
                            Finish();
                        }
                        else
                        {
                            phase = CombatPhase.defenderLunge;
                        }
                        
                    }

                    break;
                }

            case CombatPhase.defenderLunge:
                {
                    if (timer < 1.5f)
                    {
                        defender.transform.position += displacement * Time.deltaTime * GLOBAL.attackSpeed;
                    }
                    else
                    {
                        if (defenderHit)
                        {
                            attacker.Damage(defenderDamage);
                            Camera.main.GetComponent<AudioSource>().PlayOneShot(defender.equipped.sfx);
                        }
                        else
                        {
                            Camera.main.GetComponent<AudioSource>().PlayOneShot(GameDirector.Instance.sfxCancel); // TODO: miss SFX here
                        }

                        displacement = -displacement;    

                        phase = CombatPhase.defenderRetreat;
                    }
                    

                    break;
                }

            case CombatPhase.defenderRetreat:
                {
                    if (timer < 1.75f)
                    {
                        defender.transform.position += displacement * Time.deltaTime * GLOBAL.attackSpeed;
                    }
                    else if (timer > 2.5f) 
                    {
                        defender.snapToGridPos();

                        attacker.CheckDead(); // process death for attacker

                        Finish();
                    }

                    break;
                }
        }

    }


    //===============================
    // Area of Effect:
    //===============================
    public void AoEWeaponSelect(Vector2i root)
    {
        AoEUI.SetActive(true);

        AoERoot = root;

        WeaponSelect(PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>(), true); // display weapon select menu
    }

    public void AoESelect(int num)
    {
        AoEUI.SetActive(false);

        AoEWeapon = PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>().weapons[num - 1];

        // if it's a non-directional weapon with no range, it's static
        if (AoEWeapon.rangeMax == 0 && !AoEWeapon.directional)
        {
            AoEWeapon.markAoEPattern(AoEWeapon.unit.pos);

            UIManager.Instance.activateAttackButton();  // activate confirm button for AoE attacks
        }
        // the weapon must be aimed; mark aimable tiles with purple markers
        else
        {
            TileMarker.Instance.markAoETiles(AoEWeapon);
        }
    }

    // called by attack button to instigate an AoE damage sequence
    public void AoEAttack()
    {
        //TileMarker.Instance.Clear();
        TileMarker.Instance.HideMarkers();

        // attacking unit has committed to this AoE weapon for the turn so equip it
        PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>().Equip(AoEWeapon);

        AoESequence = true;
        timer = 0;

        GLOBAL.setLock(true); // lock input during AoE sequence

        UIManager.Instance.setUnitUI(false); // disable unit UI (unit turn is done after attack sequence anyway)
        UIManager.Instance.deactivateConfirmButton();

        // play animation, offset by half of tilesize to accommodate center anchor
        if (AoEWeapon.AoEanim != null)
        {
            Instantiate(AoEWeapon.AoEanim, GLOBAL.gridToWorld(AoERoot) + new Vector3((int)(IntConstants.TileSize) / 2, (int)(IntConstants.TileSize) / 2), Quaternion.identity);
        }
        else
        {
            AoEWeapon.StartAoEAnim();
        }
    }

    // called by AoE animation's AnimationDestroyer on finish
    public void AoEDamage()
    {
        // use AoEWeapon and AoERoot to search appropriate tiles for units to damage
        // TODO: store previously marked attack tiles for quick processing (remove red markers but maintain coords)
        attacker.calcCombatStats();

        foreach(KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.attackTiles)
        {
            if (ObjectManager.Instance.ObjectGrid[tile.Key.x, tile.Key.y] != null)
            {
                if (ObjectManager.Instance.ObjectGrid[tile.Key.x, tile.Key.y].tag == "Unit")
                {
                    calcAoEDamage(ObjectManager.Instance.ObjectGrid[tile.Key.x, tile.Key.y].GetComponent<Unit>());
                }
            }
        }

        // END AoE SEQUENCE:
        FinishAoE();
    }

    public void calcAoEDamage(Unit other)
    {
        // instigating unit was stored in attacker variable at Unit.AoEBegin()
        other.calcCombatStats();
        Calculate(attacker, other, ref attackerDamage, ref attackerHitrate);

        // calculate battle results before depicting (TEMP: needs to be greatly expanded, especially if we implement multiple attacks)
        attackerHit = false;
        defenderHit = false;

        if (Random.Range(0, 100) <= attackerHitrate)
        {
            other.Damage(attackerDamage);
        }   
    }

    //=========================
    // Clean Up:
    //=========================

    // cleans up when combat sequence has finished
    public void Finish()
    {
        GLOBAL.setLock(false);

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

    // cleans up when AoE sequence has finished
    public void FinishAoE()
    {
        GLOBAL.setLock(false);

        AoESequence = false;
        PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>().Deactivate();
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

    // called when cancel button is pressed in the middle of an AoE attack
    public void CancelAoE()
    {
        AoEUI.SetActive(false);
    }

    public void EnableUI()
    {
        UI.SetActive(true);
    }

    public void DisableUI()
    {
        UI.SetActive(false);
    }
}
