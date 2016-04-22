using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : Singleton<AIManager>
{
    public List<Unit> units;
    private Unit currUnit;

    void Start()
    {
        units = new List<Unit>();
        Debug.Log("units initialized");
    }

    // sets AI Manager active (beginning of computer turn)
    public void startEnemyTurn()
    {
        getUnits();
        getNext().AI.StartTurn();
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
    public Unit getNext(bool remove = true)
    {
        Debug.Log("get next units count: " + units.Count);
        if (units.Count > 0)
        {
            Unit unit = units[0];

            if (remove)
            {
                units.RemoveAt(0);
            }

            return unit;

        }
        else // enemy turn is done
        {
            return null;
        }
    }

    public void callAction(Unit u)
    {
        currUnit = u;

        Invoke("receiveActionCall", 0.75f);
    }

    public void receiveActionCall()
    {
        currUnit.AI.SelectAction();
        Debug.Log("received call");
    }


    public void callNextUnit()
    {
        Invoke("receiveNextUnitCall", 0.75f);
    }

    public void receiveNextUnitCall()
    {
        Unit unit = getNext();

        if (unit != null)
        {
            unit.AI.StartTurn();
        }
    }
}
