using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision
{
    // State vars
    Transform transform; // the transform of the object

    // the constructor, initializes state variables
    public Vision(GameObject obj)
    {
        transform = obj.GetComponent<Transform>();
    }

    //------------------------------------------------------------------------------//
    // Field of View Functions
    //------------------------------------------------------------------------------//

    //check if object is in view, true if it is, false if not
    public bool inFOV(GameObject obj)
    {
        //find the angle between npc and object
        Transform t = obj.GetComponent<Transform>();

        // return false in this corner case because if it doesn't have a transform, 
        // we can't do anything else with it anyway
        if (t == null) { return false; }

        return inFOV(t.position);
    }

    // see if a certain position is within the field of view
    public bool inFOV(Vector3 pos)
    {
        float angle = Vector3.Angle(pos - transform.position, transform.forward);

        //if object is within view of npc
        if ((angle >= 0) && (angle <= 90))
        {
            return true;
        }

        return false;
    }
}
