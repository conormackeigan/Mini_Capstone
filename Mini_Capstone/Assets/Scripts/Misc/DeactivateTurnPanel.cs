using UnityEngine;
using System.Collections;

public class DeactivateTurnPanel : MonoBehaviour {

    public void deactivateTurnPanel()
    {
        this.gameObject.SetActive(false);
        GLOBAL.setLock(false);
    }
}
