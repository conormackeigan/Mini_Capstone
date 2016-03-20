using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class EndGameRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public IEnumerator EndGameNetwork()
    {
        Debug.Log("Ending Game for player: " + PhotonNetwork.playerName);

        PlayerManager.Instance.endGame();
        TerrainLayer.Instance.endGame();
        ObjectManager.Instance.endGame();

        GameDirector.Instance.gameState = GameDirector.GameState.MAINMENU;

        yield return 0;

    }
}
