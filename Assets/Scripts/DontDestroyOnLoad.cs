using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        // Finds the 'SceneManager' GameObject at runtime and ensures it is not destroyed when a new scene is loaded
        GameObject[] sceneManagers = GameObject.FindGameObjectsWithTag("SceneManager");
        GameObject[] huds = GameObject.FindGameObjectsWithTag("HUD");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] controls = GameObject.FindGameObjectsWithTag("Controls");
        GameObject[] menuCanvas = GameObject.FindGameObjectsWithTag("Menu");
        //GameObject[] settingsCanvas = GameObject.FindGameObjectsWithTag("Settings");
        //GameObject[] pauseCanvas = GameObject.FindGameObjectsWithTag("Pause");
        //GameObject[] winCanvas = GameObject.FindGameObjectsWithTag("Win");
        //GameObject[] loseCanvas = GameObject.FindGameObjectsWithTag("Lose");

        // If a 'SceneManager' already exists (from picking a new level in the win screen) then this new 'SceneManager' is destroyed
        if (sceneManagers.Length > 1 || huds.Length > 1 || players.Length > 1 || controls.Length > 1 || menuCanvas.Length > 1) //|| settingsCanvas.Length > 1 || pauseCanvas.Length > 1 || winCanvas.Length > 1 || loseCanvas.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
