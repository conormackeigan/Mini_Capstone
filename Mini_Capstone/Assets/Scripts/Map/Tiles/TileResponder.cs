using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TileResponder : MonoBehaviour, IPointerClickHandler
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseClick()
    {
        UIManager.Instance.DeactivateEnemyPanel();

        Player currentPlayer = PlayerManager.Instance.getCurrentPlayer();

        // if there is a selected unit in moving state, gtfo dont process stuff
        if (currentPlayer.selectedObject != null)
        {
            if (currentPlayer.selectedObject.tag == "Unit")
            {
                if (currentPlayer.selectedObject.GetComponent<Unit>().state == Unit.UnitState.Moving)
                {
                    return;
                }
            }
        }

        // If an object is currently selected, check if unit and then move to current tile
        if (currentPlayer.selectedObject != null)
        {
            if (currentPlayer.selectedObject.tag == "Unit")
            {
                //if the tile is marked as traversable travel to it
                Vector2i tilePos = GLOBAL.worldToGrid(transform.position);
                if (TileMarker.Instance.travTiles.ContainsKey(tilePos))
                {
                    currentPlayer.selectedObject.GetComponent<Unit>().TravelToPos(tilePos);
                    TileMarker.Instance.Clear();
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick();
    }
}
