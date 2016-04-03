using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class TerrainLayer : Singleton<TerrainLayer>
{
    //THE GRID:
    private Tile[,] tiles;
    public Tile[,] Tiles { get { return tiles; } }

    private int tileSize = (int)IntConstants.TileSize;
    private MapScript map;

    public TextAsset textFile;

    void Start()
    {

    }

    void Update()
    {
        //on mouse click print the world coordinates
        //if (Input.GetMouseButtonDown(0))
        //{
        //    int posx = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / tileSize);
        //    int posy = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / tileSize);

        //    Debug.Log("x: " + posx + " y: " + posy);
        //}
    }

    public Vector2i convertMousePosToGrid(Vector3 mousePos)
    {
        return new Vector2i((int)(Camera.main.ScreenToWorldPoint(mousePos).x / tileSize),
                            (int)(Camera.main.ScreenToWorldPoint(mousePos).y / tileSize));
    }

    public void createMap()
    {
        map = transform.parent.GetComponent<MapScript>(); //get reference to map parent
        tiles = new Tile[map.Width, map.Height]; //THE GRID
        Object meadow = Resources.Load("Tiles/MeadowTile"); //for testing
        Object untraversable = Resources.Load("Tiles/Untraversable");
        Object forest = Resources.Load("Tiles/ForestTile");

        // Read map editor file
        /*if (!File.Exists("Assets/MapFile"))
        {
            Debug.Log("MapFile does not exist.");
            return;
        }*/
        string s = textFile.text;
        Debug.Log(s);
        string[] splitChars = { "\n", "\r" };
        string[] linesFromfile = (s.Split(splitChars, System.StringSplitOptions.RemoveEmptyEntries));
        string input;

        //instantiate sample map (meadow(weight 1) + untraversable tiles + forest(weight 2))
        int k = 0;
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {

                input = linesFromfile[k];
                k+=1;
                if (input == "Forest")
                {
                    Instantiate(forest, new Vector3(i * tileSize, j * tileSize, 0), Quaternion.identity);
                    tiles[i, j] = new ForestTile(new Vector2i(i, j));
                }
                if (input == "Mountain")
                {
                    Instantiate(untraversable, new Vector3(i * tileSize, j * tileSize, 0), Quaternion.identity);
                    tiles[i, j] = new UntraversableTile(new Vector2i(i, j));
                }
                if (input == "Grass")
                {
                    Instantiate(meadow, new Vector3(i * tileSize, j * tileSize, 0), Quaternion.identity);
                    tiles[i, j] = new MeadowTile(new Vector2i(i, j));
                }
            }
        }

        //sr.Close();

    }

    public void endGame()
    {
        TileMarker.Instance.Clear();

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i, j] != null)
                {
                    //Destroy(tiles[i, j].);
                }
                tiles[i, j] = null;
            }
        }

        GameObject[] tilesToDelete = GameObject.FindGameObjectsWithTag("Tile");

        for (int i = 0; i < tilesToDelete.Length; i++)
        {
            Destroy(tilesToDelete[i]);
        }
    }
}
