using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class DestroyUnitRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public IEnumerator DestroyUnit(int x, int y)
    {
        GameObject unit = ObjectManager.Instance.ObjectGrid[x, y];
        ObjectManager.Instance.ObjectGrid[x, y] = null;

        if(unit.GetComponent<Unit>().playerID == 1)
        {
            ObjectManager.Instance.PlayerOneUnits.Remove(unit);
        }
        else
        {
            ObjectManager.Instance.PlayerTwoUnits.Remove(unit);
        }

        GameObject.Destroy(this.gameObject);
        yield return 0; // if you allow 1 frame to pass, the object's OnDestroy() method gets called and cleans up references.
        PhotonNetwork.UnAllocateViewID(this.photonView.viewID);
    }
}
