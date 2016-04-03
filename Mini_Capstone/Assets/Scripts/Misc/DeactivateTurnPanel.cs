using UnityEngine;
using System.Collections;

public class DeactivateTurnPanel : MonoBehaviour {

    public void deactivateTurnPanel()
    {
        this.gameObject.SetActive(false);
        if(GameDirector.Instance.isMultiPlayer() && PhotonNetwork.player.ID == PlayerManager.Instance.getCurrentPlayerTurn())
        {
            GLOBAL.setLock(false);
        }
        else if (GameDirector.Instance.isSinglePlayer())
        {
            GLOBAL.setLock(false);

        }
    }
}
