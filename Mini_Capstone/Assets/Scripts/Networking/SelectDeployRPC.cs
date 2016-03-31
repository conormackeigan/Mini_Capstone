using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class SelectDeployRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public void SelectDeploy(int t)
    {
        if (t == 1)
        {
            UnitSelection.Instance.playerOneDeploy = true;
        }
        else
        {
            UnitSelection.Instance.playerTwoDeploy = true;
        }
    }
}
