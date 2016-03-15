using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class NetworkingMain : Photon.PunBehaviour
{
    private static PhotonView ScenePhotonView;
    public GameObject go;

    public bool startGame = false;
    public GameObject SpawnSpot;
    private Room[] game;
    private string roomName = "DEFAULT ROOM NAME";
    bool connecting = false;
    public string Version = "Version 1";

    // Use this for initialization
    void Start()
    {

        // NOTE: Uncomment this line for verbose network debugging
        // PhotonNetwork.logLevel = PhotonLogLevel.Full;
        ScenePhotonView = this.GetComponent<PhotonView>();

        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "My Player name");
    }

    // Update is called once per frame
    void Update()
    {
        if(go)
        {
            Destroy(go);
        }           
    }

    void OnGUI()
    {
        // Output connection info to screen
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connected == true && connecting == false && startGame == false)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = Color.cyan;
            GUILayout.Box("Lobby");
            GUI.color = Color.white;

            GUILayout.Space(20);
            GUI.color = Color.yellow;
            GUILayout.Box("Users Joined: ");
            GUI.color = Color.red;
            GUILayout.Space(20);

            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                GUI.color = Color.green;
                GUILayout.Box(PhotonNetwork.playerList[i].name);
            }

            if (GUILayout.Button("Start Game"))
            {
                if (PhotonNetwork.playerList.Length == 2)
                {
                    startGame = true;
                    GameDirector.Instance.startGame();

                }
            }

            GUILayout.EndArea();

        }

        if (PhotonNetwork.insideLobby == true)
        {

            GameDirector.Instance.gameState = GameDirector.GameState.LOBBY;

            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            GUI.color = Color.white;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = Color.cyan;
            GUILayout.Box("Lobby");
            GUI.color = Color.white;

            GUILayout.Label("Session Name:");
            roomName = GUILayout.TextField(roomName);

            if (GUILayout.Button("Create Room "))
            {
                if (roomName != "")
                {
                    RoomOptions devOptions = new RoomOptions() { isVisible = true, isOpen = true, maxPlayers = 2 };

                    PhotonNetwork.CreateRoom(roomName, devOptions, TypedLobby.Default);
                }
            }

            GUILayout.Space(20);
            GUI.color = Color.yellow;
            GUILayout.Box("Sessions Open");
            GUI.color = Color.red;
            GUILayout.Space(20);

            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                GUI.color = Color.green;
                GUILayout.Box(game.name + " " + game.playerCount + "/" + game.maxPlayers + " " + game.visible);
                if (GUILayout.Button("Join Session"))
                {
                    PhotonNetwork.JoinRoom(game.name);
                }
            }
            GUILayout.EndArea();
        }
    }


    public void Connect()
    {
        if (GameDirector.Instance.numOfPlayers == 1)
        {
            return;
        }
        // NOTE: Uncomment to turn on networking
        PhotonNetwork.ConnectUsingSettings(GLOBAL.VERSION_NUMBER);

        connecting = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        connecting = false;

        Debug.Log(PhotonNetwork.player.ID);
    }

    //function used to destroy game object
    [PunRPC]
    public void TaggedPlayer()
    {

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].GetComponent<uInfantry>().isDead)
            {
                go = units[i];
                //PhotonNetwork.Destroy(units[i]);
                Debug.Log("Destroy Object");
            }
        }
    }

}
