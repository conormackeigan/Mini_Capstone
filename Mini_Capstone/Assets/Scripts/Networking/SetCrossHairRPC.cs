using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class SetCrossHairRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public void SetCrossHair(int posX, int posY)
    {
        this.gameObject.GetComponent<CrosshairsController>().target.x = posX;
        this.gameObject.GetComponent<CrosshairsController>().target.y = posY;

        this.gameObject.GetComponent<CrosshairsController>().displacement = GLOBAL.gridToWorld(this.gameObject.GetComponent<CrosshairsController>().target) - transform.position;
    }
}
