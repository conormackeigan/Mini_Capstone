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
	void Start () {

        playerType = PLAYER_TYPE.Human;

        selectedObject = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void initPlayer(string name, int id, Color c, bool network)
    {
        playerType = PLAYER_TYPE.Human;
        playerName = name;
        playerID = id;
        playerColor = c;

        selectedObject = null;

        //TODO: Set up temp starting units and positions (once units complete)
       /* if (id == 1)
        {
            if (network)
            {
                GameObject unitObj = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(3, 3));

                unitObj = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 3));

                unitObj = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 3));

                unitObj = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(5, 5));

                unitObj = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 5));
            }
            else {
                //TODO: Note starting units are hard coded for now
                GameObject unitObj = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(3, 3));

                unitObj = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 3));

                unitObj = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 3));

                unitObj = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(5, 5));

                unitObj = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 5));
            }
        }
        else if (id == 2)
        {
            if (network)
            {
                //TODO: Note starting units are hard coded for now
                GameObject unitObj = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(3, 15));

                unitObj = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 4)); // originally 7,15

                unitObj = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 15));

                unitObj = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(5, 13));

                unitObj = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 13));
            }
            else
            {
                //TODO: Note starting units are hard coded for now
                GameObject unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(3, 10));

                unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 4)); // originally 7,15

                unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 10));

                unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(5, 12));

                unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
                unitObj.GetComponent<Unit>().playerID = playerID;
                ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 12));
            }
        }*/
    }

    public void startBoardWithUnits(List<GameObject> units)
    {
        if(playerID == 1)
        {
            GameObject unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            uInfantry script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(3, 10));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(7, 4));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(11, 10));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(5, 12));

            unitObj = Instantiate(Resources.Load("UInfantryBlue")) as GameObject;
            unitObj.GetComponent<SpriteRenderer>().enabled = true;
            script = unitObj.GetComponent<uInfantry>();
            script.playerID = 2;
            script.Init(true);
            ObjectManager.Instance.addObjectAtPos(unitObj, new Vector2i(9, 12));


            if (units.Count < 1) return;
            units[0].GetComponent<SpriteRenderer>().enabled = true;
            units[0].GetComponent<Unit>().playerID = playerID;
            units[0].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[0], new Vector2i(3, 3));

            if (units.Count < 2) return;
            units[1].GetComponent<SpriteRenderer>().enabled = true;
            units[1].GetComponent<Unit>().playerID = playerID;
            units[1].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[1], new Vector2i(7, 3));

            if (units.Count < 3) return;
            units[2].GetComponent<SpriteRenderer>().enabled = true;
            units[2].GetComponent<Unit>().playerID = playerID;
            units[2].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[2], new Vector2i(11, 3));

            if (units.Count < 4) return;
            units[3].GetComponent<SpriteRenderer>().enabled = true;
            units[3].GetComponent<Unit>().playerID = playerID;
            units[3].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[3], new Vector2i(5, 5));

            if (units.Count < 5) return;
            units[4].GetComponent<SpriteRenderer>().enabled = true;
            units[4].GetComponent<Unit>().playerID = playerID;
            units[4].GetComponent<Unit>().Init();
            ObjectManager.Instance.addObjectAtPos(units[4], new Vector2i(9, 5));
        }
    }

}
