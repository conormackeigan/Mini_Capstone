using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class NetworkingMain : Photon.PunBehaviour
{
    public bool startGame = false;
    private Room[] game;
    private string roomName = "DEFAULT ROOM NAME";
    private string playerName = "DEFAULT NAME";
    bool connecting = false;

    // Use this for initialization
    void Start()
    {
        // NOTE: Uncomment this line for verbose network debugging
        // PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        // Output connection info to screen
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        // Player is currently in lobby
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

            GUILayout.Label("Player Name:");
            playerName = GUILayout.TextField(playerName);
            PhotonNetwork.player.name = PlayerPrefs.GetString("Username", playerName);

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

        // Inside Game Room, awaiting start
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

            // Populate list of players
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                GUI.color = Color.green;
                GUILayout.Box(PhotonNetwork.playerList[i].name);
            }

            // Only the host may start the game
            if (PhotonNetwork.isMasterClient)
            {
                if (GUILayout.Button("Start Game"))
                {
                    if (PhotonNetwork.playerList.Length == 2)
                    {
                        gameObject.GetPhotonView().RPC("StartGameRPC", PhotonTargets.AllBuffered);
                    }
                }
            }
            else
            {
                GUILayout.Label("Waiting for host to start game");
            }

            GUILayout.EndArea();

        }
    }

    //==============================
    // Networking Methods
    //==============================
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

    //=================================
    // RPC Calls
    //=================================

    [PunRPC]
    public void StartGameRPC()
    {
        startGame = true;
        GameDirector.Instance.startGame();
    }

    
}
