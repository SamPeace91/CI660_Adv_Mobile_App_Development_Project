using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Collider2D collider;
    public LayerMask playerLayer;
    public SceneManagement sceneManager;

    private void Awake()
    {
        //sceneManager.GetComponent<SceneManagement>();
        sceneManager = FindObjectOfType<SceneManagement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        collider.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collider.IsTouchingLayers(playerLayer))
        {
            if(tag == "L2Door")
            {
                sceneManager.LevelTwo();
            }
            else if (tag == "L3Door")
            {
                sceneManager.LevelThree();
            }
            else if (tag == "EndDoor")
            {
                sceneManager.WinScreen();
                Debug.Log("You win the game!");
            }
        }
    }
}
