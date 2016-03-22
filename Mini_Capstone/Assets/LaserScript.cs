using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform laserHit;

	// Use this for initialization
	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        laserHit.position = hit.point;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, laserHit.position);
        if (Input.GetKey(KeyCode.A))
        {
            lineRenderer.enabled = true;
        }
	}
}
