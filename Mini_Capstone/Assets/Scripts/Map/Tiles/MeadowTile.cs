using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeadowTile : Tile
{
    public override string tileType { get { return "Meadow"; } }
    public override int weight { get { return 1; } }
    //public override Dictionary<string, int> weightExceptions 
    public override bool gTrav { get { return true; } }
    public override bool wTrav { get { return false; } }
    public override bool trav { get { return true; } }
    public override int def { get { return 0; } }
    public override int eva { get { return 0; } }

    //=================
    // Initialization:
    //=================
    public MeadowTile(Vector2i position) : base(position)
    {
    }
}
