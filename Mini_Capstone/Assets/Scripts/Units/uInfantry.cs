using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uInfantry : Unit
{
    private Vector3 correctPlayerPos; // Networking Purposes

    protected override void Start()
    {
        base.Start();

        unitName = "Infantry";

        //actions.Add("Move");
        //actions.Add("Wait");
        //actions.Add("Attack");

        maxHealth = 10;
        health = 10;
        defense = 4;
        physAtk = 5;
        energyAtk = 3;
        speed = 4;
        movementRange = 4;
        flying = false;

        specialBattleAttributes = new List<UnitSpecial>();
        specialBoardAttributes = new List<UnitSpecial>();

        specialBoardAttributes.Add(new CharismaUnitSpecial(this));

        // WEAPON(S):
        weapons = new List<Weapon>();

        Weapon rifle = new Rifle(this);
        weapons.Add(rifle);
        Equip(rifle);
        Weapon beamsword = new BeamSword(this);
        weapons.Add(beamsword);
        Equip(beamsword);      
        Weapon frag = new Frag(this);
        weapons.Add(frag); 
    }

    protected override void Update()
    {
        base.Update();

        if (!photonView.isMine && GameDirector.Instance.numOfPlayers == 2)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
        }
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    public override void OnMouseClick()
    {
        base.OnMouseClick();

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(pos.x);
            stream.SendNext(pos.y);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            pos.x = (int)stream.ReceiveNext();
            pos.y = (int)stream.ReceiveNext();
        }
    }

}
