using UnityEngine;
using System.Collections;

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
        if (id == 1)
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
        }
    }
}
