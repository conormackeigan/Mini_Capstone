using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<GameObject> players;
    public int currentPlayersTurn;
    public GameObject TurnLabelTop;
    public GameObject TurnLabel;
    public GameObject EndTurnButton;

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
        if (GameDirector.Instance.isSinglePlayer())
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject testPlayer = Instantiate(Resources.Load("Player")) as GameObject;
                // GameObject testPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0) as GameObject;

                players.Add(testPlayer);
                testPlayer.transform.parent = gameObject.transform;

                string s = (i + 1) + "Player";
                if (i == 0)
                {
                    testPlayer.GetComponent<Player>().initPlayer(s, (i + 1), Color.red, false, Player.PLAYER_TYPE.Human);
                }
                else
                {
                    Debug.Log("initing computer");
                    testPlayer.GetComponent<Player>().initPlayer(s, (i + 1), Color.red, false, Player.PLAYER_TYPE.Computer);
                }
            }
        }
        else
        {
            GameObject testPlayer = Instantiate(Resources.Load("Player")) as GameObject;
            //GameObject testPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0) as GameObject;

            players.Add(testPlayer);
            testPlayer.transform.parent = gameObject.transform;

            string s = (PhotonNetwork.player.ID) + "Player";
            testPlayer.GetComponent<Player>().initPlayer(s, (PhotonNetwork.player.ID), Color.red, true, Player.PLAYER_TYPE.Human);

            if(PhotonNetwork.player.ID != 1)
            {
                TurnLabelTop.GetComponent<Text>().text = "Enemy Turn";
                TurnLabel.GetComponent<Text>().text = "Enemy Turn";
                EndTurnButton.GetComponent<Button>().enabled = false;
                GLOBAL.setLock(true);
            }
        }
    }

    public void nextTurn()
    {
        // Deselect current unit upon end turn (if required); Reset all inactive units
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

        if (GameDirector.Instance.isSinglePlayer())
        {
            if (currentPlayersTurn == 1)
            {
                TurnLabelTop.GetComponent<Text>().text = "Your Turn";
                TurnLabel.GetComponent<Text>().text = "Your Turn";
            }
            else
            {
                TurnLabelTop.GetComponent<Text>().text = "Enemy Turn";
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
