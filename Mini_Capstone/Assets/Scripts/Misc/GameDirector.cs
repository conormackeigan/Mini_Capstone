using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class GameDirector : Singleton<GameDirector>
{
    // TODO: Add game states as needed
    public enum GameState
    {
        MAINMENU = 0,
        LOBBY,
        PURCHASE,
        BOARD,
        GAMEOVER,
        MENU
    }

    public GameState gameState;
    public int numOfPlayers;

    CombatSequence combatSequence;

    public bool locked = false; // locks user input

    //AUDIO REFERENCES:
    public AudioClip bgmTitle;
    public AudioClip bgm;
    public AudioClip sfxSelect;
    public AudioClip sfxCancel;

    // Use this for initialization
    void Start ()
    {
        numOfPlayers = 1;
        combatSequence = null;

        gameState = GameState.MAINMENU;

    }

    // Update is called once per frame
    void Update () {

    }

    public bool isSinglePlayer()
    {
        return (numOfPlayers == 1);
    }

    public bool isMultiPlayer()
    {
        return (numOfPlayers == 2);
    }

    public void setNumPlayers(int selected)
    {
        numOfPlayers = selected;
    }

    public void purchaseUnits()
    {
        if (numOfPlayers == 1 || (numOfPlayers == 2 && GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkingMain>().startGame))
        {
            PlayerManager.Instance.setUpNPlayers(numOfPlayers);
            gameState = GameState.PURCHASE;
        }
    }

    public void cancelPurchase()
    {

        if(isMultiPlayer())
        {
            gameObject.GetPhotonView().RPC("EndGameNetwork", PhotonTargets.AllBuffered, true);
        }
        else
        {
            PlayerManager.Instance.endGame();
            UnitSelection.Instance.Reset();

        }

        gameState = GameState.MAINMENU;
    }

    public void startGame()
    {
        if (numOfPlayers == 1 || (numOfPlayers == 2 && GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkingMain>().startGame))
        {
            Social.ReportProgress("CgkIpqXyhekJEAIQAQ", 100.0f, (bool success) => {
                if (success)
                {
                    Debug.Log("Achievement Get!");
                }
                else {
                    Debug.Log("Authentication failed.");
                }
            });

            PlayerManager.Instance.getCurrentPlayer().startBoardWithUnits(UnitSelection.Instance.purchasedUnits);
            gameState = GameState.BOARD;

            TerrainLayer.Instance.createMap();
            UIManager.Instance.animateTurnPanel();

        }

        //switch audio track
        Camera.main.GetComponent<AudioSource>().clip = bgm;
        Camera.main.GetComponent<AudioSource>().Play();
    }

    public void endGame(bool isDisconnect)
    {
        Social.ReportProgress("CgkIpqXyhekJEAIQAg", 100.0f, (bool success) => {
            if (success)
            {
                Debug.Log("Achievement Get!");
            }
            else {
                Debug.Log("Authentication failed.");
            }
        });

        if (isSinglePlayer())
        {
            UnitSelection.Instance.Reset();
            PlayerManager.Instance.endGame();
            TerrainLayer.Instance.endGame();
            ObjectManager.Instance.endGame();
            
            gameState = GameState.GAMEOVER;
        }
        else
        {
            gameObject.GetPhotonView().RPC("EndGameNetwork", PhotonTargets.AllBuffered, isDisconnect);
        }

    }

    public void returnToMenu()
    {
        gameState = GameState.MAINMENU;
    }


    // NOTE : CONFIRM/CANCEL BUTTON LOGIC MOVED TO UIMANAGER (Cleanup Reasons)
    // called when there is a change in the contents of the object grid
    // TODO: figure out why the buff system is completely bonkers (only first instantiated object receives buffs)
    public void BoardStateChanged()
    {
        // remove all units' board/passive buffs then reapply all units' board/passive specials
        foreach (GameObject unit in ObjectManager.Instance.PlayerOneUnits)
        {
            if (unit.GetComponent<Unit>().buffs == null)
            {
                continue;
            }
            for (int i = unit.GetComponent<Unit>().buffs.Count - 1; i >= 0; i--)
            {
                unit.GetComponent<Unit>().buffs[i].Destroy();       
            }
        }
        foreach (GameObject unit in ObjectManager.Instance.PlayerTwoUnits)
        {
            if (unit.GetComponent<Unit>().buffs == null)
            {
                continue;
            }
            for (int i = unit.GetComponent<Unit>().buffs.Count - 1; i >= 0; i--)
            {
                unit.GetComponent<Unit>().buffs[i].Destroy();
            }
        }

        foreach (List<GameObject> units in ObjectManager.Instance.playerUnits)
        {
            ApplySpecials(units);
        }
    }

    void ApplySpecials(List<GameObject> units)
    {
        // apply board specials
        foreach (GameObject unit in units)
        {
            Unit u = unit.GetComponent<Unit>();

            // Unit Specials:
            if (u.specials == null)
            {
                continue;
            }

            foreach (UnitSpecial s in u.specials)
            {
                if (s.condition != null)
                {
                    if (s.condition.eval())
                    {
                        s.effect();
                    }
                }
                else
                {
                    s.effect();
                }
            }

            // Weapon Specials:
            if (u.equipped == null)
            {
                continue;
            }

            if (u.equipped.specials != null)
            {
                foreach (Special s in u.equipped.specials)
                {
                    if (s.condition != null)
                    {
                        if (s.condition.eval())
                        {
                            s.effect();
                        }
                    }
                    else
                    {
                        s.effect();
                    }
                }
            }
                  
        } // close outer foreach

        foreach (GameObject u in units)
        { // apply buff offsets to base stats
            u.GetComponent<Unit>().UpdateStats(); 
        }
    }
}
