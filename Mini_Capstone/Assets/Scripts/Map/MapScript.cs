using UnityEngine;
using System.Collections;

//contains universal information about the map
public class MapScript : Singleton<MapScript>
{
    public int mapHeight; //for inspector editing
    public int mapWidth;

    private int height;
    private int width;
    
    public int Height { get { return height; } }
    public int Width { get { return width; } }

	void Start ()
    {
        height = mapHeight;
        width = mapWidth;
    }
	
	void Update ()
    {
	
	}
}
