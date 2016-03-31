using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class NextTurnRPC : Photon.MonoBehaviour
{
    [PunRPC]
    public void NextTurn(int t)
    {
        PlayerManager.Instance.currentPlayersTurn = t;
        if (t == PhotonNetwork.player.ID)
        {
            PlayerManager.Instance.TurnLabel.GetComponent<Text>().text = "Your Turn";
        }
        else
        {
            PlayerManager.Instance.TurnLabel.GetComponent<Text>().text = "Enemy Turn";
        }

        UIManager.Instance.animateTurnPanel();

    }
}
