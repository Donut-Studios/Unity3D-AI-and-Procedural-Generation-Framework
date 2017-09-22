using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {

    // loads other scenes in the background
    void Start()
    {
        // make sure level selector isn't lost in loading
        DontDestroyOnLoad(this);
    }

	// Update is called once per frame
	void Update () {

        // switch the current scene
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene(2);

        // quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
