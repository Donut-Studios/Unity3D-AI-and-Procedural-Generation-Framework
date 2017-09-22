using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// The Stalker quietly follows the Player Character around...
// but only when the Player isn't looking!
public class StalkerAI : CoreAI
{
	// the states that the Stalker can be in
	enum Statetype { STALKING, STILL };

	// instance variables
	Statetype state; 
	float followDist = 1;

	// *** FSM states ********************************************

	// behavior for STALKING state
	void handleStalking()
	{
		// move towards the player
		moveTo(player);

		// if too close or in view of player, switch state
		if (!(safeToGo()))
			state = Statetype.STILL;
	}

	// behavior for SEEN state
	void handleStill()
	{
		// do not move
		my_nav.ResetPath();

		// if the player is sufficiently far enough from the Stalker and not looking, switch state
		if (safeToGo())
			state = Statetype.STALKING;
	}

	// helper method for determining whether Stalker should follow the Player
	bool safeToGo()
	{
		// is the player looking at me?
		bool inView = Vector3.Dot(Vector3.forward, player_transform.InverseTransformPoint(my_transform.position)) > 0;
		// is the player too close to me?
		bool isClose = Vector3.Distance (player_transform.position, my_transform.position) < followDist;
		// are both conditions false? if so, it's safe to go.
		return !(inView || isClose);
	}

	// *** initialization **************************************
	protected void Start()
	{
        CoreAIStart(); // call parent's initializer
        state = Statetype.STILL; // set default state
	}

	// Update is called once per frame
	void Update()
	{
		switch(state)
		{
			case Statetype.STALKING:
				handleStalking();
				break;

			case Statetype.STILL:
				handleStill ();
				break;

			default:
				handleStill ();
				break;
		}
	}
}