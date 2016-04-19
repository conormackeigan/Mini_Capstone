using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeactivateTurnPanel : MonoBehaviour {

    public Button EndturnButton;

    public void deactivateTurnPanel()
    {
        EndturnButton.enabled = true;

        this.gameObject.SetActive(false);
        if(GameDirector.Instance.isMultiPlayer() && PhotonNetwork.player.ID == PlayerManager.Instance.getCurrentPlayerTurn())
        {
            GLOBAL.setLock(false);
        }
        else if (GameDirector.Instance.isSinglePlayer())
        {
            if (PlayerManager.Instance.getCurrentPlayer().playerID == 1)
            {
                GLOBAL.setLock(false);
            }
            else
            {
                AIManager.Instance.startEnemyTurn();
            }
        }
    }

    public void lockEndTurn()
    {
        EndturnButton.enabled = false;
    }
}
