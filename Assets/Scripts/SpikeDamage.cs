using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public HeroKnight player;
    public LayerMask playerLayer;
    public Collider2D collider;
    float damageTimer = 0f;
    bool firstCollision = true;

    private void Awake()
    {
        player = FindObjectOfType<HeroKnight>();
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
            FirstCollision();

            if (damageTimer >= 1f)
            {
                player.healthComponent.PlayerTakeDamage(10f);
                Debug.Log(player.healthComponent.curHealth);
                damageTimer = 0f;
            }

            damageTimer += 0.016f;
        }
        else
        {
            firstCollision = true;
        }
    }

    void FirstCollision()
    {
        if (firstCollision)
        {
            player.healthComponent.PlayerTakeDamage(10f);
            Debug.Log(player.healthComponent.curHealth);
            firstCollision = false;
        }
    }
}
