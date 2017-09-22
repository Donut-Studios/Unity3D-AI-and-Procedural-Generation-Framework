using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//predator tries to locate the player and attack the
public class PredatorAI : CoreAI
{
    private Vector3 last_position;
    private Vector3 home;
    private Ray line_of_sight;
    private Ray player_line_of_sight;
    private float personal_space;

    protected void Start()
    {
        CoreAIStart();

        personal_space = 2.5f;
        my_nav.stoppingDistance = personal_space;
        home = my_transform.position;
        last_position = my_transform.position;
        player_line_of_sight = new Ray(player.transform.position, player.transform.forward);
        line_of_sight = new Ray(my_transform.position, my_transform.forward);
    }

    //using fixed update prevents lagging
    private void FixedUpdate()
    {
        // update variables
        line_of_sight = new Ray(my_transform.position + new Vector3(0, 0.5f, 0), my_transform.forward);
        player_line_of_sight = new Ray(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.forward);

        // sample a 180 degree field of view and see if player is there
        for (int i = 0; i < 80; i += 2)
        {
            checkLineOfSight(i);
            checkLineOfSight(-i);
        }
        moveTo(last_position);
    }

    // Updates last_position to player position if player is within sampled field of view
    private void checkLineOfSight(int value)
    {
        // calculate rotation and raycast direction
        Quaternion rotate = Quaternion.AngleAxis((float)value, Vector3.up);
        Ray r1 = new Ray(line_of_sight.origin, rotate * line_of_sight.direction);
        RaycastHit hit1;

        // see if the player is hit by the raycast
        float range = 50.0f;
        if (Physics.Raycast(r1, out hit1, range))
            if (player.transform.tag == hit1.transform.tag)
                last_position = player.transform.position;
    }

    // Draw debugging
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(last_position, Vector3.one);
        Gizmos.DrawWireSphere(this.transform.position, personal_space);
        Gizmos.DrawRay(player_line_of_sight);
        Gizmos.DrawRay(line_of_sight);
    }
}