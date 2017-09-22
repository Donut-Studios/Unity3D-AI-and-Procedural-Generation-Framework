using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectorAI : CoreAI {

    // the states that the Protector can be in
    enum Statetype { SAFE, CAUTIOUS, DANGER };

    // publics (set from within editor)
    public AudioClip dangerClip;    // background music when Wanderer is in danger
    public AudioClip[] grunts;      // occasional noises the Protector will make when angered

    // private instance variables
    Statetype state;            // current state of the Protector
    GameObject wanderer;        // the Wanderer associated with this Protector (GameObject)
    WandererAI my_wanderer;     // the script controlling the Wanderer - has variables we care about
    AudioSource source;         // audio source component associated with Protector
    float gruntCooldown;        // adds time in between producing sound effects
    float CAUTION_RADIUS = 8f;  // how close the player can get before Protector is cautious

    // ****************** FSM ****************

    // how to behave if player is far away and not attacking
    void handleSafe()
    {
        // look at and follow the Wanderer
        lookAt(wanderer);
        moveTo(wanderer);

        // transition states if player gets too close
        if (Vector3.Distance(player_transform.position, wanderer.transform.position) < CAUTION_RADIUS)
            state = Statetype.CAUTIOUS;
    }

    // how to behave if player is too close
    void handleCautious()
    {
        // look at and block off the player with body
        lookAt(player);
        moveTo(blockOffPlayer());

        // occasionally make a grunting noise
        makeGrunt(0.005f);

        // transition back to safe state if player is far enough away
        if (Vector3.Distance(player_transform.position, wanderer.transform.position) > CAUTION_RADIUS)
            state = Statetype.SAFE;
    }

    // how to behave if player attacks
    void handleDanger()
    {
        // make sure to run into player and not stop short
        my_nav.stoppingDistance = 0f;

        // check if the Wanderer is out of danger and player is far enough away
        if (!my_wanderer.inDanger && distToPlayer() > CAUTION_RADIUS)
        {
            state = Statetype.CAUTIOUS;
            if (source.isPlaying) source.Stop();
            my_nav.stoppingDistance = 0.5f;
        }

        // otherwise pursue player and play danger music
        else
        {
            if (source.clip != dangerClip) source.clip = dangerClip;
            if (!source.isPlaying) source.Play();
            lookAt(player);
            moveTo(player);
            makeGrunt(0.01f);
        }
    }

    // ***** Helper Methods for FSM *****

    // helper method to determine destination if player is too close
    Vector3 blockOffPlayer()
    {
        Vector3 dir = wanderer.transform.position - player_transform.position;
        Vector3 location = -4 * dir.normalized + wanderer.transform.position;
        return location;
    }

    // helper method that, with some given chance, plays a grunting noise
    void makeGrunt(float chance)
    {
        if (gruntCooldown <= 0f && Random.Range(0f, 1f) < chance)
        {
            source.PlayOneShot(grunts[Random.Range(0, grunts.Length)]);
            gruntCooldown = 3f; // 3 real time seconds before next grunt can be played
        }
    }

    // *************** Built-In Methods ****************

	// Use this for initialization
	protected void Start () {
        CoreAIStart();
        state = Statetype.SAFE;
        wanderer = GameObject.FindWithTag("Wanderer");
        my_wanderer = wanderer.GetComponent<WandererAI>();
        source = GetComponent<AudioSource>();
        gruntCooldown = 0f;
    }
	
	// Update is called once per frame
	void Update () {

        // regardless of state, enter DANGER state if Wanderer is in danger
        if (my_wanderer.inDanger) state = Statetype.DANGER;

        // call on handler for current state
        switch(state)
        {
            case Statetype.SAFE:
                handleSafe();
                break;

            case Statetype.CAUTIOUS:
                handleCautious();
                break;

            case Statetype.DANGER:
                handleDanger();
                break;

            default:
                handleSafe();
                break;
        }

        // count down timer for sound effects
        if (gruntCooldown >= 0f) gruntCooldown -= Time.deltaTime;
	}
}
