using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TileMarker : Singleton<TileMarker>
{
    TerrainLayer terrain;
    public Dictionary<Vector2i, GameObject> travTiles; // keeps track of all marked traversible tiles by their pos
    public Dictionary<Vector2i, GameObject> attackTiles; // keeps track of all tiles that can currently be attacked
    public Dictionary<Vector2i, GameObject> AoETiles; // keeps track of all purple tiles (AoE aimers)

    // Sprites:
    public GameObject travMarker;
    public GameObject attackMarker;
    public GameObject AoEMarker;

    // Use this for initialization
    void Start()
    {
        terrain = TerrainLayer.Instance;

        travMarker = Resources.Load("Tiles/TravMarker") as GameObject;
        attackMarker = Resources.Load("Tiles/AttackMarker") as GameObject;
        AoEMarker = Resources.Load("Tiles/AoEMarker") as GameObject;

        travTiles = new Dictionary<Vector2i, GameObject>();
        attackTiles = new Dictionary<Vector2i, GameObject>();
        AoETiles = new Dictionary<Vector2i, GameObject>();
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
        List<int> ranges = new List<int>(); // all attackable ranges
        int max = 0; // the maximum range of attack, to reduce number of tiles to process

        // get all attackable ranges
        foreach (Weapon w in unit.weapons)
        {
            if (!w.actionable && unit.state != Unit.UnitState.Selected)
            { // if the weapon is not actionable, it can only be used at beginning of turn
                continue;
            }

            if (w.AoE)
            { // AoE weapons are handled through the AoE button, not unit selecting
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
                    addAttackTile(new Vector2i(j, i));
                }
            }
        }
    }

    public void addAttackTile(Vector2i pos)
    {
        GameObject marker = Instantiate(attackMarker, GLOBAL.gridToWorld(pos), Quaternion.identity) as GameObject;
        attackTiles.Add(pos, marker);
    }

    public void addAoETile(Vector2i pos)
    {
        GameObject marker = Instantiate(AoEMarker, GLOBAL.gridToWorld(pos), Quaternion.identity) as GameObject;
        AoETiles.Add(pos, marker);
    }

    //===============================
    // Area of Effect Tile Marking:
    //===============================
    public void markAoETiles(Weapon weapon)
    {
        // if the weapon is non-directional, apply its pattern to all tiles within its ranges
        if (!weapon.directional)
        {
            // y first (bottom-left to top-right)
            for (int i = Mathf.Max(weapon.unit.pos.y - weapon.rangeMax, 0); i <= Mathf.Min(weapon.unit.pos.y + weapon.rangeMax, MapScript.Instance.Height); i++)
            {
                for (int j = Mathf.Max(weapon.unit.pos.x - weapon.rangeMax, 0); j <= Mathf.Min(weapon.unit.pos.x + weapon.rangeMax, MapScript.Instance.Width); j++)
                {
                    if (weapon.unit.pos.Distance(j, i) >= weapon.rangeMin && weapon.unit.pos.Distance(j, i) <= weapon.rangeMax)
                    {
                        addAoETile(new Vector2i(j, i));
                    }
                }
            }
        }

        else // Directional Weapon:
        {

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

        foreach (KeyValuePair<Vector2i, GameObject> entry in AoETiles)
        {
            Destroy(entry.Value);
        }
        AoETiles.Clear();
    }

    // calls clear and mark trav/attack tiles
    public void Redraw(Unit u)
    {
        Clear();
        markTravTiles(u);
        markAttackTiles(u);
    }

    // removes all red tiles but does not clear attackTiles list
    public void HideMarkers()
    {
        foreach (KeyValuePair<Vector2i, GameObject> entry in attackTiles)
        {
            Destroy(entry.Value);
        }
    }
}
