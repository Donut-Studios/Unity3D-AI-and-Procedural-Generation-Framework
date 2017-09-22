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
        my_nav.stoppingDistance = 1f;

        personal_space = 2.5f;
        home = this.transform.position;
        last_position = this.transform.position;
        player_line_of_sight = new Ray(player.transform.position, player.transform.forward);
        line_of_sight = new Ray(this.transform.position, this.transform.forward);


    }

    //using fixed update prevents lagging
    private void FixedUpdate()
    {
        // update variables
        line_of_sight = new Ray(this.transform.position + new Vector3(0, 0.5f, 0), this.transform.forward);
        player_line_of_sight = new Ray(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.forward);


        for (int i = 0; i < 90; i += 4)
        {
            Quaternion rotate = Quaternion.AngleAxis(i, Vector3.up);
            Quaternion neg_rotate = Quaternion.AngleAxis(-i, Vector3.up);

            Ray r1 = new Ray(line_of_sight.origin, rotate * line_of_sight.direction);
            Ray r2 = new Ray(line_of_sight.origin, neg_rotate * line_of_sight.direction);

            RaycastHit hit1;
            RaycastHit hit2;

            float range = 50.0f;

            if (Physics.Raycast(r1, out hit1, range))
            {
                if (player.transform.tag == hit1.transform.tag)
                {
                    Debug.Log(player.transform.tag);
                    last_position = player.transform.position;
                    moveTo(player);
                }
            }

            if (Physics.Raycast(r2, out hit2, range))
            {
                if (player.transform.tag == hit2.transform.tag)
                {
                    last_position = player.transform.position;
                    moveTo(player);
                }
            }
        }
        moveTo(last_position);

    }

    private void patrol()
    {
        Vector3 home_offset = new Vector3(home.x + 10, home.y, home.z);
        if (this.transform.position == home)
            moveTo(home_offset);
        else if (this.transform.position == home_offset)
            moveTo(home);
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(last_position, Vector3.one);
        Gizmos.DrawWireSphere(this.transform.position, personal_space);
        Gizmos.DrawRay(player_line_of_sight);
        Gizmos.DrawRay(line_of_sight);
    }


}