using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class AddWeaponRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public void AddWeapon(string s)
    {
        Unit script = gameObject.GetComponent<Unit>();

        if (script.weapons == null)
        {
            script.weapons = new List<Weapon>();
        }
        switch (s)
        {
            case "Sword":
                {
                    Weapon sword = new BeamSword(script);
                    script.weapons.Add(sword);
                    script.Equip(sword);
                    break;
                }
            case "Rifle":
                {
                    Weapon rifle = new Rifle(script);
                    script.weapons.Add(rifle);
                    script.Equip(rifle);
                    break;
                }
            case "Frag":
                {
                    Weapon frag = new Frag(script);
                    script.weapons.Add(frag);
                    script.Equip(frag);
                    break;
                }
            case "Laser":
                {
                    Weapon laser = new LaserCannon(script);
                    script.weapons.Add(laser);
                    script.Equip(laser);
                    break;
                }
            case "Chain":
                {
                    Weapon chain = new EnergyChain(script);
                    script.weapons.Add(chain);
                    script.Equip(chain);
                    break;
                }
            case "Photon":
                {
                    Weapon photon = new PhotonEqualizer(script);
                    script.weapons.Add(photon);
                    script.Equip(photon);
                    break;
                }
            default:
                {
                    Debug.Log("Unit not found");
                    break;
                }
        }

    }
}