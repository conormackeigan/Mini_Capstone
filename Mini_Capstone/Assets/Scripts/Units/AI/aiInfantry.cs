using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// AI class owned by computer units
// TODO: implement macro to better process possibleAttacks and their possible positions
public class aiInfantry : aiBase
{
    private Unit unit; // the unit this AI belongs to

    private List<Pair<Unit, Weapon>> possibleAttacks; // units that can be reached with an attack this turn
    private PriorityQueue<Pair<Unit, Weapon>> attackPriority; // sorts attacks by priority (can attack kill? chance of hitting/dodging? how much damage taken in retaliation? all factors)
    private Pair<Unit, Weapon> storedAttack; // storing attack for post-move call

    private PriorityQueue<Vector2i> movePriority; // sorts all potential movements (pre-combat or not)

    private Weapon origWep; // store original weapon since we need to equip all weapons when processing their attack priorities


    public aiInfantry(Unit u)
    {
        unit = u;
        possibleAttacks = new List<Pair<Unit, Weapon>>();
        attackPriority = new PriorityQueue<Pair<Unit, Weapon>>();
        movePriority = new PriorityQueue<Vector2i>();
    }


    // called when this computer unit's turn starts
    public override void StartTurn()
    {
        // clear all lists
        possibleAttacks.Clear();
        attackPriority.Clear();
        movePriority.Clear();

        storedAttack = null;

        // store currently equipped weapon for later in case of reset
        origWep = unit.equipped;

        // turn is always started by selecting unit to mark tiles
        unit.OnMouseClick();

        // now get add all units in marked attack tiles and add to list
        GameObject[,] objectGrid = ObjectManager.Instance.ObjectGrid;

        foreach (KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.attackTiles)
        {
            if (objectGrid[tile.Key.x, tile.Key.y] != null)
            {
                Unit other = objectGrid[tile.Key.x, tile.Key.y].GetComponent<Unit>();

                if (other.playerID == 1)
                {
                    // get all weapons in range of this unit
                    foreach (Weapon w in unit.weapons)
                    {
                        // TEMP: AI cannot use AoE for now (too much extra calculations for time constraints)
                        if (w.ContainsRange(unit.pos.Distance(other.pos)) && !w.AoE)
                        {
                            possibleAttacks.Add(new Pair<Unit, Weapon>(other, w));
                        }
                    }
                }
            }
        }

        // ACTIONABLE WEAPONS POST-MOVE
        foreach (Weapon w in unit.weapons)
        {
            if (w.actionable)
            {
                // find all units attackable within this unit's movement range (with actionable weps)
                foreach (KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.travTiles)
                {
                    for (int i = Mathf.Max(tile.Key.x - w.rangeMax, 0); i <= Mathf.Min(tile.Key.x + w.rangeMax, MapScript.Instance.mapWidth - 1); i++)
                    {
                        for (int j = Mathf.Max(tile.Key.y - w.rangeMax, 0); j <= Mathf.Min(tile.Key.y + w.rangeMax, MapScript.Instance.mapHeight - 1); j++)
                        {
                            if (objectGrid[i, j] != null && tile.Key.Distance(new Vector2i(i, j)) >= w.rangeMin && tile.Key.Distance(new Vector2i(i, j)) <= w.rangeMax)
                            {
                                if (objectGrid[i, j].GetComponent<Unit>().playerID == 1)
                                {
                                    Pair<Unit, Weapon> pair = new Pair<Unit, Weapon>(objectGrid[i, j].GetComponent<Unit>(), w);

                                    bool dupe = false; // not a duplicate until we prove it is
                                    // didn't set up a comparer for the Pair type so here's a jerry-rig
                                    foreach (Pair<Unit, Weapon> p in possibleAttacks)
                                    {
                                        if (pair.first == p.first && pair.second == p.second)
                                        {
                                            dupe = true;
                                        }
                                    }
                                    if (!dupe)
                                    {
                                        possibleAttacks.Add(pair);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        // all possible attacks are now stored in possibleAttacks. analyze which one is the most efficient, and make unit move to appropriate space and/or attack
        AIManager.Instance.callAction(unit);

    } // close StartTurn()


    // called after one second delay of unit selection. attacks, moves+attacks, or waits if no attacks
    public override void SelectAction()
    {
        // first break case is no possible attacks and no possible movements, so wait
        if (possibleAttacks.Count == 0 && TileMarker.Instance.travTiles.Count == 0)
        {
            Wait();

            return;
        }

        // next case is no possible attacks, if so figure out which space is ideal
        else if (possibleAttacks.Count == 0 && TileMarker.Instance.travTiles.Count != 0)
        {
            // iterate through all player units and decide which unit to pursue
            Chase();
            //Wait(); //tmp

            return;
        }

        // next case is to process possible attacks and either attack or chase (if attacks are all unfavourable)
        else if (possibleAttacks.Count > 0)
        {
            PrioritizeAttacks();

            // priority queue populated, check if we can attack or if we have to move first
            // DEBUG: print priority queue in order
            //for (int i = 0; i < attackPriority.data.Count; i++)
            //{
            //    Debug.Log(attackPriority.data[i].Value.second.name + " " + attackPriority.data[i].Key);
            //}

            // if highest priority attack is <= 0, do not attack
            if (attackPriority.backPriority() <= 0)
            {
                // TODO: implement flee behaviour (not in conventional FE games and could be frustrating to player but what a "smart" player would do)
                // for now just wait
                Wait();

                return;
            }

            Unit target = attackPriority.back().first;
            Weapon weapon = attackPriority.back().second;
            
            // if attacking weapon is not actionable (or no possible movements), attack now
            if (!weapon.actionable || TileMarker.Instance.travTiles.Count == 0)
            {
                Attack(target, weapon);
            }
            else
            {
                if (weapon.actionable)
                {
                    // find best spot to attack from
                    FindAttackPoint(attackPriority.back());
                }
            }
        }

        else
        {
            Debug.Log("You wandered into the wrong neighbourhood");
            Wait();
        }
    }

    // adds worst case attack (enemy's best weapon) to possibleAttacks
    public void CalculateAttackPriority(Pair<Unit, Weapon> attack, bool move = false)
    {
        Unit defender = attack.first;
        defender.state = Unit.UnitState.Combat;

        // process all of target unit's weapons in range and overwrite with worst case for us
        foreach (Weapon w in defender.weapons)
        {
            if (!attack.second.ContainsRange(defender.pos.Distance(unit.pos)))
            {
                continue; // can't reach them with this weapon from this pos
            }

            int priority = 100;

            bool retaliation = true; // set to false if this attack won't be counterattacked

            if (!w.ContainsRange(defender.pos.Distance(unit.pos)))
            {
                priority += 35;

                retaliation = false;
            }

            defender.Equip(w);

            int dmgOut = 0;
            int dmgIn = 0;
            float accOut = 0;
            float accIn = 0;

            // reset combatsequence variables for offset additions
            CombatSequence.Instance.attackerDamage = 0;
            CombatSequence.Instance.defenderDamage = 0;
            CombatSequence.Instance.attackerHitrate = 0;
            CombatSequence.Instance.defenderHitrate = 0;

            CombatSequence.Instance.Calculate(unit, defender, ref dmgOut, ref accOut);

            if (retaliation)
            {
                CombatSequence.Instance.defender = defender;
                CombatSequence.Instance.retaliation = true; // weapon is in range so retaliation guaranteed (TODO: cost checks)
                CombatSequence.Instance.Calculate(defender, unit, ref dmgIn, ref accIn);

                dmgIn += CombatSequence.Instance.defenderDamage;
                accIn += CombatSequence.Instance.defenderHitrate;
            }

            // apply combat skill offsets
            dmgOut += CombatSequence.Instance.attackerDamage;
            accOut += CombatSequence.Instance.attackerHitrate;


            //==========================================================
            // ARCANE PRIORITY CALCULATIONS (prepare for nonsense)
            //==========================================================

            // KILL OFFSET:
            if (defender.effectiveHealth - dmgOut <= 0)
            {
                priority += 50; // large priority boost for kill attacks
            }
            else if (unit.effectiveHealth - dmgIn <= 0)
            {
                priority -= 20; // medium priority loss for being killed in retaliation
            }

            // DAMAGE OFFSET:
            if (dmgOut <= 0)
            {
                priority -= 1000; // not dealing any damage guarantees neg priority (do not attack)
            }
            else
            {
                priority += dmgOut * 2;
            }

            if (dmgIn <= 0)
            {
                priority += 50; // large priority boost for not taking damage in retaliation
            }
            else
            {
                priority -= dmgIn; // slight priority loss for more dmg taken (half magnitude of damage given)
            }

            // ACCURACY OFFSET:
            // 75% acc = no offset
            priority -= (int)(75 - accOut);
            priority += (int)((75 - accIn) * 0.5f);

            attackPriority.AddOrUpdateIfLower(attack, priority); // store the lowest priority attack (worst case) for each enemy unit (their best weapon)   
            //attackPriority.AddOrUpdateIfHigher(attack, priority); //tmp DEBUG

            //attackPriority.AddOrUpdateIfLower(attack, priority); // store the lowest priority attack (worst case) for each enemy unit (their best weapon)
            if (!retaliation)
            {
                attackPriority.AddOrUpdateIfHigher(attack, priority);
            }
            // non-retaliation attacks take precedence. retaliated attacks need to account for the player equipping the best possible weapon
            else
            {
                attackPriority.AddOrUpdateIfLower(attack, priority);
            }
        }

        // revert target to inactive state in case we don't attack it
        defender.state = Unit.UnitState.NotTurn;
    }


    public void PrioritizeAttacks()
    {
        // put unit in combat phase temporarily for calculations (will persist if attacks commence)
        unit.state = Unit.UnitState.Combat;
        CombatSequence.Instance.attacker = unit;

        Vector2i originalPos = unit.pos; // need to move unit around to process attacks so store original pos

        // sort possibleAttacks list by priority
        foreach (Pair<Unit, Weapon> attack in possibleAttacks)
        {
            unit.Equip(attack.second);

            foreach (KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.travTiles)
            {
                unit.pos = tile.Key;
                CalculateAttackPriority(attack);
            }

            unit.pos = originalPos;
        }

        unit.Equip(origWep);
    }


    // committed to weapon/target; find best position to attack from
    public void FindAttackPoint(Pair<Unit, Weapon> attack)
    {
        storedAttack = attack; // attack call is external so store in class-scope variable

        // set up units for combat calculations
        Unit target = attack.first;

        unit.state = Unit.UnitState.Combat;
        CombatSequence.Instance.attacker = unit;

        unit.Equip(attack.second); // committed to this weapon so equip before iterating

        Vector2i originalPos = unit.pos; // need to move unit around to test different attack points

        // process not-moving special case since initial tile is not marked in tilemarker
        if (unit.equipped.ContainsRange(unit.pos.Distance(target.pos)))
        {
            // process all possible weapon exchanges with committed weapon vs all of enemy's and store worst case as tile priority
            attackPriority.Clear();
            CalculateAttackPriority(attack);
            movePriority.Add(unit.pos, attackPriority.backPriority());
        }

        foreach (KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.travTiles)
        {
            if (!unit.equipped.ContainsRange(tile.Key.Distance(target.pos)))
            {
                continue; // don't process tiles the chosen weapon can't reach
            }

            unit.pos = tile.Key;

            // process all possible weapon exchanges with committed weapon vs all of enemy's and store worst case as tile priority
            attackPriority.Clear();
            CalculateAttackPriority(attack);
            movePriority.Add(unit.pos, attackPriority.backPriority());
        }

        // sorted all tiles by how effective the attack is from their vantage, pick the highest priority
        Vector2i targetPos = movePriority.back();
        unit.pos = originalPos;

        TerrainLayer.Instance.tileObjects[targetPos.x, targetPos.y].GetComponent<TileResponder>().OnMouseClick();
    }


    // called by unit when finished traversing path
    public override void ReachedDestination()
    {
        if (storedAttack != null)
        {
            Attack(storedAttack.first, storedAttack.second);
        }
        else
        {
            Wait();
        }
    }


    // if the target's coord isn't marked with an attack tile when this is called something goofed
    public void Attack(Unit target, Weapon weapon)
    {
        if (unit.equipped != weapon)
        {
            unit.Equip(weapon);
        }

        target.OnMouseClick();
    }


    // computes which unit to chase (in the case of no attacks)
    public void Chase()
    {
        //
    }


    public void Wait()
    {
        UIManager.Instance.ConfirmAction();

        AIManager.Instance.callNextUnit();
    }

} // close class
