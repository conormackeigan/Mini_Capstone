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
        if (GameDirector.Instance.locked)
        {
            return;
        }

        UIManager.Instance.DeactivateEnemyPanel();

        Player currentPlayer = PlayerManager.Instance.getCurrentPlayer();

        // If an object is currently selected, check if unit and then move to current tile
        if (currentPlayer.selectedObject != null)
        {
            if (currentPlayer.selectedObject.tag == "Unit")
            {
                // if the tile is marked as traversable travel to it
                Vector2i tilePos = GLOBAL.worldToGrid(transform.position);

                if (TileMarker.Instance.travTiles.ContainsKey(tilePos))
                {
                    currentPlayer.selectedObject.GetComponent<Unit>().TravelToPos(tilePos);
                    TileMarker.Instance.Clear();
                }

                // if the tile is marked with an AoE tile, begin AoE selection
                else if (TileMarker.Instance.AoETiles.ContainsKey(tilePos))
                {
                    //TileMarker.Instance.Clear(); // clear purple tiles
                    //GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoEWeapon.markAoEPattern(GLOBAL.worldToGrid(transform.position));
                    //GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoERoot = GLOBAL.worldToGrid(transform.position);
                    //UIManager.Instance.activateAttackButton();  // activate confirm button for AoE attacks
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick();
    }
}
