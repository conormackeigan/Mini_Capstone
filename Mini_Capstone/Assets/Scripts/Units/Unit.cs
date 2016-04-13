using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class Unit : Photon.MonoBehaviour, IPointerClickHandler
{
    //----------------------------------------------------------------------------------------------------------------------
    // States:
    //----------------------------------------------------------------------------------------------------------------------
    public enum UnitState
    {
        Neutral, // has not moved yet, unselected
        Selected,
        Moving,
        Waiting, // reached new location, waiting for confirmation or cancel
        Combat,
        AoE, // state after pressing AoE button; does not revert until cancel or an AoE attack
        Damage, // damage state, controls quick flinch movement and drains healthbar in realtime
        Inactive, // greyed out for the turn
        NotTurn // not the unit's turn right now, no actions available
    }

    //----------------------------------------------------------------------------------------------------------------------
    // Attributes
    //----------------------------------------------------------------------------------------------------------------------
    public aiBase AI; // null for player units

    public string unitName; // Name of unit
    public string unitPrefabName; // Name of prefab used by unit
    protected GameObject unitPrefab; // Prefab used by unit

    public int playerID; // Player controlling unit

    public UnitState state = UnitState.Neutral;
    public bool animate = false;

    //MOVEMENT VARIABLES
    public List<Vector2i> path; // the path of nodes for this unit to travel (moving state only)
    public Vector2i prev; // previously processed node, for directions

    public List<Weapon> weapons; // weapons this unit starts with (attacks)
    public Weapon equipped = null; // which of this unit's weapons is currently equipped

    public int health;
    public bool flying;

    private int finalDmg; // amount of damage to administer over course of real-time dmg sequence
    private float timer; // timer for controlling damage sequence (animations would be p nice)
    private float prevTimer; // for controlling cycles
    private float interval; // time between hp-- intervals
    private UnitState prevState; // stores units previous state for reverting post-damage sequence

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

    public List<UnitSpecial> specials; // special attributes that affect interactions with other units on the board
    public List<Buff> buffs; // all buffs this unit is currently receiving (including debuffs)

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
        // specials<UnitSpecial> is initialized in the subtype Start() method

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
                ObjectManager.Instance.ObjectGrid[pos.x, pos.y] = null;
                gameObject.SetActive(false);
		        //destroyUnit();
                if(GameDirector.Instance.isMultiPlayer())
                {
                    gameObject.GetPhotonView().RPC("DestroyUnit", PhotonTargets.AllBuffered, pos.x, pos.y);
                }

                GLOBAL.setLock(false); // unlock user input
                if(ObjectManager.Instance.isGameOver())
                {
                    GameDirector.Instance.endGame();
                }
            }
        }

        // move along path state
        else if (state == UnitState.Moving)
        {
            Move();
        }

        else if (state == UnitState.Damage)
        {
            DamageSequence();
        }

        // temp: grey out on inactive
        else if (state == UnitState.Inactive)
        {
        //    gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
        }

        // TODO: move this color change to the frame this unit is reactivated
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    protected virtual void OnGUI()
    {

    }

    public virtual void Init(bool defaultStats = false)
    {

    }

    public virtual void OnMouseClick()
    {
        if (state == UnitState.Inactive)
        {
            //TODO: display unit's information before returning???
            return;
        }
        
        Player currentPlayer = PlayerManager.Instance.getCurrentPlayer();

        //===========================
        // Clicking on Ally Unit:
        //===========================
        if (currentPlayer.playerID == playerID) // player can't input when not turn so don't bother checking
        {
            if (currentPlayer.selectedObject != null)
            {
                // can only switch between units on tap in selected state
                if (currentPlayer.selectedObject.tag == "Unit")
                {
                    Unit selected = currentPlayer.selectedObject.GetComponent<Unit>();

                    if (selected.state != UnitState.Selected)
                    {
                        return;
                    }

                    selected.deselectUnit();

                    if (selected == this)
                    {
                        Camera.main.GetComponent<AudioSource>().PlayOneShot(GameDirector.Instance.sfxCancel); // not reselecting so play cancel chime
                        return; // don't reselect after deselecting if clicking selected unit
                    }
                }
            }

            selectUnit();
        }
        //===========================
        // Clicking on Enemy Unit:
        //===========================
        else
        {
            if (TileMarker.Instance.attackTiles.ContainsKey(pos) && PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>().state != UnitState.AoE) 
            {
                currentPlayer.selectedObject.GetComponent<Unit>().Attack(this);
            }
            else
            {
                UIManager.Instance.ActivateFriendPanel(this);
            }
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
        // disable UI until destination reached
        UIManager.Instance.setUnitUI(false);
        UIManager.Instance.deactivateAoEButton();

        path = new List<Vector2i>(); // tile nodes of the path to be travelled

        Tile curr = TerrainLayer.Instance.Tiles[destination.x, destination.y]; // destination node
        path.Add(curr.pos);

        while (curr.parent != null)
        {
            path.Add(curr.parent.pos);
            curr = curr.parent;
        }

        // path has been computed, travel to node at end of list until at destination
        state = UnitState.Moving; // set unit state
        GLOBAL.setLock(true); // lock user input until path is traversed
    }

    // select this unit
    public void selectUnit()
    {
        PlayerManager.Instance.getCurrentPlayer().selectedObject = gameObject;
        state = UnitState.Selected;
        selectPos = pos;

        // mark tiles this unit can attack and reach
        TileMarker.Instance.Clear();
        TileMarker.Instance.markTravTiles(this);
        TileMarker.Instance.markAttackTiles(this);

        UIManager.Instance.ActivateFriendPanel(this);

        if (playerID == 1 || GameDirector.Instance.isMultiPlayer())
        {
            UIManager.Instance.setUnitUI(true);
        }

        createOverlay();

        //if unit has at least one AoE weapon, make the AoE button available
        bool AoE = false;
        foreach(Weapon w in weapons)
        {
            if (w.AoE)
                AoE = true;            
        }
        if (AoE && (playerID == 1 || GameDirector.Instance.isMultiPlayer())) 
        {
            UIManager.Instance.activateAoEButton();
        }

        Camera.main.GetComponent<AudioSource>().PlayOneShot(GameDirector.Instance.sfxSelect);
    }

    // deselects unit, either after unit turn or as a result of the cancel button in selected state
    public void deselectUnit()
    {
        PlayerManager.Instance.getCurrentPlayer().selectedObject = null;

        if (state != UnitState.Inactive)
            state = UnitState.Neutral; // if deselecting after turn don't change state

        TileMarker.Instance.Clear(); //clean up tilemarker in case tiles were marked

        UIManager.Instance.deactivateAoEButton();
        UIManager.Instance.DeactivateFriendPanel();      
        UIManager.Instance.deactivateAttackButton();
        UIManager.Instance.deactivateAoEButton();
        UIManager.Instance.setUnitUI(false);
        destroyOverlay();
    }

    // reverts most recent action via cancel button (combat, waiting and selected states)
    public void ResetUnit()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(GameDirector.Instance.sfxCancel);

        // If no actions taken then deselect
        if (state == UnitState.Selected || state == UnitState.Inactive)
        {
            deselectUnit();
        }
        // If unit was waiting for confirmation, revert that action
        else if (state == UnitState.Waiting || state == UnitState.Combat)
        {
            if (CombatSequence.Instance.active)
            { // cancels combat. button should only be available if combat hasn't been initiated yet (no safeguard checks in place right now)
                CombatSequence.Instance.Cancel();
            }

            TileMarker.Instance.Clear(); // remarking trav and/or attack tiles so clear current cache
            if (state == UnitState.Waiting || pos == selectPos)
            { // revert to selected if cancelling a move or unmoved combat
                if (state == UnitState.Combat)
                {
                    UIManager.Instance.deactivateAttackButton();
                }
                
                //state = UnitState.Selected;
                ObjectManager.Instance.moveUnitToGridPos(PlayerManager.Instance.getCurrentPlayer().selectedObject, selectPos);
                GameDirector.Instance.BoardStateChanged(); // update buffs
                //TileMarker.Instance.markTravTiles(this);
                selectUnit();
            }
            else
            { // UnitState.Combat post-move, revert to waiting
                UIManager.Instance.deactivateAttackButton();
                state = UnitState.Waiting;
                GameDirector.Instance.BoardStateChanged(); // update buffs
                TileMarker.Instance.markAttackTiles(this);
            }

        }
        //==========================================
        // AoE Reset Cases:
        //==========================================
        else if (state == UnitState.AoE)
        {
            // just force unit to selected state regardless of phase if in AoE state
            CombatSequence.Instance.CancelAoE();
            UIManager.Instance.deactivateAttackButton();

            selectUnit();
        }
    }

    // sets unit to inactive
    public void Deactivate()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
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

                    if (isHuman() || GameDirector.Instance.isMultiPlayer())
                    {
                        UIManager.Instance.setUnitUI(true); // display UI now that inputs are 
                    }

                    ObjectManager.Instance.moveUnitToGridPos(gameObject, prev);
                    TileMarker.Instance.markAttackTiles(this);
                    GameDirector.Instance.BoardStateChanged();

                    if (!isHuman())
                    {
                        AI.ReachedDestination();
                    }
                    else
                    {
                        GLOBAL.setLock(false); // inputs are no longer locked
                    }
                }
            }
            else
            { // haven't reached next node in path yet, continue lerping
                Vector2i dir = path[path.Count - 1] - prev;// next node in path is guaranteed to be neighbour, so displacement vector = dir vector
                transform.Translate((dir * (2 * (1 + Convert.ToInt32(Input.GetKey(KeyCode.Space))))) * (Time.deltaTime * 100)); // double speed if button held (space for temporarily)
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
        //ApplySpecials();
    }


    // returns true if this is a human-controlled unit
    public bool isHuman()
    {
        if (PlayerManager.Instance.players[playerID - 1].GetComponent<Player>().playerType == Player.PLAYER_TYPE.Human)
        {
            return true;
        }

        return false;
    }


    // same as GameDirector.BoardStateChanged() but for this unit only
    public void ApplySpecials()
    {
        // remove buffs as they will all be readded if they're still applicable
        if (buffs != null)
        {
            for (int i = 0; i < buffs.Count; i--)
            {
                Debug.Log(buffs[i].GetType() + " i: " + i);
                buffs[i].Destroy();       
            }
        }
        
        // Unit Specials:
        if (specials != null)
        {
            foreach (UnitSpecial s in specials)
            {
                if (s.condition != null)
                {
                    if (s.condition.eval())
                    {
                        s.effect();
                    }
                }
                else
                {
                    s.effect();
                }
            }
        }
       
        // Weapon Specials:
        if (equipped != null)
        {
            if (equipped.specials != null)
            {
                foreach (Special s in equipped.specials)
                {
                    if (s.condition != null)
                    {
                        if (s.condition.eval())
                        {
                            s.effect();
                        }
                    }
                    else
                    {
                        s.effect();
                    }
                }
            }
        }

    }


    // this method is called when a marked enemy tile is clicked
    public void Attack(Unit other)
    {
        // PRELIMINARY ATTACK CALCULATIONS (PRE-CONFIRM)
        TileMarker.Instance.Clear();
        UIManager.Instance.deactivateAoEButton();

        state = UnitState.Combat;

        // calculate units' combat stats
        GameDirector.Instance.BoardStateChanged(); // update buff stats for combat specials
        calcCombatStats();
        other.calcCombatStats();

        // enable combat sequence
        GameObject combatSequence = GameObject.Find("CombatSequence");
        combatSequence.GetComponent<CombatSequence>().active = true;
        combatSequence.GetComponent<CombatSequence>().Enable(this, other);
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

    // begins damage sequence
    public void Damage(int dmg)
    {
        GLOBAL.setLock(true);

        timer = 0;
        prevTimer = 0;
        // TODO: enable mini healthbar under board unit here (or always enabled if not too obstructive)

        if (health - dmg < 0)
        { // if there is overkill use it to offset final dmg amount
            finalDmg = dmg + (health - dmg);
        }
        else
        {
            finalDmg = dmg;
        }

        // get interval
        interval = GLOBAL.dmgTime / finalDmg;

        // DAMAGE TEXT:
        if (finalDmg < 1)
        {
            // TODO: instantiate "NO DAMAGE" text
        }
        else
        {
            // TODO: instantiate "-finalDmg" text
        }

        prevState = state; // store current state to revert after sequence
        state = UnitState.Damage;
    }

    

    // real-time damage sequence
    public void DamageSequence()
    {
        if (finalDmg > 0)
        {
            if (timer == 0)
            {
                health--;
            }

            if (timer - prevTimer >= interval)
            {
                prevTimer = timer;
                prevTimer -= (timer % interval); // round prevTimer to nearest interval multiple
                health--;
            }

            // flinch motion
            if (timer < 0.06f)
            {
                if (CombatSequence.Instance.active)
                {
                    transform.position -= CombatSequence.Instance.displacement * Time.deltaTime * GLOBAL.attackSpeed * 0.5f;
                }
                else
                {
                    transform.position += Vector3.left * Time.deltaTime * GLOBAL.attackSpeed * 0.75f;
                }
            }
            else if (timer < 0.12f)
            {
                if (CombatSequence.Instance.active)
                {
                    transform.position += CombatSequence.Instance.displacement * Time.deltaTime * GLOBAL.attackSpeed * 0.5f;
                }
                else
                {
                    transform.position += Vector3.right * Time.deltaTime * GLOBAL.attackSpeed * 0.75f;
                }
            }
            // done flinch
            else
            {
                snapToGridPos();
            }
        }

        timer += Time.deltaTime;

        if (timer >= GLOBAL.dmgTime)
        {
            state = prevState;

            if (!CombatSequence.Instance.active)
            { // combat sequences have death checks integrated, only check if not combat (and unlock input)
                CheckDead();
                GLOBAL.setLock(false);
            }
        }
    }

    public bool CheckDead()
    {
        if (health <= 0)
        {
            isDead = true;
            GLOBAL.setLock(true);

            return true;
        }

        return false;
    }

    //==========================================
    // Area of Effect:
    //==========================================
    public void AoEBegin()
    {
        state = UnitState.AoE;
       
        TileMarker.Instance.Clear(); // clear all markers from selected state

        // ***new sequence*** menu select for AoE weapon, purple markers to aim, red markers for confirm
        CombatSequence.Instance.AoEWeaponSelect(pos);
        CombatSequence.Instance.attacker = this;
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

    public void destroyUnit()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GLOBAL.locked() == false)
        {
            OnMouseClick();
        }
    }
}
