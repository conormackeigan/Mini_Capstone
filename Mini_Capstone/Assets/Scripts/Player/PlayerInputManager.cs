// NOTE: DO NOT TURN THIS ON, USING NEW METHOD ENTIRELY TO WORK WITH COLLISION


using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour
{
/*
    public int playerID;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Allow input only if it is current players turn
        //if (PlayerManager.Instance.currentPlayersTurn == playerID)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        LeftMouseClickHandler();
        //    }
       // }
    }

    void LeftMouseClickHandler()
    {
        GameObject hitObject = FindHitObject();

        if (hitObject.tag == "Unit")
        {
            hitObject.GetComponent<Unit>().OnMouseClick();
        }
    }

    GameObject FindHitObject()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        if (hit.collider)
        {
            Debug.Log(hit.collider.gameObject.tag);
            return hit.collider.gameObject;
        }
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LeftMouseClickHandler();
    }
    */
}
