using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<GameObject> players;
    public int currentPlayersTurn;
    public GameObject TurnLabel;

	// Use this for initialization
	void Start () {
        currentPlayersTurn = 1;
    }

    // Update is called once per frame
    void Update () {
    }

    public void endGame()
    {
        for(int i = 0; i < players.Count; i++)
        {
            Destroy(players[i]);
        }

        players.Clear();

        currentPlayersTurn = 1;
    }

    public void setUpNPlayers(int n)
    {
        if (GameDirector.Instance.numOfPlayers == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject testPlayer = Instantiate(Resources.Load("Player")) as GameObject;
                // GameObject testPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0) as GameObject;

                players.Add(testPlayer);
                testPlayer.transform.parent = gameObject.transform;

                string s = (i + 1) + "Player";
                testPlayer.GetComponent<Player>().initPlayer(s, (i + 1), Color.red, false);
            }
        }
        else
        {
            GameObject testPlayer = Instantiate(Resources.Load("Player")) as GameObject;
            //GameObject testPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0) as GameObject;

            players.Add(testPlayer);
            testPlayer.transform.parent = gameObject.transform;

            string s = (PhotonNetwork.player.ID) + "Player";
            testPlayer.GetComponent<Player>().initPlayer(s, (PhotonNetwork.player.ID), Color.red, true);
        }
    }

    public void nextTurn()
    {
        // Deselect current unit upon end turn (if required); Reset all inactive units
        UIManager.Instance.DeactivateEnemyPanel();
        ObjectManager.Instance.ResetToActive(currentPlayersTurn);

        if (getCurrentPlayer().selectedObject != null && getCurrentPlayer().selectedObject.tag == "Unit")
        {
            getCurrentPlayer().selectedObject.GetComponent<Unit>().deselectUnit();
        }

        currentPlayersTurn += 1;

        if(currentPlayersTurn > 2)
        {
            currentPlayersTurn = 1;
        }

        if (GameDirector.Instance.numOfPlayers == 1)
        {
            if (currentPlayersTurn == 1)
            {
                TurnLabel.GetComponent<Text>().text = "Your Turn";
            }
            else
            {
                TurnLabel.GetComponent<Text>().text = "Enemy Turn";
            }

            UIManager.Instance.animateTurnPanel();
        }
        else
        {
            gameObject.GetPhotonView().RPC("NextTurn", PhotonTargets.AllBuffered, currentPlayersTurn);
        }

    }

    public Player getCurrentPlayer()
    {
        if (players.Count == 1)
        {
            return players[0].GetComponent<Player>();
        }
        else
        {
            return players[currentPlayersTurn - 1].GetComponent<Player>();
        }
    }

    public int getCurrentPlayerTurn()
    {
        return currentPlayersTurn;
    }

}
