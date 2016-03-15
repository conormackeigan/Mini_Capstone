using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 
public class UntraversableTile : Tile
{
    public override string tileType { get { return "Untraversable"; } }
    public override int weight { get { return 1000; } }
    //public override Dictionary<string, int> weightExceptions 
    public override bool gTrav { get { return false; } }
    public override bool wTrav { get { return false; } }
    public override bool trav { get { return false; } }
    public override int def { get { return -1000; } }
    public override int eva { get { return -1000; } }

    //=================
    // Initialization:
    //=================
    public UntraversableTile(Vector2i position) : base(position)
    {
    }
}
