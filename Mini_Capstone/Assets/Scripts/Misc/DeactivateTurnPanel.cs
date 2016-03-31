using UnityEngine;
using System.Collections;

public class DeactivateTurnPanel : MonoBehaviour {

    public void deactivateTurnPanel()
    {
        this.gameObject.SetActive(false);
        if(PhotonNetwork.player.ID == PlayerManager.Instance.getCurrentPlayerTurn())
        {
            GLOBAL.setLock(false);
        }
    }
}
