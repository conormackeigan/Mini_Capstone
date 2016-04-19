using UnityEngine;
using System.Collections;
using System.IO;

public class MapEditor : MonoBehaviour {

    public int width;
    public int height;
    private string[,] tiles;
    private GameObject[] tileObjects;

    // Use this for initialization
    void Start () {

        tileObjects = GameObject.FindGameObjectsWithTag("Tile");
        tiles = new string[width,height];

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j] = findAtPos(i, j);
            }
        }

        if (File.Exists("MapFile.txt"))
        {
            Debug.Log("MapFile.txt already exists.");
            return;
        }

        StreamWriter sr = File.CreateText("MapFile.txt");

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(tiles[i,j] == "Forest")
                {
                    sr.WriteLine("Forest");
                }
                if (tiles[i, j] == "Grass")
                {
                    sr.WriteLine("Grass");
                }
                if (tiles[i, j] == "Mountain")
                {
                    sr.WriteLine("Mountain");
                }
            }
        }
        //sr.Close();
   
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    string findAtPos(int i, int j)
    {
        for(int m = 0; m < tileObjects.Length; m++)
        {
            if(tileObjects[m].transform.position.x == i * 64
                && tileObjects[m].transform.position.y == j * 64)
            {
                if(tileObjects[m].GetComponent<isForest>())
                {
                    return "Forest";
                }
                if (tileObjects[m].GetComponent<isGrass>())
                {
                    return "Grass";
                }
                if (tileObjects[m].GetComponent<isMountain>())
                {
                    return "Mountain";
                }
            }
        }

        return null;
    }
}
