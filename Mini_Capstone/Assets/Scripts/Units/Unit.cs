using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class Unit : Photon.MonoBehaviour, IPointerClickHandler
{
    //----------------------------------------------------------------------------------------------------------------------
    // States (TODO: Make FSM, these states are temporary and probably not all necessary)
    //----------------------------------------------------------------------------------------------------------------------
    public enum UnitState
    {
        Neutral, // has not moved yet, unselected
        Selected,
        Moving,
        Waiting, // reached new location, waiting for confirmation or cancel
        Combat,
        Inactive, // greyed out for the turn
        NotTurn // not the unit's turn right now, no actions available
    }

    //----------------------------------------------------------------------------------------------------------------------
    // Attributes
    //----------------------------------------------------------------------------------------------------------------------

    public string unitName; // Name of unit
    public string unitPrefabName; // Name of prefab used by unit
    protected GameObject unitPrefab; // Prefab used by unit

    public int playerID { get; set; } // Player controlling unit
    public List<string> actions; // Available actions for unit
    //public bool currentlySelected = false;

    public UnitState state = UnitState.Neutral;
    //public bool isActive = true;
    public bool animate = false;

    //MOVEMENT VARIABLES
    public List<Vector2i> path; // the path of nodes for this unit to travel (moving state only)
    public Vector2i prev; // previously processed node, for directions

    public List<Weapon> weapons; // weapons this unit starts with (attacks)
    public Weapon equipped = null; // which of this unit's weapons is currently equipped

    public int health;
    public bool flying;

    // natural stats (unit base)
    public int maxHealth;
    public int defense;
    public int physAtk;
    public int energyAtk;
    public int speed; // affects accuracy and evasion
    public int movementRange;

    // buff offsets
    public int healthBuff;
    public int defenseBuff;
    public int physAtkBuff;
    public int energyAtkBuff;
    public int speedBuff;
    public int movementBuff;

    // final stats
    public int effectiveHealth;
    public int effectiveDefense;
    public int effectivePhysAtk;
    public int effectiveEnergyAtk;
    public int effectiveSpeed;
    public int effectiveMovementRange;

    // combat stats (final stats + weapon/terrain)
    public int combatHealth;
    public int combatDefense;
    public int combatPhysAtk;
    public int combatEnergyAtk;
    public int combatSpeed;
    public int combatAccuracy; // function of speed and other factors
    public int combatEvasion; // function of speed and other factors

    public List<UnitSpecial> specialBoardAttributes; // special attributes that affect interactions with other units on the board
    public List<UnitSpecial> specialBattleAttributes; // special attributes that affect interactions with other units in battle sequences
    public List<Buff> buffs; // all buffs this unit is currently receiving
    public List<Buff> debuffs; // all debuffs this unit is currently receiving

    public Vector2i pos; //this unit's current position in grid coords
    public Vector2i Pos { get { return pos; } set { pos = value; } }
    public Vector2i selectPos; // stores original pos before committing a pending move

    public Sprite sprite; //this will be an animation controller; static sprite for now
    public GameObject overlay; // Temp holder for overlay asset on selection

    public bool isDead = false;
    Color color; // for fadeout on death

    //----------------------------------------------------------------------------------------------------------------------
    // Methods
    //----------------------------------------------------------------------------------------------------------------------

    //====================
    // Virtual Functions
    //====================
    protected virtual void Start()
    {
        //===================
        // Initialization:
        //===================
        buffs = new List<Buff>();
        debuffs = new List<Buff>();

        state = UnitState.Neutral;
        overlay = null;

        //assign sprite to sprite renderer component
        if (sprite != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            color = gameObject.GetComponent<SpriteRenderer>().color;
        }
    }

    protected virtual void Update()
    {
        // fade out on death
        if (isDead)
        {
            if (gameObject.GetComponent<SpriteRenderer>().color.a > 0)
            {
                color.a -= Time.deltaTime;
                gameObject.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                if (GameDirector.Instance.numOfPlayers == 1)
                {
                    ObjectManager.Instance.ObjectGrid[pos.x, pos.y] = null;
                    //Destroy(gameObject);
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.GetPhotonView().RPC("DestroyUnit", PhotonTargets.AllBuffered);
                }
            }
        }

        // move along path state
        else if (state == UnitState.Moving)
        {
            Move();
        }

        // temp: grey out on inactive
        else if (state == UnitState.Inactive)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        }

        // temp: white on inactive->active (and default)
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }      
    }

    protected virtual void OnGUI()
    {

    }

    public virtual void OnMouseClick()
    {
        // TODO: implement spectator mode when not player's turn

        Player currentPlayer = PlayerManager.Instance.getCurrentPlayer();
        // if there is a selected unit in moving state, gtfo dont process stuff
        if (currentPlayer.selectedObject != null)
        {
            if (currentPlayer.selectedObject.tag == "Unit")
            {
                if (currentPlayer.selectedObject.GetComponent<Unit>().state == Unit.UnitState.Moving)
                {
                    return;
                }
            }
        }

        if (state != UnitState.Inactive)
        {
            selectUnit();
        }
    }

    //================
    // Grid Functions
    //================

    // forces unit's coordinates immediately to grid position
    public void snapToGridPos()
    {
        transform.position = new Vector3(pos.x * (int)IntConstants.TileSize, pos.y * (int)IntConstants.TileSize, 0);
    }

    // forces unit's transform immediately to a specified grid coordinate
    public void snapToPos(Vector2i newPos)
    {
        transform.position = new Vector3(newPos.x * (int)IntConstants.TileSize, newPos.y * (int)IntConstants.TileSize, 0);
    }

    // makes the unit travel to its destination
    public void TravelToPos(Vector2i destination)
    {
        path = new List<Vector2i>(); // tile nodes of the path to be travelled

        Tile curr = TerrainLayer.Instance.Tiles[destination.x, destination.y]; // destination node
        path.Add(curr.pos);

        while (curr.parent != null)
        {
            path.Add(curr.parent.pos);
            curr = curr.parent;
        }

        // path has been computed, travel to node at end of list until at destination
        state = UnitState.Moving;// set unit state
    }

    // this method handles all cases of clicking on an active unit. TODO: separate logic so this method only selects
    public void selectUnit()
    {
        // If the unit selected belongs to current player
        int currentTurn = PlayerManager.Instance.getCurrentPlayerTurn();
        Player currentPlayer = PlayerManager.Instance.getCurrentPlayer();
        if (currentTurn == playerID && currentPlayer.playerID == playerID)
        {
            // If same object selected then deselect
            if (currentPlayer.selectedObject == this.gameObject)
            {
                // perhaps we should expand unit info (weapons) when clicking on selected unit?

                //currentPlayer.selectedObject.GetComponent<Unit>().ResetUnit(true);
                //deselectUnit();
            }
            else
            {
                // If another object is currently selected, select this unit instead
                if (currentPlayer.selectedObject != null)
                {
                    // cannot interact with other ally units while a unit is selected
                    return;

                    //if (currentPlayer.selectedObject.tag == "Unit")
                    //{
                    //    if (currentPlayer.selectedObject.GetComponent<Unit>().state != UnitState.Waiting)
                    //    { // only select if the currently selected unit is not waiting
                    //        currentPlayer.selectedObject.GetComponent<Unit>().ResetUnit();
                    //        currentPlayer.selectedObject.GetComponent<Unit>().deselectUnit();
                    //    }
                    //}
                }

                currentPlayer.selectedObject = gameObject;
                state = UnitState.Selected;
                selectPos = pos;

                // mark tiles this unit can attack and reach
                TileMarker.Instance.Clear();
                TileMarker.Instance.markTravTiles(this);
                TileMarker.Instance.markAttackTiles(this);

                UIManager.Instance.ActivateFriendPanel(this);
                UIManager.Instance.setUnitUI(true);

                createOverlay();
            }
        }
        // Attack unit with currently selected
        else if (TileMarker.Instance.attackTiles.ContainsKey(pos))
        {
            UIManager.Instance.activateAttackButton();
            currentPlayer.selectedObject.GetComponent<Unit>().Attack(this);
        }
    }

    // deselects unit, either after unit turn or as a result of the cancel button in selected state
    public void deselectUnit()
    {
        PlayerManager.Instance.getCurrentPlayer().selectedObject = null;
        if (state != UnitState.Inactive)
        {
            state = UnitState.Neutral;
        }

        //clean up tilemarker in case tiles were marked
        TileMarker.Instance.Clear();

        UIManager.Instance.DeactivateFriendPanel();
        UIManager.Instance.deactivateAttackButton();
        UIManager.Instance.setUnitUI(false);

        destroyOverlay();
    }

    // reverts most recent action via cancel button
    public void ResetUnit()
    {
        // If no actions taken then deselect
        if (state == UnitState.Selected || state == UnitState.Inactive)
        {
            deselectUnit();
        }
        // If unit was waiting for confirmation, revert that action
        else if (state == UnitState.Waiting || state == UnitState.Combat)
        {
            if (GameObject.Find("CombatSequence").GetComponent<CombatSequence>().active)
            { // cancels combat. button should only be available if combat hasn't been initiated yet (no safeguard checks in place right now)
                GameObject.Find("CombatSequence").GetComponent<CombatSequence>().Cancel();              
            }

            state = UnitState.Selected;
            ObjectManager.Instance.moveUnitToGridPos(PlayerManager.Instance.getCurrentPlayer().selectedObject, selectPos);
            GameDirector.Instance.BoardStateChanged(); // update buffs

            TileMarker.Instance.Redraw(this);
        }

    }

    // sets unit to inactive
    public void Deactivate()
    {
        state = UnitState.Inactive;
        deselectUnit();
    }

    // moves a unit along a path
    void Move()
    {
        if (path.Count > 0)
        {
            if (path[path.Count - 1].Distance(transform.position) <= 4) // magic number: move 2-4 pixels per frame. prevents passing destination (i hope.............)
            { // reached next node in list, update list and proceed to next destination
                snapToPos(path[path.Count - 1]); // snap to exact position before continuing
                prev = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);

                if (path.Count == 0)
                { // reached destination
                    state = UnitState.Waiting;
                    ObjectManager.Instance.moveUnitToGridPos(gameObject, prev);
                    TileMarker.Instance.markAttackTiles(this);
                    GameDirector.Instance.BoardStateChanged();
                }
            }
            else
            { // haven't reached next node in path yet, continue lerping
                Vector2i dir = path[path.Count - 1] - prev;// next node in path is guaranteed to be neighbour, so displacement vector = dir vector
                transform.Translate( (dir * (2 * (1 + Convert.ToInt32(Input.GetKey(KeyCode.Space))))) * (Time.deltaTime * 100) ); // double speed if button held (space for temporarily)
            }
        }
    }

    public void Equip(Weapon w)
    {
        if (!weapons.Contains(w))
        {
            Debug.Log("dragons (equipping a weapon the unit doesn't own");
            return; // can't equip a weapon that we don't have (this should never be called...)            
        }

        equipped = w;
        GameDirector.Instance.BoardStateChanged();
    }
    
    // this method is called when a marked enemy tile is clicked
    public void Attack(Unit other)
    {
        // PRELIMINARY ATTACK CALCULATIONS (PRE-CONFIRM)

        TileMarker.Instance.Clear();

        // TODO: develop dmg calculation formula

        state = UnitState.Combat;

        // calculate units' combat stats
        GameDirector.Instance.BoardStateChanged(); // update buff stats for combat specials
        calcCombatStats();
        other.calcCombatStats();

        // enable combat sequence
        GameObject combatSequence = GameObject.Find("CombatSequence");
        combatSequence.GetComponent<CombatSequence>().active = true;
        combatSequence.GetComponent<CombatSequence>().Enable(this, other);

        // **obsolete: handled in combatsequence enable now** crosshair lockon (calls StartCombatSequence() on complete) 
        //GameObject crosshairs = Instantiate(Resources.Load("Crosshairs"), GLOBAL.gridToWorld(pos), Quaternion.identity) as GameObject;
        //crosshairs.GetComponent<CrosshairsController>().target = other.pos;
    }
    
    // called after Attack() and crosshair movement. Prepares a battle sequence for confirmation/cancel
    public void StartCombatSequence()
    {
        // Hook up proper references and enable combat sequence controller

    }

    //update stats w/ buffs
    public void UpdateStats()
    {
        effectiveHealth = maxHealth + healthBuff;
        effectivePhysAtk = physAtk + physAtkBuff;
        effectiveEnergyAtk = energyAtk + energyAtkBuff;
        effectiveDefense = defense + defenseBuff;
        effectiveSpeed = speed + speedBuff;
        effectiveMovementRange = movementRange + movementBuff;
    }

    public void calcCombatStats()
    {
        combatHealth = effectiveHealth;
        combatPhysAtk = effectivePhysAtk;
        combatEnergyAtk = effectiveEnergyAtk;
        combatDefense = effectiveDefense + TerrainLayer.Instance.Tiles[pos.x, pos.y].def; // apply terrain bonus/debuff to def
        combatSpeed = effectiveSpeed;
        combatAccuracy = effectiveSpeed;
        combatEvasion = effectiveSpeed + TerrainLayer.Instance.Tiles[pos.x, pos.y].eva; // apply terrain bonus/debuff to eva
    }

    // deals damage to this unit and returns whether or not it died
    public void Damage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            isDead = true;
        }
    }

    public void createOverlay()
    {
        if (overlay == null)
        {
            overlay = Instantiate(Resources.Load("SelectOverlay")) as GameObject;

            overlay.transform.parent = gameObject.transform;
            overlay.transform.position = gameObject.transform.position;
        }
    }

    public void destroyOverlay()
    {
        Destroy(overlay);
        overlay = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick();
    }
}
