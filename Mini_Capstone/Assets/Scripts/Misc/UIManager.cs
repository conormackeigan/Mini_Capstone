using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    GameObject[] boardObjets;
    GameObject[] lobbyObjects;
    GameObject[] unitObjects;

    public GameObject friendlyPanel;
    public GameObject enemyPanel;
    public GameObject attackButton;
    public GameObject waitButton;
    public GameObject AoEButton;
    public GameObject turnPanel;
    public GameObject purchaseUI;

    // Use this for initialization
    void Start () {

        boardObjets = GameObject.FindGameObjectsWithTag("BoardUI");
        lobbyObjects = GameObject.FindGameObjectsWithTag("LobbyUI");
        unitObjects = GameObject.FindGameObjectsWithTag("UnitUI");
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

            for (int i = 0; i < unitObjects.Length; i++)
            {
                unitObjects[i].SetActive(false);
            }

        }
        else if (gameState == GameDirector.GameState.LOBBY)
        {
            for (int i = 0; i < lobbyObjects.Length; i++)
            {
                lobbyObjects[i].SetActive(false);
            }
        }
        else if (gameState == GameDirector.GameState.PURCHASE)
        {
            for (int i = 0; i < lobbyObjects.Length; i++)
            {
                lobbyObjects[i].SetActive(false);
            }

            purchaseUI.SetActive(true);
        }
        else if (gameState == GameDirector.GameState.BOARD)
        {
            purchaseUI.SetActive(false);

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

    public void setUnitUI(bool b)
    {
        for (int i = 0; i < unitObjects.Length; i++)
        {
            unitObjects[i].SetActive(b);
        }
    }

    public void activateAttackButton()
    {
        attackButton.SetActive(true);
        waitButton.SetActive(false);
    }

    public void deactivateAttackButton()
    {
        attackButton.SetActive(false);
        waitButton.SetActive(true);
    }

    public void activateAoEButton()
    {
        AoEButton.SetActive(true);
    }

    public void deactivateAoEButton()
    {
        AoEButton.SetActive(false);
    }

    // disables both attack and wait button regardless of which is active
    public void deactivateConfirmButton()
    {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
    }

    public void animateTurnPanel()
    {
        turnPanel.SetActive(true);
        GLOBAL.setLock(true);
    }

    public void ActivateFriendPanel(Unit u)
    {
        friendlyPanel.SetActive(true);

        friendlyPanel.transform.Find("UnitName").GetComponent<Text>().text = u.unitName;
        friendlyPanel.transform.Find("UnitImage").GetComponent<Image>().sprite = u.sprite;
        friendlyPanel.transform.Find("UnitHealthValue").GetComponent<Text>().text = u.health.ToString();

        friendlyPanel.transform.Find("HealthSlider").GetComponent<Slider>().maxValue = u.maxHealth;
        friendlyPanel.transform.Find("HealthSlider").GetComponent<Slider>().value = u.health;

        friendlyPanel.GetComponent<SelectedInfo>().init();

    }

    public void DeactivateFriendPanel()
    {
        friendlyPanel.GetComponent<SelectedInfo>().close();
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

    public void ConfirmAction()
    {
        Player p = PlayerManager.Instance.getCurrentPlayer();
        if (p.selectedObject != null && p.selectedObject.tag == "Unit")
        { 
            // AoE:
            if (p.selectedObject.GetComponent<Unit>().state == Unit.UnitState.AoE)
            {
                // unit is confirming an AoE attack; tell CombatSequence to call the realtime AoE sequence
                GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoEAttack();                
            }
            // Regular Combat:
            else if (GameObject.Find("CombatSequence").GetComponent<CombatSequence>().active)
            { // if a combat sequence is active confirm button will initiate the sequence
                CombatSequence combat = GameObject.Find("CombatSequence").GetComponent<CombatSequence>();
                if (combat.preCombat)
                {
                    combat.preCombat = false;
                    combat.InitiateSequence();
                }
            }
            // End Turn:
            else
            {
                p.selectedObject.GetComponent<Unit>().Deactivate();                
                GameDirector.Instance.BoardStateChanged();
            }
        }

        // Check if turn should end
        ObjectManager.Instance.EndTurnForPlayer(p.playerID);
    }


    public void AoEBegin()
    {
        // this button is only available when a unit is selected so it's safe to assume
        //Debug.Assert(PlayerManager.Instance.getCurrentPlayer().selectedObject == null, "No unit selected to perform an AoE attack");

        PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>().AoEBegin();
        deactivateAoEButton();
        deactivateConfirmButton();
    }

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
