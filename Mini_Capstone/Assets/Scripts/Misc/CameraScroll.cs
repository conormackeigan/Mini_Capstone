using UnityEngine;
using System.Collections;


// TODO: Untested; boundries to be added

public class CameraScroll : MonoBehaviour {

    public float speed = 0.1F;
    void Update()
    {
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
    }
}
