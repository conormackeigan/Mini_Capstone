using UnityEngine;
using System.Collections;

public class AttackTileScript : MonoBehaviour
{
    private bool darkenFlag; // true when the tile is darkening (alpha++)
    private bool waitFlag; // true when the tile is pausing (at 
    Color color;

	void Start ()
    {
        darkenFlag = true;
        waitFlag = false;

        //gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.7f);
        color = gameObject.GetComponent<SpriteRenderer>().color;
        color.a = 0.7f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }


    void Update()
    {
        if (waitFlag)
        {
            return;
        }

        if (darkenFlag)
        {
            if (color.a < 0.825f)
            {
                color.a += Time.deltaTime * 0.5f;
                gameObject.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                darkenFlag = false;
            }
        }

        else
        {
            if (color.a > 0.3f)
            {
                color.a -= Time.deltaTime * 0.5f;
                gameObject.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                darkenFlag = true;
                waitFlag = true;
                Invoke("wait", 0.5f);
            }
        }
    }


    void wait()
    {
        waitFlag = false;
    }
}
