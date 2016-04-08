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

    public aiInfantry(Unit u)
    {
        unit = u;
        possibleAttacks = new List<Pair<Unit, Weapon>>();
    }

    // called when this computer unit's turn starts
    public override void StartTurn()
    { 
        // turn is always started by selecting unit to mark tiles
        unit.OnMouseClick();

        // now get add all units in marked attack tiles and add to list
        GameObject[,] objectGrid = ObjectManager.Instance.ObjectGrid;

        foreach(KeyValuePair<Vector2i, GameObject> tile in TileMarker.Instance.attackTiles)
        {
            if (objectGrid[tile.Key.x, tile.Key.y] != null)
            {
                Unit other = objectGrid[tile.Key.x, tile.Key.y].GetComponent<Unit>();
                if (other.playerID == 1)
                {
                    // get all weapons in range of this unit
                    foreach(Weapon w in unit.weapons)
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
                    for (int i = tile.Key.x - w.rangeMax; i <= tile.Key.x + w.rangeMax; i++)
                    {
                        for (int j = tile.Key.y - w.rangeMax; j <= tile.Key.y + w.rangeMax; i++)
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

        // first break case is no possible attacks and no possible movements, so wait
        if (possibleAttacks.Count == 0 && TileMarker.Instance.travTiles.Count == 0)
        {
            UIManager.Instance.ConfirmAction();

            EndTurn();
            return;
        }



    } // close StartTurn()


    public override void EndTurn()
    {
        Unit unit = aiManager.Instance.getNext();

        if (unit != null)
        {
            unit.AI.StartTurn();
        }
    }

} // close class
