using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
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
            if(this.tag == "Coin")
            {
                sceneManager.CoinUpdate();
            }

            if (this.tag == "Potion")
            {
                sceneManager.PotionPickup();
            }

            this.collider.enabled = false;
            this.gameObject.SetActive(false);
        }
    }
}