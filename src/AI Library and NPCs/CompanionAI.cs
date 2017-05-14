using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionAI : CoreAI
{ 
    // the states that the Companion can be in
    enum Statetype { LISTENING, MOVING, FOLLOWING };

    // instance variables
	Renderer rend;
	Shader normal;
	Shader glowing;
    Statetype state;
    Vector3 mouse_position;

    // ****************** FSM ****************

    // how to behave if Companion is waiting for a command
    void handleListening()
    {
        // make him stop closer to target
        my_nav.stoppingDistance = 1f;

		// glow to indicate listening state
		rend.material.shader = glowing;

        // check mouse buttons to see if there is an action to do,
        // or to see if state should change
        if (checkMouseButtons())
            return;

        // default behavior is to stop and look at where you point
        stopHere();
        lookAtMouse();
    }

    // moves Buddy to the mouse position, then returns to listening
    void handleMoving()
    {
        if (checkMouseButtons())
            return;

        // if position reached, go back to listening
		if (Vector3.Distance (my_transform.position, mouse_position) < 0.1) {
			state = Statetype.LISTENING;
			return;
		}
        // default is to keep moving
        else 
			moveTo (mouse_position);
    }

    // how to behave if Buddy is not waiting for a command
    void handleFollowing()
    {
        // make him stop a little further away from you
        my_nav.stoppingDistance = 9f;

        // click on Buddy to make him wait and listen for commands
        if (Input.GetMouseButtonDown(0))
        {
			if (getMouseObject().tag == "Companion")
			{
            	state = Statetype.LISTENING;
			}
        }        
        // default is follow player around
        else {
			lookAt(playerFront);
            moveTo(playerFront);
        }
    }

    // helper method to do stuff found in two states
    bool checkMouseButtons()
    {
        if (Input.GetMouseButtonDown(0))
        {
			// stop acting / listening if right button pressed and start following
			if (getMouseObject ().tag == "Companion")
			{
				// stop glowing
				rend.material.shader = normal;
				state = Statetype.FOLLOWING;
				return true;
			}
			// change direction if left click is pressed
            state = Statetype.MOVING;
            mouse_position = findMouse();
            return true;
        }
        return false;
    }

    // **************** Start and Update ********************

    // Use this for initialization.  If you want to use Start() method, it
    // must include the function call CoreAIStart()
    protected void Start()
	{
		CoreAIStart();
		rend = (GameObject.Find("chr_janitor1")).GetComponent<Renderer>();
		normal = Shader.Find("Diffuse");
		glowing = Shader.Find("Self-Illumin/Diffuse");
		this.setActive(false);
        state = Statetype.FOLLOWING;
	}

	// Update is called once per frame
	void Update()
	{
        // toggle active state with middle button
        if (Input.GetMouseButtonDown(1))
        {
            toggleActive();
        }

        // doesn't do anything if the object isn't active
		if (isActive) {

			// handle the current state
			switch (state) {
			case Statetype.LISTENING:
				handleListening ();
				break;

			case Statetype.MOVING:
				handleMoving ();
				break;

			case Statetype.FOLLOWING:
				handleFollowing ();
				break;

			default:
				handleFollowing ();
				break;
			}
		}
	}
}
