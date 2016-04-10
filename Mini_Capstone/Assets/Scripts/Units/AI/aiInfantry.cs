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
                        int dist = unit.pos.Distance(other.pos);
                        if (dist >= w.rangeMin && dist <= w.rangeMax)
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
                            if (objectGrid[i, j] != null && tile.Key.Distance(new Vector2i(i, j)) >= w.rangeMin)
                            {
                                if (objectGrid[i, j].GetComponent<Unit>().playerID == 1)
                                {
                                    Pair<Unit, Weapon> pair = new Pair<Unit, Weapon>(objectGrid[i, j].GetComponent<Unit>(), w);
                                    if (!possibleAttacks.Contains(pair))
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
            //Chase();
            Wait();

            return;
        }

        // next case is to process possible attacks and either attack or chase (if attacks are all unfavourable)
        else if (possibleAttacks.Count > 0)
        {
            PrioritizeAttacks();
        }

        else
        {
            Wait();
        }
    }


    // NOTE: only takes into account unit's currently equipped weapon; not smart enough to process all potential equips yet
    public void PrioritizeAttacks()
    {
        // put unit in combat phase temporarily for calculations (will persist if attacks commence)
        unit.state = Unit.UnitState.Combat;
        CombatSequence.Instance.attacker = unit;
        int counter = 0;
        // sort possibleAttacks list by priority
        foreach (Pair<Unit, Weapon> attack in possibleAttacks)
        {
            // put target in combat phase during calculation for combat skills
            attack.first.state = Unit.UnitState.Combat;

            unit.Equip(attack.second);

            Unit defender = attack.first;

            int priority = 100;

            int dmgOut = 0;
            int dmgIn = 0;
            float accOut = 0;
            float accIn = 0;

            
            CombatSequence.Instance.Calculate(unit, defender, ref dmgOut, ref accOut);

            CombatSequence.Instance.Calculate(defender, unit, ref dmgIn, ref accIn);
            Debug.Log("DMG OUT: " + dmgOut + " DMG IN: " + dmgIn);

            counter++;
            Debug.Log(counter);
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
            //priority -= (int)(75 - accOut);
            //priority += (int)((75 - accIn) * 0.5f);

            attackPriority.Add(attack, priority);

            // revert target to inactive state in case we don't attack it
            attack.first.state = Unit.UnitState.NotTurn;
        }

        unit.Equip(origWep);

        // priority queue populated, check if we can attack or if we have to move first
        // DEBUG: print priority queue in order
        for (int i = 0; i < attackPriority.data.Count; i++)
        {
            Debug.Log(attackPriority.data[i].Value.second.name + " " + attackPriority.data[i].Key);
        }

        // if highest priority attack is <= 0, do not attack
        if (attackPriority.frontPriority() <= 0)
        {
            // TODO: implement flee behaviour (not in conventional FE games and could be frustrating to player but what a "smart" player would do)
            // for now just wait
            Wait();

            return;
        }

        Unit target = attackPriority.back().first;
        Weapon weapon = attackPriority.back().second;

        // if attacking weapon is not actionable, attack now
        if (!weapon.actionable)
        {
            Attack(target, weapon);
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

    }


    public void Wait()
    {
        UIManager.Instance.ConfirmAction();

        AIManager.Instance.callNextUnit();
    }

} // close class
