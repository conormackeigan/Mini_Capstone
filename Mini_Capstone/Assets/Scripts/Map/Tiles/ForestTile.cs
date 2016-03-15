using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 
public class ForestTile : Tile
{
    public override string tileType { get { return "Forest"; } }
    public override int weight { get { return 2; } }
    //public override Dictionary<string, int> weightExceptions 
    public override bool gTrav { get { return true; } }
    public override bool wTrav { get { return false; } }
    public override bool trav { get { return true; } }
    public override int def { get { return 10; } }
    public override int eva { get { return 10; } }

    //=================
    // Initialization:
    //=================
    public ForestTile(Vector2i position) : base(position)
    {
    }
}
