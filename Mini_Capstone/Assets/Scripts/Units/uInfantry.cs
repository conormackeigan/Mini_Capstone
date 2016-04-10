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

        if (GameDirector.Instance.isSinglePlayer() && playerID == 2)
        {
            AI = new aiInfantry(this);
        }
        else
        {
            AI = null;
        }

        flying = false;

        specials = new List<UnitSpecial>();

        specials.Add(new CharismaUnitSpecial(this));
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

    public override void Init(bool defaultStats = false)
    {
        base.Init(defaultStats);

        if (defaultStats)
        {
            maxHealth = 10;
            health = 10;
            defense = 4;
            physAtk = 5;
            energyAtk = 10;
            speed = 4;
            movementRange = 4;


            // WEAPON(S):
            weapons = new List<Weapon>();

            
            Weapon beamsword = new BeamSword(this);
            weapons.Add(beamsword);
            Equip(beamsword);
            Weapon rifle = new Rifle(this);
            weapons.Add(rifle);
            Equip(rifle);
            Weapon sniper = new SniperRifle(this);
            weapons.Add(sniper);
            //weapons.Add(new LaserCannon(this));
            //weapons.Add(new PhotonEqualizer(this));
            //weapons.Add(new EnergyChain(this));
        }

        UpdateStats();
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
            // Position
            stream.SendNext(transform.position);
            stream.SendNext(pos.x);
            stream.SendNext(pos.y);

            // Player Stats
            stream.SendNext(maxHealth);
            stream.SendNext(health);
            stream.SendNext(defense);
            stream.SendNext(physAtk);
            stream.SendNext(energyAtk);
            stream.SendNext(speed);
            stream.SendNext(movementRange);

        }
        else
        {
            // Network player, receive data
            // Position
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            pos.x = (int)stream.ReceiveNext();
            pos.y = (int)stream.ReceiveNext();

            // Player Stats
            maxHealth = (int)stream.ReceiveNext();
            health = (int)stream.ReceiveNext();
            defense = (int)stream.ReceiveNext();
            physAtk = (int)stream.ReceiveNext();
            energyAtk = (int)stream.ReceiveNext();
            speed = (int)stream.ReceiveNext();
            movementRange = (int)stream.ReceiveNext();

        }
    }

}
