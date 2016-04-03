using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class StartUnitRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public void StartUnit(int playerID, int posX, int posY)
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponent<Unit>().playerID = playerID;
        this.gameObject.GetComponent<Unit>().Init();
        ObjectManager.Instance.addObjectAtPos(this.gameObject, new Vector2i(posX, posY));

    }
}
