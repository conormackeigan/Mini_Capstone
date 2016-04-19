using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    public enum PLAYER_TYPE
    {
        Human = 0,
        Computer
    }

    // Shared Player Attributes
    public PLAYER_TYPE playerType;
    public string playerName;
    public int playerID;
    public Color playerColor;

    public GameObject selectedObject;

	// Use this for initialization
	void Start ()
    {


        selectedObject = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void initPlayer(string name, int id, Color c, bool network, PLAYER_TYPE type)
    {      
        playerType = type;
        playerName = name;
        playerID = id;
        playerColor = c;

        selectedObject = null;
    }

    public void startBoardWithUnits(List<GameObject> units)
    {
        // AI controlled units
        // TODO: create a custom selection of different unit types that are within the ticket maximum (static for current scope)
        if (GameDirector.Instance.isSinglePlayer())
        {
            GameObject unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            uInfantry script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 14));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 8));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(15, 14));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 16));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(13, 16));
        }

        // Locations to spawn on map
        int posX = 0;
        int posY = 0;

        // Spawn Units for player
        if (units.Count < 1) return;
        if (GameDirector.Instance.isMultiPlayer()) {
            posX = (playerID == 1) ? 7 : 7;
            posY = (playerID == 1) ? 7 : 14;
            units[0].GetPhotonView().RPC("StartUnit", PhotonTargets.AllBuffered, playerID, posX, posY);
        }
        else
        {
            units[0].GetComponent<SpriteRenderer>().enabled = true;
            units[0].GetComponent<Unit>().playerID = playerID;
            units[0].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[0], new Vector2i(7, 7));
        }

        if (units.Count < 2) return;
        if (GameDirector.Instance.isMultiPlayer())
        {
            posX = (playerID == 1) ? 11 : 11;
            posY = (playerID == 1) ? 7 : 8;
            units[1].GetPhotonView().RPC("StartUnit", PhotonTargets.AllBuffered, playerID, posX, posY);
        }
        else
        {
            units[1].GetComponent<SpriteRenderer>().enabled = true;
            units[1].GetComponent<Unit>().playerID = playerID;
            units[1].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[1], new Vector2i(11, 7));
        }


        if (units.Count < 3) return;
        if (GameDirector.Instance.isMultiPlayer())
        {
            posX = (playerID == 1) ? 15 : 15;
            posY = (playerID == 1) ? 7 : 14;
            units[2].GetPhotonView().RPC("StartUnit", PhotonTargets.AllBuffered, playerID, posX, posY);
        }
        else
        {
            units[2].GetComponent<SpriteRenderer>().enabled = true;
            units[2].GetComponent<Unit>().playerID = playerID;
            units[2].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[2], new Vector2i(15, 7));
        }


        if (units.Count < 4) return;
        if (GameDirector.Instance.isMultiPlayer())
        {
            posX = (playerID == 1) ? 9 : 9;
            posY = (playerID == 1) ? 9 : 16;
            units[3].GetPhotonView().RPC("StartUnit", PhotonTargets.AllBuffered, playerID, posX, posY);
        }
        else
        {
            units[3].GetComponent<SpriteRenderer>().enabled = true;
            units[3].GetComponent<Unit>().playerID = playerID;
            units[3].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[3], new Vector2i(9, 9));
        }


        if (units.Count < 5) return;
        if (GameDirector.Instance.isMultiPlayer())
        {
            posX = (playerID == 1) ? 13 : 13;
            posY = (playerID == 1) ? 9 : 16;
            units[4].GetPhotonView().RPC("StartUnit", PhotonTargets.AllBuffered, playerID, posX, posY);
        }
        else
        {
            units[4].GetComponent<SpriteRenderer>().enabled = true;
            units[4].GetComponent<Unit>().playerID = playerID;
            units[4].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[4], new Vector2i(13, 9));
        }

        
    }

}
