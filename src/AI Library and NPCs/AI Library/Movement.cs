using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement {

    // useful constants
    Vector2 SCREEN_CENTER = new Vector2(Screen.width / 2, Screen.height / 2);
    float CAM_RAY_LENGTH = 80f;

    //GameObject obj;      // the object being manipulated
    Transform transform; // the transform of the object
    Rigidbody rb;        // the rigid body of the object
    NavMeshAgent nav;    // the nav mesh of the object

    // the constructor
    public Movement(GameObject obj)
    {
        //this.obj = obj;
        transform = obj.GetComponent<Transform>();
        rb = obj.GetComponent<Rigidbody>();
        nav = obj.GetComponent<NavMeshAgent>();
    }

    // head toward the specified GameObject's position - returns true if valid destination
    public bool moveTo(GameObject obj)
    {
        Transform t = obj.GetComponent<Transform>();
        if (t == null) {
            return false;
        }
        return moveTo(t.position);
    }

    // head toward a specific position - returns true if valid destination
    public bool moveTo(Vector3 d)
    {
        return nav.SetDestination(d);
    }

    // stand still
    public void stopHere()
    {
        nav.ResetPath();
    }

    // turn towards specified GameObject
    public void lookAt(GameObject obj)
    {
        Transform t = obj.GetComponent<Transform>();
        if (t == null) { return; }
        lookAt(t.position);
    }

    // turn towards specified position
	// USING QUATERNIONS MAKES THIS SNAPPY AND NOT SMOOTH, MAYBE TRY SLERP() TO SMOOTH THE TRANSITION???
    public void lookAt(Vector3 pos)
    {
        Vector3 lookDirection = pos - transform.position;
        lookDirection.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(newRotation);
    }

    // turn towards where the mouse is pointing
    public void lookAtMouse()
    {
        lookAt(findMouse());
    }

    // return the mouse location as a position
    public Vector3 findMouse()
    {
        // create a RaycastHit variable to store information about what was hit by the ray
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER);

        // if it hits something, return the location
        if (Physics.Raycast(ray, out hit, CAM_RAY_LENGTH))
            return hit.point;
        
        return new Vector3(0f, 0f, 0f);
    }

	// return the GameObject that the mouse is pointing towards
	public GameObject getMouseObject()
	{
		// create a RaycastHit variable to store information about what was hit by the ray
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(SCREEN_CENTER);

		// if it hits something, return the GameObject, otherwise return null
		if (Physics.Raycast (ray, out hit, CAM_RAY_LENGTH))
			return hit.transform.gameObject;
		else
			return null;
	}
}
