﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    GameObject[] boardObjets;
    GameObject[] lobbyObjects;

    public GameObject friendlyPanel;
    public GameObject enemyPanel;

    // Use this for initialization
    void Start () {

        boardObjets = GameObject.FindGameObjectsWithTag("BoardUI");
        lobbyObjects = GameObject.FindGameObjectsWithTag("LobbyUI");
    }
	
	// Update is called once per frame
	void Update () {

        GameDirector.GameState gameState = GameDirector.Instance.gameState;

        if (gameState == GameDirector.GameState.MAINMENU)
        {
            for (int i = 0; i < lobbyObjects.Length; i++)
            {
                lobbyObjects[i].SetActive(true);
            }

            for (int i = 0; i < boardObjets.Length; i++)
            {
                boardObjets[i].SetActive(false);
            }

        }
        else if (gameState == GameDirector.GameState.LOBBY)
        {

            for (int i = 0; i < lobbyObjects.Length; i++)
            {
                lobbyObjects[i].SetActive(false);
            }
        }
        else if (gameState == GameDirector.GameState.BOARD)
        {
            for (int i = 0; i < lobbyObjects.Length; i++)
            {
                lobbyObjects[i].SetActive(false);
            }

            for (int i = 0; i < boardObjets.Length; i++)
            {
                boardObjets[i].SetActive(true);
            }
        }

    }

    public void ActivateFriendPanel(Unit u)
    {
        friendlyPanel.SetActive(true);

        friendlyPanel.transform.Find("UnitName").GetComponent<Text>().text = u.unitName;
        friendlyPanel.transform.Find("UnitImage").GetComponent<Image>().sprite = u.sprite;
        friendlyPanel.transform.Find("UnitHealthValue").GetComponent<Text>().text = u.health.ToString();
        friendlyPanel.transform.Find("UnitAttackValue").GetComponent<Text>().text = u.physAtk.ToString();
        friendlyPanel.transform.Find("UnitDefenceValue").GetComponent<Text>().text = u.defense.ToString();

    }

    public void DeactivateFriendPanel()
    {
        friendlyPanel.SetActive(false);

    }

    public void ActivateEnemyPanel(Unit u)
    {
        enemyPanel.SetActive(true);

        enemyPanel.transform.Find("UnitName").GetComponent<Text>().text = u.unitName;
        enemyPanel.transform.Find("UnitImage").GetComponent<Image>().sprite = u.sprite;
        enemyPanel.transform.Find("UnitHealthValue").GetComponent<Text>().text = u.health.ToString();
        enemyPanel.transform.Find("UnitAttackValue").GetComponent<Text>().text = u.physAtk.ToString();
        enemyPanel.transform.Find("UnitDefenceValue").GetComponent<Text>().text = u.defense.ToString();

        TileMarker.Instance.Clear();
        TileMarker.Instance.markTravTiles(u);
        TileMarker.Instance.markAttackTiles(u);
    }

    public void DeactivateEnemyPanel()
    {
        if(enemyPanel.activeSelf)
        {
            enemyPanel.SetActive(false);
            TileMarker.Instance.Clear();
        }
    }
    

    // TODO: THIS LOGIC + DESELECT AND RESET METHODS ARE A MESS, CLEAN UP SOON
    public void ConfirmAction()
    {
        Player p = PlayerManager.Instance.getCurrentPlayer();
        if (p.selectedObject != null && p.selectedObject.tag == "Unit")
        { // confirm button commits whatever action is currently available to the unit. for now just force inactive until combat is implemented

            if (GameObject.Find("CombatSequence").GetComponent<CombatSequence>().active)
            { // if a combat sequence is active confirm button will initiate the sequence
                CombatSequence combat = GameObject.Find("CombatSequence").GetComponent<CombatSequence>();
                if (combat.preCombat)
                {
                    combat.preCombat = false;
                    combat.InitiateSequence();
                }
            }
            else
            {
                p.selectedObject.GetComponent<Unit>().Deactivate();                
                GameDirector.Instance.BoardStateChanged();
            }
        }

        // Check if turn should end
        ObjectManager.Instance.EndTurnForPlayer(p.playerID);
    }

    // TODO: SAME AS ABOVE
    public void CancelAction()
    {
        Player p = PlayerManager.Instance.getCurrentPlayer();
        if (p.selectedObject != null)
        {
            switch (p.selectedObject.tag)
            {
                case "Unit":
                    {
                        p.selectedObject.GetComponent<Unit>().ResetUnit();
                        break;
                    }
            }

        }
    }
}
