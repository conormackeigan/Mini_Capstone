using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//wrapper class for grid node data
public abstract class Tile
{
    //=================
    // Attributes:
    //=================
    public Vector2i pos;

    // Pathfinding:
    public int cost; // current cheapest cost to travel to this node
    public List<Vector2i> neighbours; // all adjacent cells
    public Tile parent; // previous node in shortest path to this tile

    //=================
    // Properties:
    //=================
    public abstract string tileType { get; } // used for Resource.Load()ing sprite  
    public abstract int weight { get; } // cost to traverse this tile
    //public abstract Dictionary<string, int> weightExceptions { get; } // string = unit type, int = cost to traverse this tile for that type of unit

    public abstract bool gTrav { get; } //whether this tile can be traversed by ground
    public abstract bool wTrav { get; } //whether this tile can be traversed by water
    public abstract bool trav { get; } //whether this tile can be traversed at all (for air units)

    public abstract int def { get; } //defense multiplier this tile provides to the unit that occupies it
    public abstract int eva { get; } //evasion multiplier this tile provides to the unit that occupies it

    //=================
    // Initialization:
    //=================
    public Tile(Vector2i position)
    {
        pos = position;

        // add neighbours
        neighbours = new List<Vector2i>();
        parent = null;

        if (pos.x > 0)
        {
            neighbours.Add(new Vector2i(pos.x - 1, pos.y));
        }
        if (pos.x < MapScript.Instance.Width - 1)
        {
            neighbours.Add(new Vector2i(pos.x + 1, pos.y));
        }
        if (pos.y > 0)
        {
            neighbours.Add(new Vector2i(pos.x, pos.y - 1));
        }
        if (pos.y < MapScript.Instance.Height - 1)
        {
            neighbours.Add(new Vector2i(pos.x, pos.y + 1));
        }

        cost = 10000;
    }
}
