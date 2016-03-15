using UnityEngine;
using System.Collections;

public class sBarracks : Structure {

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        structureName = "Barracks";

        units.Add("Infantry");
        unitPrefabs.Add("UInfantry");

        health = 1;
        defense = 1;

        //TODO: Set the player that owns this unit upon creation (should be done by object which creates the unit)
        // player = ???;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        PlayerSelect();
        PlayerInteract(); // TEST CODE; REMOVE THIS

    }

    protected override void OnGUI()
    {
        base.OnGUI();

    }

    // Selects the unit by player
    void PlayerSelect()
    {
        // Converting Mouse Pos to 2D (vector2) World Pos
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // TODO: Ensure that current player can select unit (i.e correct team)
        if (hit.collider && hit.collider.gameObject.transform.position == gameObject.transform.position)
        {
            currentlySelected = !currentlySelected;
        }
        else
        {
            // TODO: Fix grid position logic before turning this back on
            //currentlySelected = false; 
        }
    }

    //================================================================
    // TEST CODE ONLY 
    void PlayerInteract()
    {
        if (Input.GetMouseButtonDown(0) && currentlySelected)
        {
            Instantiate(Resources.Load(unitPrefabs[0]));
            currentlySelected = false;
        }
    }
    // REMOVE THIS
    //================================================================
}
