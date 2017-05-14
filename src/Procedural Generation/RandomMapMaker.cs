using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This script will be attached to a Procedural Generation object in scenes that want a 
// random distribution of items - a forest with randomly placed trees for example. Also
// supports placement of items at a constant y offset - like a level with platforms overhead
public class RandomMapMaker : MonoBehaviour {

    // public vars
    public GameObject player;        // the player object to instantiate
    public GameObject reticle;       // onscreen representation of the mouse
    public GameObject[] npcs;        // list of the NPC characters we could instantiate
    public GameObject[] floor;       // list of floor prefabs
    public GameObject[] groundItems; // list of grounded item prefabs
    public GameObject[] inAirItems;  // list of items with the given y offset
    public float groundItemProb;     // probability of placing a grounded item
    public float inAirItemProb;      // probability of placing an item in air
	public float minScaling;         // minScaling possible tileSize multiplier
	public float maxScaling;         // highest possible tileSize multiplier
    public int mapWidth;             // width of map in number of floor tiles
    public int mapHeight;            // height of map in number of floor tiles
    public float tileSize;           // tileSize of one side of square floor patch
    public float yOffset;            // distance inAirItems are off the ground
    public bool shouldStack;         // true if in air items can go on top of ground items

    // private vars
    GameObject[,] mapFloor;          // type of floor to place in room
    GameObject[,] mapItems;          // whether and where to place items on the ground
    GameObject[,] mapInAirItems;     // whether and where to place in air items
    Vector3 mapCenter;               // position of center of map

    // Use this for initialization
    void Start()
    {
        // initialize variables
        if (tileSize == 0f) tileSize = 3.2f;  // assume this size if none given
        mapCenter = new Vector3(mapWidth * tileSize / 2, 1f, mapHeight * tileSize / 2);

        // call functions to lay out a 2D array representation of the map, 
        // instantiate the environment
        layoutMap();
        fillMap();

        // with the scene complete, put in NPCs
        placeNpcs();

        // place the player at the center of the room and add the crosshairs to the screen
        Instantiate(player, mapCenter, Quaternion.identity);
        Instantiate(reticle);
    }

    // Figure out where floor and items should go in world
    void layoutMap()
    {
        // initialize the tileSize of the map
        mapFloor = new GameObject[mapWidth, mapHeight];
        mapItems = new GameObject[mapWidth, mapHeight];
        mapInAirItems = new GameObject[mapWidth, mapHeight];

        // fill in the arrays with patches of grass and possibly trees
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                // tiles for the floor
                mapFloor[i, j] = floor[Random.Range(0, floor.GetLength(0))];

                // add grounded items according to density
                if (Random.value < groundItemProb)
                    mapItems[i, j] = groundItems[Random.Range(0, groundItems.GetLength(0))];
                else
                    mapItems[i, j] = null;

                // add in air items according to density, stacking over grounded items if allowed
                if (Random.value < inAirItemProb)
                {
                    mapInAirItems[i, j] = inAirItems[Random.Range(0, inAirItems.GetLength(0))];
                    if (!shouldStack && mapItems[i, j] != null)
                        mapInAirItems[i, j] = null;
                }
                else
                    mapInAirItems[i, j] = null;
            }
    }

    // Lay down the floors and items in the world
    void fillMap()
    {
        // go through arrays and place floor/items as they're written
        // adjust position according to i, j multiplied by the tileSize
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                // get prefabs to be placed
                GameObject floorObj = mapFloor[i, j];
                GameObject groundItemObj = mapItems[i, j];
                GameObject inAirItemObj = mapInAirItems[i, j];

                // create floor instances and space according to tile size
                Instantiate(floorObj, new Vector3(i * tileSize, 0.0f, j * tileSize), 
                    Quaternion.identity);

                // place ground items if they're not null - scale and position properly
                if (groundItemObj != null)
                {
                    float scale = Random.Range(minScaling, maxScaling);
                    Vector3 scaling = new Vector3(scale, scale, scale);
					GameObject t = Instantiate(groundItemObj, 
                        new Vector3(i * tileSize, groundItemObj.transform.position.y, j * tileSize),
                        Quaternion.identity);
                    t.transform.localScale += scaling;
                }

                // place in air items if not null
                if (inAirItemObj != null)
                    Instantiate(inAirItemObj, new Vector3(i * tileSize, 
                        yOffset + inAirItemObj.transform.position.y, j * tileSize), 
                        Quaternion.identity);
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
            Instantiate(npc, new Vector3(Random.Range(0f, xbound), 0f, 
                Random.Range(0f, ybound)), Quaternion.identity);
        }
    }

    // create the NavMesh at run time after map is generated
    void bakeNavMesh()
    {
        // lots of BS about the scene that will need editing at some point I'm sure
        Bounds b = new Bounds(mapCenter, mapCenter + new Vector3(0f, 10f, 0f));
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
        NavMeshBuilder.CollectSources(b, 0, NavMeshCollectGeometry.RenderMeshes, 0, markups, sources);
        NavMeshBuildSettings settings = NavMesh.CreateSettings();

        // here's the actual important line
        NavMeshBuilder.BuildNavMeshData(settings, sources, b, mapCenter, Quaternion.identity);
    }
}
