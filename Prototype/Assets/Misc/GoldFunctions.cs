using System.Collections;
using UnityEngine;

public class GoldFunctions : MonoBehaviour {

	// Rotate coin
	void Update () {
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}

	// If player or companion touch this, disappear and increment score
	void OnTriggerEnter(Collider other) 
	{
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Companion"))
        {
            PlayerScore.score++;
            gameObject.SetActive(false);
        }
	}
}
