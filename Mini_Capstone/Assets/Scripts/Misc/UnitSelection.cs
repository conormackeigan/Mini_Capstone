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
    public string selectedUnit;
    public Sprite cancelSprite;

    public GameObject purchasedTab;
    public GameObject ticketCount;
    public GameObject selectionPanel;
    public GameObject purchasePanel;
    public GameObject previewPanel;

    public List<GameObject> purchasedUnits;

    // Stats Panel
    int statPoints;
    int health;
    int attack;
    int defence;
    int speed;

	// Use this for initialization
	void Start () {
        purchasedUnits = new List<GameObject>();
        currentTotal = 500;

        ticketCount.GetComponent<Text>().text = currentTotal.ToString();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void Deploy()
    {
        GameDirector.Instance.startGame();
    }

    public void Reset()
    {
        foreach (GameObject g in purchasedUnits)
        {
            Destroy(g);
        }

        purchasedUnits = new List<GameObject>();
        currentTotal = 500;
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

        selectedUnit = "Infantry";

        statPoints = 5;
        health = 5;
        attack = 5;
        defence = 5;
        speed = 5;

        updateHealth(0);
        updateAttack(0);
        updateDefence(0);
        updateSpeed(0);

    }

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
                    updateHealth(1);
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
                    updateHealth(-1);
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

    void updateHealth(int i)
    {
        if ((i == 1 && health == 10) || (i == -1 && health == 1))
        {
            return;
        }

        health += i;
        statPoints -= i;

        GameObject.Find("HealthSlider").GetComponent<Slider>().value = (float) health;
        GameObject.Find("HealthStatCount").GetComponent<Text>().text = health.ToString();
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

    public void purchaseInfantry()
    {
        // TODO: Provide feedback for fail
        if (purchasedUnits.Count >= 5 || currentTotal - INFANTRY_BASE_COST < 0)
        {
            return;
        }

        Debug.Log("Infantry Purchased");

        GameObject infantry = Instantiate(Resources.Load("UInfantryRed")) as GameObject;
        purchasedUnits.Add(infantry);
        uInfantry script = infantry.GetComponent<uInfantry>();

        script.maxHealth = health;
        script.health = health;
        script.defense = health;
        script.physAtk = attack;
        script.energyAtk = attack;
        script.speed = speed;

        decreaseTicketCount(INFANTRY_BASE_COST);
        updatePurchasedTab();

        selectionPanel.SetActive(true);
        purchasePanel.SetActive(false);
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

        decreaseTicketCount(TANK_BASE_COST);
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

        decreaseTicketCount(EXO_BASE_COST);
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
            if (i - 1 < purchasedUnits.Count)
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
        if(i > purchasedUnits.Count)
        {
            return;
        }

        Transform t = previewPanel.transform;
        t.FindChild("InfantryImage").FindChild("Image").GetComponent<Image>().sprite = purchasedUnits[i - 1].GetComponent<Unit>().sprite;
        t.FindChild("StatAllocation").FindChild("HealthStat").FindChild("HealthStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().health.ToString();
        t.FindChild("StatAllocation").FindChild("AttackStat").FindChild("AttackStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().physAtk.ToString();
        t.FindChild("StatAllocation").FindChild("DefenceStat").FindChild("DefenceStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().defense.ToString();
        t.FindChild("StatAllocation").FindChild("SpeedStat").FindChild("SpeedStatCount").GetComponent<Text>().text = purchasedUnits[i - 1].GetComponent<Unit>().speed.ToString();

        previewPanel.SetActive(true);
        purchasePanel.SetActive(false);
        selectionPanel.SetActive(false);

    }
}
