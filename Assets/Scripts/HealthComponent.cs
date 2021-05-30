using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    public float curHealth;
    public float maxHealth = 100f;
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
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Sets delay for the lose screen to appear
        if(justDied == true)
        {
            deathDelay += 0.016f;

            if (deathDelay >= 2f)
            {
                Debug.Log("WE'RE IN!");
                justDied = false;
                deathDelay = 0;
                sceneManager.LoseScreen();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public void EnemyTakeDamage(float damage)
    {
        curHealth -= damage;
        Debug.Log("Enemy health = " + curHealth);
        animator.SetTrigger("Hurt");

        if (curHealth <= 0f)
        {
            EnemyDeath();
        }
    }

    public void PlayerTakeDamage(float damage)
    {
        curHealth -= damage;
        fillAmount.fillAmount -= damage / 100f;
        Debug.Log("Player health = " + curHealth);
        animator.SetTrigger("Hurt");

        if (curHealth <= 0f)
        {
            PlayerDeath();
        }
    }

    public void PlayerHealthRegen(float healAmount)
    {
        //Fill amount calculated, so health bar is the same as current health
        curHealth += healAmount;
        fillAmount.fillAmount += 0.2f;
        sceneManager.PotionUsed();
    }

    void EnemyDeath()
    {
        Debug.Log("Enemy died");
        animator.SetTrigger("Death");
        enemy.Death();
        GetComponent<Collider2D>().enabled = false; //Disable enemy collider
        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //Stops input
        this.enabled = false; //Disables enemy's health component
    }

    void PlayerDeath()
    {
        Debug.Log("Player died");
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false; //Disable player collider
        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //Stops input
        justDied = true;
    }
}
