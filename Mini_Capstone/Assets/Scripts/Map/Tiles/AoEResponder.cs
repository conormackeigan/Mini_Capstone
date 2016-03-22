using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AoEResponder : MonoBehaviour, IPointerClickHandler
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
        TileMarker.Instance.Clear(); // clear purple tiles
        GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoEWeapon.markAoEPattern(GLOBAL.worldToGrid(transform.position));
        GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoERoot = GLOBAL.worldToGrid(transform.position);
        UIManager.Instance.activateAttackButton();  // activate confirm button for AoE attacks
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick();
    }
}
