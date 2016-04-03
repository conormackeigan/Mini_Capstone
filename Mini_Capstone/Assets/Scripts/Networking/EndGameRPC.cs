using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class EndGameRPC : Photon.MonoBehaviour
{
    public GameObject networking;

    [PunRPC]
    public IEnumerator EndGameNetwork()
    {
        Debug.Log("Ending Game for player: " + PhotonNetwork.playerName);

        UnitSelection.Instance.Reset();
        PlayerManager.Instance.endGame();
        TerrainLayer.Instance.endGame();
        ObjectManager.Instance.endGame();

        yield return 0;
        networking.GetComponent<NetworkingMain>().Disconnect();
    }
}
