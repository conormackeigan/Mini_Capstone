using UnityEngine;
using System.Collections;

public class uTank : Unit
{

    protected override void Start()
    {
        base.Start();

        unitName = "Tank";

        //actions.Add("Move");
        //actions.Add("Wait");
        //actions.Add("Attack");

        //health = 5;
        //defense = 15;
        //attack = 1;

    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

    }
}
