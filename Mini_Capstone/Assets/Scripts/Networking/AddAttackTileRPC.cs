using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class AddAttackTileRPC : Photon.MonoBehaviour
{

    [PunRPC]
    public IEnumerator AddAttackTile(int j, int i)
    {
        Debug.Log("Adding Attack Tile for player: " + PhotonNetwork.playerName);

        GameObject marker = Instantiate(TileMarker.Instance.attackMarker, GLOBAL.gridToWorld(j, i), Quaternion.identity) as GameObject;
        TileMarker.Instance.attackTiles.Add(new Vector2i(j, i), marker);

        yield return 0;
    }
}
