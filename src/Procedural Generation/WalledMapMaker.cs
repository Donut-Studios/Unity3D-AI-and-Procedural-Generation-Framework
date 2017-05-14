using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will be attached to a Procedural Generation object in scenes that want building-type
// walls laid out in them.  Currently, walls of two different heights can be laid out, and they will
// be properly aligned with respect to the X and Z axes
public class WalledMapMaker : MonoBehaviour {

    // public vars
    public GameObject player;        // the player object to instantiate
    public GameObject reticle;       // onscreen representation of the mouse
    public GameObject[] npcs;        // list of the NPC characters we could instantiate
    public GameObject[] floor;       // list of floor prefabs
    public GameObject wallXHigh;     // a tall wall primitive or prefab oriented along x axis
    public GameObject wallXLow;      // a short wall primitive or prefab oriented along x axis
    public GameObject wallZHigh;     // a tall wall primitive or prefab oriented along z axis
    public GameObject wallZLow;      // a short wall primitive or prefab oriented along z axis
    public int mapWidth;             // width of the map in number of tiles
    public int mapHeight;            // height of the map in number of tiles
    public float tileSize;           // size of the floor tiles or wall lengths
    public int maxBuildingSize;      // largest the randomly generated buildings should be (min 3)

    // private vars
    GameObject[,] mapFloor;          // type of floor to place in room
    GameObject[,] mapWalls;          // where walls should be placed in the room
    Vector3 mapCenter;               // position of center of map

    // Use this for initialization
    void Start()
    {
        // initialize variables
        if (tileSize == 0f) tileSize = 3.2f;  // assume this size if none given
        mapCenter = new Vector3(mapWidth * tileSize / 2, 0.25f, mapHeight * tileSize / 2);

        // call functions to lay out a 2D array representation of the map, 
        // instantiate the environment
        layoutMap();
        fillMap();
        fillEdges();

        // with the scene complete, put in NPCs
        placeNpcs();

        // place the player at the center of the room and add the crosshairs to the screen
        Vector3 playerStart = new Vector3(tileSize, player.transform.position.y, tileSize);
        Instantiate(player, playerStart, Quaternion.Euler(new Vector3(0f, 45f)));
        Instantiate(reticle);
    }

    // Figure out where floors and walls should go in the map
    void layoutMap()
    {
        // initialize the tileSize of the map
        mapFloor = new GameObject[mapWidth, mapHeight];
        mapWalls = new GameObject[mapWidth, mapHeight];

        // fill in the arrays with flooring and also possibly walls
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                // tiles for the floor
                mapFloor[i, j] = floor[Random.Range(0, floor.GetLength(0))];

                // walls - start null then place a building of random size if enough space
                if (i % (maxBuildingSize + 1) == 0 && j % (maxBuildingSize + 1) == 0 && 
                    i <= mapWidth - maxBuildingSize && j <= mapHeight - maxBuildingSize)
                    layoutBuilding(i, j);

                // remove a few random walls to look decrepit (and also adds pathways)
                if (mapWalls[i, j] != null)
                    if (Random.value < 0.05) // 5% chance of removing wall
                        mapWalls[i, j] = null;
            }
    }

    // Lay down the grass and trees in the world
    void fillMap()
    {
        // go through arrays and place floors and walls as they're written
        // adjust position according to i, j multiplied by the tileSize
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                // get prefabs to be placed
                GameObject floorObj = mapFloor[i, j];
                GameObject wallObj = mapWalls[i, j];

                // create floor instances and space according to tile size
                Instantiate(floorObj, new Vector3((i + 0.5f) * tileSize, 0.0f, (j + 0.5f) * tileSize),
                    Quaternion.identity);

                // place walls
                if (wallObj != null)
                    Instantiate(wallObj, new Vector3((i + 0.5f) * tileSize, wallObj.transform.position.y, 
                        (j + 0.5f) * tileSize), Quaternion.identity);
            }
    }

    // lay out walls along the edges of the room to prevent escape
    void fillEdges()
    {
        // fill in edges along the width
        for (int i = 0; i < mapWidth; i++)
        {
            Instantiate(wallXHigh, new Vector3((i + 0.5f) * tileSize, 
                wallXHigh.transform.position.y, 0.5f * tileSize), Quaternion.identity);
            Instantiate(wallXHigh, new Vector3((i + 0.5f) * tileSize,
                wallXHigh.transform.position.y, (mapHeight - 0.5f) * tileSize), Quaternion.identity);
        }

        // fill in edges along the height
        for (int j = 0; j < mapHeight; j++)
        {
            Instantiate(wallZHigh, new Vector3(0.5f * tileSize,
                wallZHigh.transform.position.y, (j + 0.5f) * tileSize), Quaternion.identity);
            Instantiate(wallZHigh, new Vector3((mapWidth - 0.5f) * tileSize,
                wallZHigh.transform.position.y, (j + 0.5f) * tileSize), Quaternion.identity);
        }
    }

    // put down NPCs at some random locations in the room
    void placeNpcs()
    {
        // go through each of the NPCs attached to the script and randomly instantiate them
        foreach (GameObject npc in npcs)
        {
            float xbound = (mapWidth - 1) * tileSize;
            float ybound = (mapHeight - 1) * tileSize;
            Instantiate(npc, new Vector3(Random.Range(tileSize, xbound), 0f,
                Random.Range(tileSize, ybound)), Random.rotation);
        }
    }

    // place squares of walls of random sizes that represent buildings
    void layoutBuilding(int x, int y)
    {
        int size = Random.Range(3, maxBuildingSize);

        // fill in edges of building along the width
        for (int i = x; i < x + size; i++)
        {   
            mapWalls[i, y] = pick(wallXHigh, wallXLow, 0.5f);
            mapWalls[i, y + size] = pick(wallXHigh, wallXLow, 0.5f);
        }

        // fill in edges of building along the height
        for (int j = y; j < y + size; j++)
        {
            mapWalls[x, j] = pick(wallZHigh, wallZLow, 0.5f);
            mapWalls[x + size, j] = pick(wallZHigh, wallZLow, 0.5f);
        }
    }

    // pick between two things with some probability (P(option1) = prob, P(option2) = 1 - prob)
    GameObject pick(GameObject option1, GameObject option2, float prob)
    {
        if (Random.value < prob)
            return option1;
        else
            return option2;
    }
}
