using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectedInfo : MonoBehaviour {

    public GameObject infoPanel;
    public Sprite energy;
    public Sprite bullet;

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


        // Weapons (Modify by selecting weapons from units)
        Transform weaponMenu = infoPanel.transform.FindChild("WeaponSelect");

        weaponMenu.FindChild("Attack1").gameObject.SetActive(false);
        weaponMenu.FindChild("Attack2").gameObject.SetActive(false);
        weaponMenu.FindChild("Attack3").gameObject.SetActive(false);

        List<Weapon> unitWeapons = u.weapons;
        for (int j = 0; j < unitWeapons.Count; j++)
        {
            string weaponIndex = "Attack" + (j + 1);
            weaponMenu.FindChild(weaponIndex).gameObject.SetActive(true);
            if (unitWeapons[j].type == Weapon.WeaponType.Physical)
            {
                weaponMenu.FindChild(weaponIndex).FindChild("WeaponType").FindChild("Image").GetComponentInChildren<Image>().sprite = bullet;
            }
            else
            {
                weaponMenu.FindChild(weaponIndex).FindChild("WeaponType").FindChild("Image").GetComponentInChildren<Image>().sprite = energy;
            }
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponName").GetComponentInChildren<Text>().text = unitWeapons[j].name;
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponAttack").GetComponentInChildren<Text>().text = unitWeapons[j].power.ToString();
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponRange").GetComponentInChildren<Text>().text = unitWeapons[j].rangeMin.ToString() + "-" + unitWeapons[j].rangeMax.ToString();
            weaponMenu.FindChild(weaponIndex).FindChild("WeaponAccuracy").GetComponentInChildren<Text>().text = unitWeapons[j].accuracy.ToString();

        }
    }

    public void close()
    {
        infoPanel.SetActive(false);
    }
}
