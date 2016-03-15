﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : Singleton<GameDirector>
{
    // TODO: Add game states as needed
    public enum GameState
    {
        MAINMENU = 0,
        LOBBY,
        BOARD,
        MENU
    }

    public GameState gameState;
    public int numOfPlayers;

    CombatSequence combatSequence;

    //AUDIO REFERENCES:
    public AudioClip bgmTitle;
    public AudioClip bgm;

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

    public void setNumPlayers(int selected)
    {
        numOfPlayers = selected;
    }

    public void startGame()
    {
        if (numOfPlayers == 1 || (numOfPlayers == 2 && GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkingMain>().startGame))
        {
            PlayerManager.Instance.setUpNPlayers(numOfPlayers);
            gameState = GameState.BOARD;

            TerrainLayer.Instance.createMap();
        }

        //switch audio track
        Camera.main.GetComponent<AudioSource>().clip = bgm;
        Camera.main.GetComponent<AudioSource>().Play();
    }

    public void endGame()
    {
        PlayerManager.Instance.endGame();
        TerrainLayer.Instance.endGame();
        ObjectManager.Instance.endGame();

        gameState = GameState.MAINMENU;
    }

    // NOTE : CONFIRM/CANCEL BUTTON LOGIC MOVED TO UIMANAGER (Cleanup Reasons)
    // called when there is a change in the contents of the object grid
    // TODO: figure out why the buff system is completely bonkers (only first instantiated object receives buffs)
    public void BoardStateChanged()
    {
        // remove all units' board buffs then apply all units' board specials
        foreach (GameObject unit in ObjectManager.Instance.PlayerOneUnits)
        {
            if (unit.GetComponent<Unit>().buffs == null)
            {
                continue;
            }
            for (int i = unit.GetComponent<Unit>().buffs.Count - 1; i >= 0; i--)
            {
                if (unit.GetComponent<Unit>().buffs[i].type == Buff.BuffType.Board)
                {
                    unit.GetComponent<Unit>().buffs[i].Destroy();
                }
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
                if (unit.GetComponent<Unit>().buffs[i].type == Buff.BuffType.Board)
                {
                    unit.GetComponent<Unit>().buffs[i].Destroy();
                }
            }
        }

        foreach (List<GameObject> units in ObjectManager.Instance.playerUnits)
        {
            ApplyBoardSpecials(units);
        }
    }

    void ApplyBoardSpecials(List<GameObject> units)
    {
        // apply board specials
        foreach (GameObject unit in units)
        {
            Unit u = unit.GetComponent<Unit>();

            // Unit Specials:
            if (u.specialBoardAttributes == null)
            {
                continue;
            }

            foreach (UnitSpecial s in u.specialBoardAttributes)
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

            if (u.equipped.boardSpecials != null)
            {
                foreach (Special s in u.equipped.boardSpecials)
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
