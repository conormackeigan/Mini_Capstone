using UnityEngine;
using System.Collections;


// TODO: Untested; boundries to be added

public class CameraScroll : MonoBehaviour {

    public float speed = 0.1F;
    void Update()
    {
        if(GameDirector.Instance.gameState != GameDirector.GameState.BOARD)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0.0f * speed, 10.0f * speed, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0.0f * speed, -10.0f * speed, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-10.0f * speed, 0.0f * speed, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(10.0f * speed, 0.0f * speed, 0);
        }

        float minX = 715;
        float maxX = 825;
        float minY = 400;
        float maxY = 430;

        var v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = v3;

    }
}
