using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Structure : MonoBehaviour {

    //----------------------------------------------------------------------------------------------------------------------
    // Attributes
    //----------------------------------------------------------------------------------------------------------------------

    public string structureName; // Name of structure
    public string structurePrefabName; // Name of prefab used by structure
    protected GameObject structuePrefab; // Prefab used by structure

    //TODO: Protected???
    public Player player; // Player controlling structure
    public List<string> units; // Available units for structure to build
    public List<string> unitPrefabs; // Available units for structure to build

    public bool currentlySelected = false;

    public int health;
    public int defense;

    //private MapScript map; //reference to the map
    //private TerrainLayer terrainLayer; //reference to terrain layer
    //private UnitLayer unitLayer; //reference to unit layer

    private Vector2i pos; //this unit's position in grid coords
    public Vector2i Pos { get { return pos; } set { pos = value; } }
    public Sprite sprite; //this will be an animation controller; static sprite for now

    //----------------------------------------------------------------------------------------------------------------------
    // Constructors
    //----------------------------------------------------------------------------------------------------------------------

    public Structure()
    {
    }

    public Structure(Vector2i position)
    {
        pos = position;
    }

    //----------------------------------------------------------------------------------------------------------------------
    // Methods
    //----------------------------------------------------------------------------------------------------------------------

    //====================
    // Virtual Functions
    //====================
    protected virtual void Start()
    {
        //map = GameObject.Find("Map").GetComponent<MapScript>();
        //terrainLayer = GameObject.Find("TerrainLayer").GetComponent<TerrainLayer>();
        //unitLayer = GameObject.Find("UnitLayer").GetComponent<UnitLayer>();

        //assign sprite to sprite renderer component
        if (sprite != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnGUI()
    {

    }

    //================
    // Grid Functions
    //================

    //forces unit's coordinates immediately to grid position
    public void snapToGridPos()
    {
        transform.position = new Vector3(pos.x * (int)IntConstants.TileSize, pos.y * (int)IntConstants.TileSize, 0);
    }
}
