using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class aiManager : Singleton<aiManager>
{
    public List<Unit> units;

	// Use this for initialization
	void Start ()
    {
        units = new List<Unit>();
	}
	
    public void getUnits()
    {
        units.Clear();

        foreach (GameObject go in ObjectManager.Instance.PlayerTwoUnits)
        {
            units.Add(go.GetComponent<Unit>());
        }
    }

    // get next unit in turn order
    public Unit getNext()
    {
        if (units.Count > 0)
        {
            Unit unit = units[0];
            units.RemoveAt(0);

            return unit;
        }
        else // enemy turn is done
        {
            return null;
        }
    }
}
