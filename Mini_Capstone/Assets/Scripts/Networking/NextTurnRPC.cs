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
            PlayerManager.Instance.TurnLabelTop.GetComponent<Text>().text = "Your Turn";
            PlayerManager.Instance.TurnLabel.GetComponent<Text>().text = "Your Turn";
            PlayerManager.Instance.EndTurnButton.GetComponent<Button>().enabled = true;
            GLOBAL.setLock(false);
        }
        else
        {
            PlayerManager.Instance.TurnLabelTop.GetComponent<Text>().text = "Enemy Turn";
            PlayerManager.Instance.TurnLabel.GetComponent<Text>().text = "Enemy Turn";
            PlayerManager.Instance.EndTurnButton.GetComponent<Button>().enabled = false;
            GLOBAL.setLock(true);
        }

        UIManager.Instance.animateTurnPanel();

    }
}
