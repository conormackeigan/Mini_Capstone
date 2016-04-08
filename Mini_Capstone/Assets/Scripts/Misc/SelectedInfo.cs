using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectedInfo : MonoBehaviour {

    public GameObject infoPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void init(Unit u)
    {
        Unit currentUnit = u;
        infoPanel.transform.FindChild("UnitName").GetComponent<Text>().text = currentUnit.unitName;
        infoPanel.transform.FindChild("InfantryImage").FindChild("Image").GetComponent<Image>().sprite = currentUnit.sprite;

        Transform stats = infoPanel.transform.FindChild("StatAllocation");
        stats.FindChild("HealthStat").FindChild("HealthStatCount").GetComponent<Text>().text = currentUnit.health.ToString();
        stats.FindChild("AttackStat").FindChild("AttackStatCount").GetComponent<Text>().text = currentUnit.energyAtk.ToString();
        stats.FindChild("DefenceStat").FindChild("DefenceStatCount").GetComponent<Text>().text = currentUnit.defense.ToString();
        stats.FindChild("SpeedStat").FindChild("SpeedStatCount").GetComponent<Text>().text = currentUnit.speed.ToString();

    }

    public void close()
    {
        infoPanel.SetActive(false);
    }
}
