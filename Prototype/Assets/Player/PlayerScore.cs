using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScore : MonoBehaviour {

    public static bool isUsed;// sets whether scoring is in use or not for the scene
    public static int score;  // an easily accessible variable for various objects to alter score
    static int prevScore;     // score seen at the last update

    public AudioClip loss;    // losing points sound effect
    public AudioClip gain;    // gaining points sound effect
    public GameObject audioObject;   // reference to the prefab holding the audio source
    AudioSource source;       // AudioSource component attached to Player

    GUIStyle style;           // how the score looks when drawn on screen

	// Use this for initialization
	void Start ()
    {
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(2)))
            isUsed = false;
        else
            isUsed = true;

        // get audio source
        GameObject audioCopy = Instantiate(audioObject, GetComponent<Transform>());
        source = audioCopy.GetComponent<AudioSource>();

        // set score vars
        score = 0;
        prevScore = score;

        // style vars
        style = new GUIStyle();
        style.richText = true;
    }

    // decrement the cool down timer for invincibility if necessary
    void Update()
    {
        if (isUsed)
        {
            // play sounds if score has changed
            if (prevScore < score) source.PlayOneShot(gain);
            if (prevScore > score) source.PlayOneShot(loss);
            prevScore = score;
        }
    }

    // Draw the score to the screen
    void OnGUI()
    {
        if (isUsed)
            GUILayout.Label("<size=30><color=white>Score: " + score + "</color></size>", style);
    }
}
