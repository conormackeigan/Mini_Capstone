using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitSelection : Singleton<UnitSelection>
{
    int INFANTRY_BASE_COST = 100;
    int TANK_BASE_COST = 125;
    int EXO_BASE_COST = 250;

    public int currentTotal;
    public int unitTotal;
    public List<string> weaponsSelected;
    public string selectedUnit;
    public Sprite cancelSprite;
    public Sprite energySprite;
    public Sprite bullet;
    public Text unitCostText;

    public GameObject purchasedTab;
    public GameObject ticketCount;
    public GameObject selectionPanel;
    public GameObject purchasePanel;
    public GameObject previewPanel;
    public GameObject deployButton;
    public GameObject weaponSelect;

    public List<GameObject> purchasedUnits;

    // Stats Panel
    int statPoints;
    int energy;
    int attack;
    int defence;
    int speed;

    public bool playerOneDeploy = false;
    public bool playerTwoDeploy = false;

    // Use this for initialization
    void Start () {
        purchasedUnits = new List<GameObject>();
        currentTotal = 500;

        ticketCount.GetComponent<Text>().text = currentTotal.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if(GameDirector.Instance.isMultiPlayer())
        {
            if (playerOneDeploy && playerTwoDeploy)
            {
                GameDirector.Instance.startGame();
            }
            else if ((PhotonNetwork.player.ID == 1 && playerOneDeploy) || (PhotonNetwork.player.ID == 2 && playerTwoDeploy))
            {
                deployButton.GetComponent<Button>().enabled = false;
                deployButton.GetComponentInChildren<Text>().text = "Waiting for Other Player";
            }
        }
	}

    //==============================
    // Menu Functions
    //==============================
    public void Deploy()
    {
        if(purchasedUnits.Count <= 0)
        {
            return;
        }

        if (GameDirector.Instance.isMultiPlayer())
        {
            gameObject.GetPhotonView().RPC("SelectDeploy", PhotonTargets.AllBuffered, PhotonNetwork.player.ID);
        }
        else
        {
            GameDirector.Instance.startGame();
        }
    }

    public void Reset()
    {
        foreach (GameObject g in purchasedUnits)
        {
            Destroy(g);
        }

        playerOneDeploy = false;
        playerTwoDeploy = false;
        deployButton.GetComponent<Button>().enabled = true;
        deployButton.GetComponentInChildren<Text>().text = "Deploy";

        purchasedUnits = new List<GameObject>();
        currentTotal = 500;
        ticketCount.GetComponent<Text>().text = currentTotal.ToString();
        updatePurchasedTab();
    }

    public void Purchase()
    {
        switch(selectedUnit)
        {
            case "Infantry":
            {
                purchaseInfantry();
                break;
            }
            case "Tank":
            {
                purchaseTank();
                break;
            }
            case "Exo":
            {
                purchaseExo();
                break;
            }
            default:
                break;
        }
    }

    public void decreaseTicketCount(int c)
    {
        currentTotal = int.Parse(ticketCount.GetComponent<Text>().text) - c;
        ticketCount.GetComponent<Text>().text = currentTotal.ToString();
    }

    public void increaseTicketCount(int c)
    {
        currentTotal = int.Parse(ticketCount.GetComponent<Text>().text) + c;
        ticketCount.GetComponent<Text>().text = currentTotal.ToString();
    }

    public void selectInfantry()
    {
        Debug.Log("Infantry Selected");

        selectionPanel.SetActive(false);
        purchasePanel.SetActive(true);

        unitTotal = 0;

        selectedUnit = "Infantry";

        statPoints = 5;
        energy = 5;
        attack = 5;
        defence = 5;
        speed = 5;

        updateEnergy(0);
        updateAttack(0);
        updateDefence(0);
        updateSpeed(0);

        if (!weaponsSelected.Contains("Rifle"))
        {
            weaponsSelected.Add("Rifle");
        }

        unitCostText.color = new Color(0.19f, 0.19f, 0.19f);
        unitCostText.text = (INFANTRY_BASE_COST - unitTotal).ToString();

    }

    //==================================
    // Stat Allocation
    //==================================

    public void increaseStat(string stat)
    {
        if(statPoints == 0)
        {
            return;
        }

        switch (stat)
        {
            case "Health":
                {
                    updateEnergy(1);
                    break;
                }
            case "Attack":
                {
                    updateAttack(1);
                    break;
                }
            case "Defence":
                {
                    updateDefence(1);
                    break;
                }
            case "Speed":
                {
                    updateSpeed(1);
                    break;
                }
            default:
                break;
        }
    }

    public void decreaseStat(string stat)
    {
        
        switch (stat)
        {
            case "Health":
                {
                    updateEnergy(-1);
                    break;
                }
            case "Attack":
                {
                    updateAttack(-1);
                    break;
                }
            case "Defence":
                {
                    updateDefence(-1);
                    break;
                }
            case "Speed":
                {
                    updateSpeed(-1);
                    break;
                }
            default:
                break;
        }

    }

    void updateEnergy(int i)
    {
        if ((i == 1 && energy == 10) || (i == -1 && energy == 1))
        {
            return;
        }

        energy += i;
        statPoints -= i;

        GameObject.Find("HealthSlider").GetComponent<Slider>().value = (float) energy;
        GameObject.Find("HealthStatCount").GetComponent<Text>().text = energy.ToString();
        GameObject.Find("StatPoints").GetComponent<Text>().text = statPoints.ToString();

    }

    void updateAttack(int i)
    {
        if ((i == 1 && attack == 10) || (i == -1 && attack == 1))
        {
            return;
        }

        attack += i;
        statPoints -= i;

        GameObject.Find("AttackSlider").GetComponent<Slider>().value = (float)attack;
        GameObject.Find("AttackStatCount").GetComponent<Text>().text = attack.ToString();
        GameObject.Find("StatPoints").GetComponent<Text>().text = statPoints.ToString();

    }

    void updateDefence(int i)
    {
        if ((i == 1 && defence == 10) || (i == -1 && defence == 1))
        {
            return;
        }

        defence += i;
        statPoints -= i;

        GameObject.Find("DefenceSlider").GetComponent<Slider>().value = (float)defence;
        GameObject.Find("DefenceStatCount").GetComponent<Text>().text = defence.ToString();
        GameObject.Find("StatPoints").GetComponent<Text>().text = statPoints.ToString();

    }

    void updateSpeed(int i)
    {
        if ((i == 1 && speed == 10) || (i == -1 && speed == 1))
        {
            return;
        }

        speed += i;
        statPoints -= i;

        GameObject.Find("SpeedSlider").GetComponent<Slider>().value = (float)speed;
        GameObject.Find("SpeedStatCount").GetComponent<Text>().text = speed.ToString();
        GameObject.Find("StatPoints").GetComponent<Text>().text = statPoints.ToString();

    }

    //====================================
    // Weapon Selection
    //====================================

    public void updateWeaponMenu(Text name)
    {
        string unitName = name.text;

        switch (unitName)
        {
            case "Infantry":
                {
                    setInfantryWeapons();
                    break;
                }
            default:
                {
                    Debug.Log("Unit not found");
                    break;
                }
        }

    }

    void setInfantryWeapons()
    {
        // BeamSword
        Transform Attack1 = weaponSelect.transform.FindChild("Attack1");
        Attack1.FindChild("WeaponType").FindChild("Image").GetComponent<Image>().sprite = energySprite;
        Attack1.FindChild("WeaponName").FindChild("Name").GetComponent<Text>().text = "Sword";
        Attack1.FindChild("WeaponAttack").FindChild("Power").GetComponent<Text>().text = "15";
        Attack1.FindChild("WeaponRange").FindChild("Range").GetComponent<Text>().text = "1";
        Attack1.FindChild("WeaponAccuracy").FindChild("Accuracy").GetComponent<Text>().text = "100";
        Attack1.FindChild("Cost").FindChild("Text").GetComponent<Text>().text = "20";
        Attack1.FindChild("SelectedHighLight").gameObject.SetActive(weaponsSelected.Contains("Sword"));

        // Rifle (Default)
        Transform Attack2 = weaponSelect.transform.FindChild("Attack2");
        Attack2.FindChild("WeaponType").FindChild("Image").GetComponent<Image>().sprite = bullet;
        Attack2.FindChild("WeaponName").FindChild("Name").GetComponent<Text>().text = "Rifle";
        Attack2.FindChild("WeaponAttack").FindChild("Power").GetComponent<Text>().text = "8";
        Attack2.FindChild("WeaponRange").FindChild("Range").GetComponent<Text>().text = "1-3";
        Attack2.FindChild("WeaponAccuracy").FindChild("Accuracy").GetComponent<Text>().text = "100";
        Attack2.FindChild("Cost").FindChild("Text").GetComponent<Text>().text = "0";
        Attack2.FindChild("SelectedHighLight").gameObject.SetActive(weaponsSelected.Contains("Rifle"));


        // Frag
        Transform Attack3 = weaponSelect.transform.FindChild("Attack3");
        Attack3.FindChild("WeaponType").FindChild("Image").GetComponent<Image>().sprite = bullet;
        Attack3.FindChild("WeaponName").FindChild("Name").GetComponent<Text>().text = "Frag";
        Attack3.FindChild("WeaponAttack").FindChild("Power").GetComponent<Text>().text = "10";
        Attack3.FindChild("WeaponRange").FindChild("Range").GetComponent<Text>().text = "0-3";
        Attack3.FindChild("WeaponAccuracy").FindChild("Accuracy").GetComponent<Text>().text = "85";
        Attack3.FindChild("Cost").FindChild("Text").GetComponent<Text>().text = "30";
        Attack3.FindChild("SelectedHighLight").gameObject.SetActive(weaponsSelected.Contains("Frag"));

    }

    public void weaponSelected(GameObject attackInfo)
    {
        Transform t = attackInfo.transform;
        GameObject highlight = t.FindChild("SelectedHighLight").gameObject;

        if (weaponsSelected.Count >= 3 && !highlight.activeSelf)
        {
            return;
        }

        highlight.SetActive(!highlight.activeSelf);

        int cost = int.Parse(t.FindChild("Cost").FindChild("Text").GetComponent<Text>().text);

        if (highlight.activeSelf)
        {
            unitTotal -= cost;
            unitCostText.text = (INFANTRY_BASE_COST - unitTotal).ToString();
        }
        else
        {
            unitTotal += cost;
            unitCostText.text = (INFANTRY_BASE_COST - unitTotal).ToString();
        }

        string weapon = t.FindChild("WeaponName").FindChild("Name").GetComponent<Text>().text;
        if (highlight.activeSelf && !weaponsSelected.Contains(weapon))
        {
            weaponsSelected.Add(weapon);
        }
        else
        {
            weaponsSelected.Remove(weapon);
        }
    }

    void initWeapon(string s, Unit script)
    {
        if(script.weapons == null)
        {
            script.weapons = new List<Weapon>();
        }
        switch (s)
        {
            case "Sword":
                {
                    Weapon sword = new BeamSword(script);
                    script.weapons.Add(sword);
                    script.Equip(sword);
                    break;
                }
            case "Rifle":
                {
                    Weapon rifle = new Rifle(script);
                    script.weapons.Add(rifle);
                    script.Equip(rifle);
                    break;
                }
            case "Frag":
                {
                    Weapon frag = new Frag(script);
                    script.weapons.Add(frag);
                    script.Equip(frag);
                    break;
                }
            default:
                {
                    Debug.Log("Unit not found");
                    break;
                }
        }
    }

    //=====================================
    // Purchasing
    //=====================================

    public void purchaseInfantry()
    {
        // TODO: Provide feedback for fail
        if (purchasedUnits.Count >= 5 || currentTotal - INFANTRY_BASE_COST - unitTotal < 0)
        {
            unitCostText.color = Color.red;
            return;
        }

        unitCostText.color = new Color(0.19f, 0.19f, 0.19f);
        Debug.Log("Infantry Purchased");

        GameObject infantry = null;
        if (GameDirector.Instance.isMultiPlayer())
        {
            if (PlayerManager.Instance.getCurrentPlayer().playerID == 1)
            {
                infantry = PhotonNetwork.Instantiate("UInfantryRed", Vector3.zero, Quaternion.identity, 0) as GameObject;
            }
            else
            {
                infantry = PhotonNetwork.Instantiate("UInfantryBlue", Vector3.zero, Quaternion.identity, 0) as GameObject;
            }
        }
        else
        {
           infantry = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
        }
        purchasedUnits.Add(infantry);
        uInfantry script = infantry.GetComponent<uInfantry>();

        script.maxEnergy = energy;
        script.energy = energy;
        script.defense = defence;
        script.physAtk = attack;
        script.energyAtk = attack;
        script.speed = speed;
        script.movementRange = speed;

        for(int i = 0; i < weaponsSelected.Count; i++)
        {
            initWeapon(weaponsSelected[i], script);
        }
        weaponsSelected.Clear();

        decreaseTicketCount(INFANTRY_BASE_COST - unitTotal);
        updatePurchasedTab();

        selectionPanel.SetActive(true);
        purchasePanel.SetActive(false);
        purchasePanel.transform.FindChild("StatAllocation").gameObject.SetActive(true);
        purchasePanel.transform.FindChild("WeaponSelect").gameObject.SetActive(false);
        previewPanel.SetActive(false);
    }

    public void purchaseTank()
    {
        // TODO: Provide feedback for fail
        if (purchasedUnits.Count >= 5 || currentTotal - TANK_BASE_COST < 0)
        {
            return;
        }

        Debug.Log("Tank Purchased");

        GameObject tank = Instantiate(Resources.Load("UTankRed")) as GameObject;
        purchasedUnits.Add(tank);

        decreaseTicketCount(TANK_BASE_COST - unitTotal);
        updatePurchasedTab();

        selectionPanel.SetActive(true);
        purchasePanel.SetActive(false);
        previewPanel.SetActive(false);
    }

    public void purchaseExo()
    {
        // TODO: Provide feedback for fail
        if (purchasedUnits.Count >= 5 || currentTotal - EXO_BASE_COST < 0)
        {
            return;
        }

        Debug.Log("Tank Purchased");

        GameObject infantry = Instantiate(Resources.Load("UTankBlue")) as GameObject;
        purchasedUnits.Add(infantry);

        decreaseTicketCount(EXO_BASE_COST - unitTotal);
        updatePurchasedTab();

        selectionPanel.SetActive(true);
        purchasePanel.SetActive(false);
        previewPanel.SetActive(false);

    }

    public void updatePurchasedTab()
    {
        Image[] sprites = purchasedTab.GetComponentsInChildren<Image>();

        for(int i = 1; i < sprites.Length; i++)
        {
            if (i - 1 < purchasedUnits.Count && purchasedUnits.Count != 0)
            {
                sprites[i].sprite = purchasedUnits[i-1].GetComponent<Unit>().sprite;
            }
            else
            {
                sprites[i].sprite = cancelSprite;
            }
        }
    }

    public void openPurchasedUnit(int i)
    {
        if(i > purchasedUnits.Count || purchasedUnits.Count == 0)
        {
            return;
        }

        Transform t = previewPanel.transform;
        t.FindChild("InfantryImage").FindChild("Image").GetComponent<Image>().sprite = purchasedUnits[i - 1].GetComponent<Unit>().sprite;

        // Stats
        t.FindChild("StatAllocation").FindChild("HealthStat").FindChild("HealthStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().energy.ToString();
        t.FindChild("StatAllocation").FindChild("AttackStat").FindChild("AttackStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().physAtk.ToString();
        t.FindChild("StatAllocation").FindChild("DefenceStat").FindChild("DefenceStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().defense.ToString();
        t.FindChild("StatAllocation").FindChild("SpeedStat").FindChild("SpeedStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().speed.ToString();

        // Weapons (Modify by selecting weapons from units)
        Transform weaponMenu = t.FindChild("WeaponSelect");

        weaponMenu.FindChild("Attack1").gameObject.SetActive(false);
        weaponMenu.FindChild("Attack2").gameObject.SetActive(false);
        weaponMenu.FindChild("Attack3").gameObject.SetActive(false);

        List<Weapon> unitWeapons = purchasedUnits[i - 1].GetComponent<Unit>().weapons;
        for(int j = 0; j < unitWeapons.Count; j++)
        {
            string weaponIndex = "Attack" + (j+1);
            weaponMenu.FindChild(weaponIndex).gameObject.SetActive(true);
            if (unitWeapons[j].type == Weapon.WeaponType.Physical)
            {
                weaponMenu.FindChild(weaponIndex).FindChild("WeaponType").FindChild("Image").GetComponentInChildren<Image>().sprite = bullet;
            }
            else
            {
                weaponMenu.FindChild(weaponIndex).FindChild("WeaponType").FindChild("Image").GetComponentInChildren<Image>().sprite = energySprite;
            }
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponName").GetComponentInChildren<Text>().text = unitWeapons[j].name;
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponAttack").GetComponentInChildren<Text>().text = unitWeapons[j].power.ToString();
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponRange").GetComponentInChildren<Text>().text = unitWeapons[j].rangeMin.ToString() + "-" + unitWeapons[j].rangeMax.ToString();
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponAccuracy").GetComponentInChildren<Text>().text = unitWeapons[j].accuracy.ToString();

        }

        previewPanel.SetActive(true);
        purchasePanel.SetActive(false);
        selectionPanel.SetActive(false);

    }

}
