using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;

    //My Additions

    //Adding touch controls
    public Joystick joystick;
    public float runSpeed = 40f;
    public HealthComponent healthComponent;
    public Button jumpButton;
    public Button blockButton;
    public Button attackButton;
    public Button rollButton;

    //Attacking
    public Transform attackPointRight;
    public Transform attackPointLeft;
    public float attackRange = 0.5f;
    public float attackDamage = 20f;
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    //Other
    public LayerMask enemyLayers;
    public SceneManagement sceneManager;
    public Collider2D playerCollider;
    float gyroDirection = 0f;
    float gyroMoveSpeed = 10f;
    public Collider2D groundCollider;
    float jumpTimer = 0f;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        playerCollider = GetComponent<Collider2D>();
        healthComponent = GetComponent<HealthComponent>();
        sceneManager.GetComponent<SceneManagement>();
        groundCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update ()
    {
        //Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Input and movement
        float inputX = joystick.Horizontal;
        float inputY = joystick.Vertical;

        //Swap direction of sprite depending on walk direction
        if (inputX > 0 || Input.acceleration.x >= 0.2f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0 || Input.acceleration.x <= -0.2f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        //Move
        if (!m_rolling)
        {
            //Uses virtual joystick's horizontal tilt
            if (joystick.Horizontal >= 0.2f || joystick.Horizontal <= -0.2f)
            {
                //Provides velocity to the player, so they move
                m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
            }
            else
            {
                //Only uses the gyroscope if the joystick is not being touched
                if (sceneManager.gyroActive)
                {
                    //Uses the gyroscope's 'x' axis tilt
                    if (Input.acceleration.x >= 0.2f || Input.acceleration.x <= -0.2f)
                    {
                        gyroDirection = Input.acceleration.x * gyroMoveSpeed;
                        m_body2d.velocity = new Vector2(gyroDirection, m_body2d.velocity.y);
                    }
                    else
                    {
                        m_body2d.velocity = new Vector2(inputX * 0f, m_body2d.velocity.y);
                    }
                }
                else
                {
                    m_body2d.velocity = new Vector2(inputX * 0f, m_body2d.velocity.y);
                }
            }
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Animations

        //Wall Slide
        m_animator.SetBool("WallSlide", (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State()));

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(attackButton.IsInvoking() && Time.time >= nextAttackTime)
        {   
            //Ensures the player cannot spam the attack button
            if(m_timeSinceAttack > 0.25f && !m_rolling)
            {
                attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        //Block
        else if (inputY < -0.5 && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        //Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }  

        //Jump
        //Uses the 'z' axis on the gyroscope if active
        else if ((inputY > 0.5 || (Input.acceleration.z >= -0.625f && sceneManager.gyroActive)) && m_grounded && !m_rolling && groundCollider.IsTouchingLayers())
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon || Input.acceleration.x >= 0.2f || Input.acceleration.x <= -0.2f)
        {
            //Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            //Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    //Animation Events
    //Called in end of roll animation.
    void AE_ResetRoll()
    {
        m_rolling = false;
    }

    //Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            //Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            //Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        float r1PosX = m_wallSensorR1.gameObject.transform.position.x;
        float r1PosZ = m_wallSensorR1.gameObject.transform.position.z;
        float r2PosZ = m_wallSensorR2.gameObject.transform.position.z;
    }

    public void jump()
    {
        if (m_grounded && !m_rolling && groundCollider.IsTouchingLayers())
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
    }

    public void block()
    {
        if (!m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
    }

    public void unblock()
    {
        m_animator.SetBool("IdleBlock", false);
    }

    public void attack()
    {
        if(m_facingDirection > 0)
        {
            //Obtains all of the enemies that are overlapping the attack sensors
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointRight.position, attackRange, enemyLayers);

            //Deals damage to each enemy overlapping the attack sensors
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<HealthComponent>().EnemyTakeDamage(attackDamage);
            }
        }
        else if(m_facingDirection < 0)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointLeft.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<HealthComponent>().EnemyTakeDamage(attackDamage);
            }
        }

        m_currentAttack++;

        //Loop back to one after third attack
        if (m_currentAttack > 3)
            m_currentAttack = 1;

        //Reset Attack combo if time since last attack is too large
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;

        //Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);

        //Reset timer
        m_timeSinceAttack = 0.0f;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPointRight == null || attackPointLeft == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
        Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
    }

    public void roll()
    {
        if (!m_rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
    }

    public void healthRegen()
    {
        //Regens health when a potion is picked up, but only usable if the player has 80 or less health remaining
        if(healthComponent.curHealth <= 80f && sceneManager.potionCount >= 1)
        {
            healthComponent.PlayerHealthRegen(20f);
        }
    }

    public void takeDamage(float damage)
    {
        healthComponent.PlayerTakeDamage(damage);
    }

    public void restartGame()
    {
        //Resets the player after death (so they can be controlled again)
        m_animator.SetTrigger("Block");
        playerCollider.enabled = true;
        m_body2d.bodyType = RigidbodyType2D.Dynamic;
    }

    public void death()
    {
        //Turns off all of the controls upon death
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        jumpButton.gameObject.SetActive(false);
        blockButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        rollButton.gameObject.SetActive(false);
        joystick.gameObject.SetActive(false);
        print("The player has died");
    }

    public void gyroControls()
    {
        if (!sceneManager.gyroActive)
        {
            //Checks to see if the device supports gyro controls
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = false;
                Debug.Log("Gyroscope deactivated.");
            }
        }
        else if (sceneManager.gyroActive)
        {
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
                Debug.Log("Gyroscope activated.");
            }
        }
    }
}
