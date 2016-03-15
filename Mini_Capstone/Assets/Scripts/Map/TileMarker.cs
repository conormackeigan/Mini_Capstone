using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TileMarker : Singleton<TileMarker>
{
    TerrainLayer terrain;
    public Dictionary<Vector2i, GameObject> travTiles; // keeps track of all marked traversible tiles by their pos
    public Dictionary<Vector2i, GameObject> attackTiles; // keeps track of all tiles that can currently be attacked

    // Sprites:
    public GameObject travMarker;
    public GameObject attackMarker;

    // Use this for initialization
    void Start()
    {
        terrain = TerrainLayer.Instance;

        travMarker = Resources.Load("Tiles/TravMarker") as GameObject;
        attackMarker = Resources.Load("Tiles/AttackMarker") as GameObject;

        travTiles = new Dictionary<Vector2i, GameObject>();
        attackTiles = new Dictionary<Vector2i, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // marks all tiles a unit can reach
    public void markTravTiles(Unit unit)
    {
        int range = unit.effectiveMovementRange;

        // if the unit has flying mark all traversible tiles within movement range        
        if (unit.flying)
        {
            for (int i = Mathf.Max(unit.Pos.y - range, 0); i <= Mathf.Min(unit.Pos.y + range, MapScript.Instance.Height); i++)
            {
                for (int j = Mathf.Max(unit.Pos.x - range, 0); j <= Mathf.Min(unit.Pos.x + range, MapScript.Instance.Width); j++)
                {
                    if (unit.Pos.Distance(j, i) <= range && terrain.Tiles[j, i].trav)
                    {
                        GameObject marker = Instantiate(travMarker, GLOBAL.gridToWorld(j, i), Quaternion.identity) as GameObject;
                        travTiles.Add(new Vector2i(j, i), marker);
                    }
                }
            }
        }

        // REAL TILE MARKING:
        else
        {
            // start by resetting all cost and parent info
            foreach (Tile t in TerrainLayer.Instance.Tiles)
            {
                t.cost = 10000;
                t.parent = null;
            }
           
            Tile start = TerrainLayer.Instance.Tiles[unit.Pos.x, unit.Pos.y];

            start.cost = 0; //we are already on this tile; costs nothing to get where you already are

            PriorityQueue<Tile> openList = new PriorityQueue<Tile>();
            openList.Add(start, start.cost);

            while (!openList.Empty())
            {
                Tile current = openList.delete_min();

                if (current != start && ObjectManager.Instance.ObjectGrid[current.pos.x, current.pos.y] == null && !travTiles.ContainsKey(current.pos))
                { //don't add root, occupied or duplicate tiles to traversible list
                    GameObject marker = Instantiate(travMarker, GLOBAL.gridToWorld(current.pos), Quaternion.identity) as GameObject;
                    travTiles.Add(current.pos, marker);
                }

                expand(openList, current, range);
            }
        }

    }

    private void expand(PriorityQueue<Tile> openList, Tile current, int range)
    {
        foreach (Vector2i n in current.neighbours)
        {
            Tile neighbour = TerrainLayer.Instance.Tiles[n.x, n.y];
            if (ObjectManager.Instance.ObjectGrid[neighbour.pos.x, neighbour.pos.y] != null)
            { // don't process tiles occupied by enemies
                if (ObjectManager.Instance.ObjectGrid[neighbour.pos.x, neighbour.pos.y].tag == "Unit")
                {
                    if (ObjectManager.Instance.ObjectGrid[neighbour.pos.x, neighbour.pos.y].GetComponent<Unit>().playerID != PlayerManager.Instance.getCurrentPlayer().playerID)
                    {
                        continue;
                    }
                }
            }

            if (neighbour.cost > current.cost + neighbour.weight)
            {
                neighbour.cost = current.cost + neighbour.weight;
                if (neighbour.cost <= range)
                {
                    neighbour.parent = current;
                    openList.AddOrUpdate(neighbour, neighbour.cost);
                }
            }
        }
    }


    // marks tiles a unit can attack in red
    public void markAttackTiles(Unit unit)
    {
        // if unit doesn't have any weapons gtfo of here
        if (unit.weapons.Count == 0)
        {
            return;
        }

        List<int> ranges = new List<int>(); // all attackable ranges
        int max = 0; // the maximum range of attack, to reduce number of tiles to process

        // get all attackable ranges
        foreach (Weapon w in unit.weapons)
        {
            if (!w.actionable && unit.state != Unit.UnitState.Selected)
            { // if the weapon is not actionable, it can only be used at beginning of turn
                continue;
            }

            for (int i = w.rangeMin; i <= w.rangeMax; i++)
            {
                if (i > max)
                {
                    max = i;
                }

                if (!ranges.Contains(i))
                {
                    ranges.Add(i);
                }
            }
        }

        // mark all tiles x range from unit as long as x is contained in ranges
        for (int i = Mathf.Max(unit.Pos.y - max, 0); i <= Mathf.Min(unit.Pos.y + max, MapScript.Instance.Height); i++)
        {
            for (int j = Mathf.Max(unit.Pos.x - max, 0); j <= Mathf.Min(unit.Pos.x + max, MapScript.Instance.Width); j++)
            {
                // if distance from processing tile to unit is in ranges, mark it
                if (ranges.Contains(unit.Pos.Distance(j, i)))
                {
                    GameObject marker = Instantiate(attackMarker, GLOBAL.gridToWorld(j, i), Quaternion.identity) as GameObject;
                    attackTiles.Add(new Vector2i(j, i), marker);
                }
            }
        }



    }


    public void Clear()//bool trav = true)
    {
        foreach (KeyValuePair<Vector2i, GameObject> entry in travTiles)
        {
            Destroy(entry.Value);
        }
        travTiles.Clear();
        
        foreach (KeyValuePair<Vector2i, GameObject> entry in attackTiles)
        {
            Destroy(entry.Value);
        }
        attackTiles.Clear();
    }

    // calls clear and mark trav/attack tiles
    public void Redraw(Unit u)
    {
        Clear();
        markTravTiles(u);
        markAttackTiles(u);
    }


}
