using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : Singleton<ObjectManager>
{

    //----------------------------------------------------------------------------------------------------------------------
    // Attributes
    //----------------------------------------------------------------------------------------------------------------------

    // List of units belonging to each player
    public List<GameObject> PlayerOneUnits;
    public List<GameObject> PlayerTwoUnits;
    public List<List<GameObject>> playerUnits; // first index = player num - 1

    // The gameobject grid (used for unit & structure placement)
    private GameObject[,] objectGrid;
    public GameObject[,] ObjectGrid { get { return objectGrid; } }

    // Grid properties
    private int tileSize = (int)IntConstants.TileSize;
    private MapScript map;

    //----------------------------------------------------------------------------------------------------------------------
    // Methods
    //----------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        map = transform.parent.GetComponent<MapScript>(); //get reference to map parent
        objectGrid = new GameObject[map.Width, map.Height];
        clearGrid(); // Initialize unit grid

        

        PlayerOneUnits = new List<GameObject>();
        PlayerTwoUnits = new List<GameObject>();
        playerUnits = new List<List<GameObject>>();

        playerUnits.Add(PlayerOneUnits);
        playerUnits.Add(PlayerTwoUnits);
    }

    void Update()
    {
    }

    public void endGame()
    {
        map = transform.parent.GetComponent<MapScript>(); //get reference to map parent
        clearGrid(); // Initialize unit grid

        foreach (List<GameObject> units in ObjectManager.Instance.playerUnits)
        {
            foreach (GameObject unit in units)
            {
                unit.GetComponent<Unit>().destroyUnit();
            }

        }

        PlayerOneUnits = new List<GameObject>();
        PlayerTwoUnits = new List<GameObject>();
        playerUnits = new List<List<GameObject>>();

        UIManager.Instance.DeactivateFriendPanel();
        UIManager.Instance.DeactivateEnemyPanel();

    }

    //initializes grid
    void clearGrid()
    {
        for (int i = 0; i < objectGrid.GetLength(0); i++)
        {
            for (int j = 0; j < objectGrid.GetLength(1); j++)
            {
                if(objectGrid[i, j] != null)
                {
                    Destroy(objectGrid[i, j]);
                }
                objectGrid[i, j] = null;
            }
        }
    }

    // Adds a given game object to the object grid
    public void addObjectAtPos(GameObject obj, Vector2i pos)
    {
        // Create the provided unit type at the given position
        Debug.Assert(obj != null);
        Debug.Assert(objectGrid[pos.x, pos.y] == null, "ADDING OBJECT TO NON-EMPTY SPACE");

        if (obj.tag == "Unit")
        {
            objectGrid[pos.x, pos.y] = obj;

            Unit unitScript = obj.GetComponent<Unit>();
            unitScript.Pos = pos;
            unitScript.snapToGridPos();

            // Add to unit list
            if(unitScript.playerID == 1)
            {
                PlayerOneUnits.Add(obj);
            }
            else if (unitScript.playerID == 2)
            {
                PlayerTwoUnits.Add(obj);
            }
        }
        else if (obj.tag == "Structure")
        {
            Debug.Log("IMPLEMENT STRUCTURES");
        }
    }

    // Removes the object from the grid
    public void removeObjectAtPos(Vector2i pos)
    {
        Destroy(objectGrid[pos.x, pos.y]);
        objectGrid[pos.x, pos.y] = null;
    }

    // Moves a given unit to the given position on the object grid
    public void moveUnitToGridPos(GameObject unitObj, Vector2i newPos)
    {
        Debug.Assert(unitObj.tag == "Unit", "MOVING A NON UNIT");

        Unit unitScript = unitObj.GetComponent<Unit>();

        Debug.Assert(objectGrid[unitScript.Pos.x, unitScript.Pos.y] == unitObj, "ERROR: MOVING UNIT THAT DOES NOT EXIST");

        if(unitScript.state == Unit.UnitState.Inactive)
        {
            return;
        }

        // Remove unit in current position
        objectGrid[unitScript.Pos.x, unitScript.Pos.y] = null;
        objectGrid[newPos.x, newPos.y] = unitObj;

        unitScript.Pos = newPos;
        unitScript.snapToGridPos();

        //TileMarker.Instance.Clear(false); // remove trav tiles and old attack tiles
        //TileMarker.Instance.markAttackTiles(unitScript);
    }

    public void EndTurnForPlayer(int p)
    {
        // Check if turn should automatically end (all actions used)
        bool nextTurn = true;
        if(p == 1)
        {
            for(int i = 0; i < PlayerOneUnits.Count; i++)
            {
                if (PlayerOneUnits[i].GetComponent<Unit>().state != Unit.UnitState.Inactive)
                {
                    nextTurn = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < PlayerTwoUnits.Count; i++)
            {
                if (PlayerTwoUnits[i].GetComponent<Unit>().state != Unit.UnitState.Inactive)
                {
                    nextTurn = false;
                }
            }
        }

        if(nextTurn)
        {
            ResetToActive(p);
            PlayerManager.Instance.nextTurn();
        }
    }

    public void ResetToActive(int p)
    {
        if (p == 1)
        {
            for (int i = 0; i < PlayerOneUnits.Count; i++)
            {
                PlayerOneUnits[i].GetComponent<Unit>().state = Unit.UnitState.Neutral; //isActive = true;
            }
        }
        else
        {
            for (int i = 0; i < PlayerTwoUnits.Count; i++)
            {
                PlayerTwoUnits[i].GetComponent<Unit>().state = Unit.UnitState.Neutral; //isActive = true;
            }
        }
    }

    public GameObject FindDead()
    {
        for (int i = 0; i < PlayerOneUnits.Count; i++)
        {
            if (PlayerOneUnits[i].GetComponent<Unit>().isDead== true)
            {
                Debug.Log("Found DEAD");

                return PlayerOneUnits[i].gameObject;
            }
        }

        for (int i = 0; i < PlayerTwoUnits.Count; i++)
        {
            if (PlayerTwoUnits[i].GetComponent<Unit>().isDead == true)
            {
                Debug.Log("Found DEAD");

                return PlayerTwoUnits[i].gameObject;
            }
        }

        return null;
    }

    public bool isGameOver()
    {
        bool playerOneWin = true;
        bool playerTwoWin = true;

        for (int i = 0; i < PlayerOneUnits.Count; i++)
        {
            if (PlayerOneUnits[i].GetComponent<Unit>().isDead == false)
            {
                playerTwoWin = false;
            }
        }

        for (int i = 0; i < PlayerTwoUnits.Count; i++)
        {
            if (PlayerTwoUnits[i].GetComponent<Unit>().isDead == false)
            {
                playerOneWin = false;
            }
        }

        return playerOneWin || playerTwoWin;
    }

}
