using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public float curHealth;
    public float maxHealth = 100f;
    //private BoxCollider2D collider;
    public Animator animator;
    public HeroKnight player;
    public Bandit enemy;
    public Image fillAmount;
    public SceneManagement sceneManager;
    private float deathDelay = 0f;
    private bool justDied = false;

    private void Awake()
    {
        enemy = FindObjectOfType<Bandit>();
        player = FindObjectOfType<HeroKnight>();
        sceneManager = FindObjectOfType<SceneManagement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        //collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        //player = GetComponent<HeroKnight>();
    }

    // Update is called once per frame
    void Update()
    {
        if(justDied == true)
        {
            deathDelay += 0.016f;

            if (deathDelay >= 2f)
            {
                Debug.Log("WE'RE IN!");
                justDied = false;
                deathDelay = 0;
                sceneManager.LoseScreen();
                //this.enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if(collision.gameObject.tag == "Enemy")
        {
            print("True");
            curHealth -= 20;
            animator.SetTrigger("Hurt");

            if (curHealth <= 0)
            {
                print("Dead");
                //animator.SetTrigger("Death");
                player.death();
            }
        }*/
        /*else if(collision.gameObject.tag == "Player")
        {
            print("The player hurt me");
            curHealth -= 20;
            //animator.SetTrigger("Hurt");

            if (curHealth <= 0)
            {
                //animator.SetTrigger("Death");
            }
        }*/
    }

    public void EnemyTakeDamage(float damage)
    {
        curHealth -= damage;
        Debug.Log("Enemy health = " + curHealth);
        // Play hurt animation
        animator.SetTrigger("Hurt");

        if (curHealth <= 0f)
        {
            EnemyDeath();
        }
    }

    public void PlayerTakeDamage(float damage)
    {
        curHealth -= damage;
        fillAmount.fillAmount -= damage / 100f; //0.2f;
        Debug.Log("Player health = " + curHealth);
        // Play hurt animation
        animator.SetTrigger("Hurt");

        if (curHealth <= 0f)
        {
            PlayerDeath();
        }
    }

    public void PlayerHealthRegen(float healAmount)
    {
        curHealth += healAmount;
        fillAmount.fillAmount += 0.2f;
        sceneManager.PotionUsed();
    }

    void EnemyDeath()
    {
        Debug.Log("Enemy died");
        animator.SetTrigger("Death");
        enemy.Death();
        GetComponent<Collider2D>().enabled = false; // Disable enemy collider
        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        this.enabled = false;
    }

    void PlayerDeath()
    {
        Debug.Log("Player died");
        animator.SetTrigger("Death");
        //player.death();
        GetComponent<Collider2D>().enabled = false; // Disable player collider
        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        justDied = true;
    }

    /*public void TakeDamage(int damage, Collider2D victim)
    {
        curHealth -= damage;

        if(victim.tag == "Enemy")
        {
            Debug.Log("Enemy health = " + curHealth);
        }

        else if(victim.tag == "Player")
        {
            Debug.Log("Player health = " + curHealth);
        }

        // Play hurt animation
        animator.SetTrigger("Hurt");

        if(curHealth <= 0)
        {
            Die(victim);
        }
    }*/

    /*void Die(Collider2D deadVictim)
    {
        if(deadVictim.tag == "Enemy")
        {
            Debug.Log("Enemy died");
            animator.SetTrigger("Death");
            GetComponent<Collider2D>().enabled = false; // Disable enemy collider
            gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            this.enabled = false;
        }
        else if(deadVictim.tag == "Player")
        {
            Debug.Log("Player died");
        }
    }*/
}
