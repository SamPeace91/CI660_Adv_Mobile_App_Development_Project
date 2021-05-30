using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        //Finds the relevant GameObject at runtime and ensures it is not destroyed when a new scene is loaded
        GameObject[] sceneManagers = GameObject.FindGameObjectsWithTag("SceneManager");
        GameObject[] huds = GameObject.FindGameObjectsWithTag("HUD");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] controls = GameObject.FindGameObjectsWithTag("Controls");
        GameObject[] menuCanvas = GameObject.FindGameObjectsWithTag("Menu");

        // If the object already exists (by completing a game loop back to the main menu) then this new object is destroyed
        if (sceneManagers.Length > 1 || huds.Length > 1 || players.Length > 1 || controls.Length > 1 || menuCanvas.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
