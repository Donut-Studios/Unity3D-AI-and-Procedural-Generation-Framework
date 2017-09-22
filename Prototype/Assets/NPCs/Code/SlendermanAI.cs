using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Slenderman quietly follows the Player Character around...
// but you can never actually see him!
public class SlendermanAI : CoreAI
{ 
	// ***** initialization *****
	protected void Start()
	{
     CoreAIStart(); // call parent's initializer
	}

	// Update is called once per frame
	void Update()
	{
		// is the player looking at me?
		bool inView = Vector3.Dot(Vector3.forward, player_transform.InverseTransformPoint(my_transform.position)) > 0;

		// is the player too close to me?
		bool isClose = Vector3.Distance(player_transform.position, my_transform.position) < 7;

		// if player isn't looking at me and I'm not too close, move towards player
		if(!(inView || isClose))
		{
			// move towards the player
			moveTo(player);
		}
		// if player is looking at me, teleport behind player
		if(inView)
		{
			transform.LookAt(player_transform);
			Transform copy_transform = my_transform;
			copy_transform.RotateAround(player_transform.position, player_transform.right, 100.0f);
			moveTo(copy_transform.position);
			// // obtain location behind player
			// tr = player_transform.forward;
			// behind_player.z -= 10;
			// my_transform.position = behind_player;
		}
	}
}