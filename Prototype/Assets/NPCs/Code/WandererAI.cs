using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// The Wanderer runs about aimlessly, deciding on a new destination within
// sight whenever he reaches his current destination.
public class WandererAI : CoreAI
{
    public bool inDanger;   // a variable for the Protector to look at to determine state

    float COOL_TIME = 5f;  // a constant for how long it takes to cool down in seconds
    float dangerCooldown;   // resets inDanger upon reaching COOL_TIME
	float walkDist;         // distance Wanderer should move before switching direction

	// Use this for initialization.  If you want to use Start() method, it
	// must include the line "CoreAIStart()"
	protected void Start()
	{
        // initialize parent class
        CoreAIStart();

        // set vars
        inDanger = false;
        dangerCooldown = COOL_TIME;
        walkDist = 20f;

        // begin walking randomly
        randomDest(walkDist);
	}

	// Update is called once per frame
	void Update()
	{
		// if reached target or out of range, randomly change target position
		if ((my_transform.position - dest).magnitude < 2 || 
            (my_transform.position - dest).magnitude > walkDist + 1)
            randomDest(walkDist);

        // wander randomly - check if the movement is valid in case the Wanderer has
        // tried to walk off the map.  If that has happened, set a new destination
        bool couldMove = moveTo();
        if (!couldMove) { randomDest(walkDist); }

        // if inDanger is true, add to the cool down timer - reset inDanger if cool
        if (inDanger) dangerCooldown -= Time.deltaTime;
        if (dangerCooldown <= 0.0)
        {
            inDanger = false;
            dangerCooldown = COOL_TIME;
        }
	}

    // if player touches this, increment score, indicate danger, 
    // and then jump to a random place in room
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // add to score
            PlayerScore.score++;

            // indicate danger
            inDanger = true;
            dangerCooldown = COOL_TIME;

            // jump randomly
            RandomMapMaker script = GameObject.Find("ProceduralGenerator").GetComponent<RandomMapMaker>();
            bool successful = false;
            while (!successful)// keep jumping until it's valid
            {
                float randomX = Random.Range(0f, (script.mapWidth - 1) * script.tileSize);
                float randomZ = Random.Range(0f, (script.mapWidth - 1) * script.tileSize);
                successful = my_nav.Warp(new Vector3(randomX, 0f, randomZ)); 
            }
        }
    }
}
