using UnityEngine;
using System.Collections;
//using Pathfinding;

public class Bandit : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;

    // My Additions

    //Patrolling
    [HideInInspector]
    public bool mustPatrol;
    public float nextWaypointDistance = 3f;
    public GameObject[] waypoints;

    private int turnTimerLeft = 10;
    private int turnTimerRight = 10;
    private bool mustTurn = false;
    int currentWaypoint = 0;

    //Layers to interact with
    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    public LayerMask waypointLeftLayer;
    public LayerMask waypointRightLayer;
    public LayerMask playerLayer;

    //Colliders to interact with
    public HealthComponent health;
    public Collider2D bodyCollider;
    public Collider2D groundCollider;
    public Transform groundDetection;
    public Collider2D targetCollider;
    public HeroKnight target;

    //Attacking
    public GameObject e_AttackPointRight;
    public GameObject e_AttackPointLeft;
    public float attackRange = 0.6f;
    public float attackDamage = 20f;
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    private void Awake()
    {
        target = FindObjectOfType<HeroKnight>();
    }

    // Use this for initialization
    void Start () {
        mustPatrol = true;
        bodyCollider = GetComponent<Collider2D>();
        groundCollider = GetComponent<Collider2D>();
        groundDetection = GetComponent<Transform>();
        targetCollider = target.GetComponent<Collider2D>();
        health = GetComponent<HealthComponent>();
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

    void FixedUpdate()
    {
        //Movement calculation
        Vector2 direction = ((Vector2)waypoints[currentWaypoint].transform.position - m_body2d.position).normalized;
        Vector2 force = direction * m_speed * Time.deltaTime;

        m_body2d.AddForce(force);

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

        //Faced direction calculation
        if (groundCollider.IsTouchingLayers(waypointLeftLayer) || e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && !mustTurn) //(groundCollider.IsTouchingLayers(obstacleLayer) || !groundCollider.IsTouchingLayers(groundLayer))
        {
            if (groundCollider.IsTouchingLayers(waypointLeftLayer))
            {
                mustTurn = true;
                currentWaypoint = 1;
            }

            else if (e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
            {
                if(turnTimerLeft >= 10)
                {
                    mustTurn = true;
                    currentWaypoint = 1;
                }
                else if(turnTimerLeft <= 0)
                {
                    turnTimerLeft = 11;
                }

                turnTimerLeft--;
            }

            else
            {

            }
        }
        else if (groundCollider.IsTouchingLayers(waypointRightLayer) || e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && mustTurn)
        {
            if (groundCollider.IsTouchingLayers(waypointRightLayer))
            {
                mustTurn = false;
                currentWaypoint = 0;
            }

            else if (e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
            {
                if (turnTimerRight >= 10)
                {
                    mustTurn = false;
                    currentWaypoint = 0;
                }
                else if (turnTimerRight <= 0)
                {
                    turnTimerRight = 11;
                }

                turnTimerRight--;
            }

            else
            {

            }
        }
        else
        {
            turnTimerRight = 10;
            turnTimerLeft = 10;
        }

        //Swap direction of sprite depending on walk direction
        if (mustTurn)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (!mustTurn)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        //Move
        if (!e_AttackPointLeft.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && !e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
        {
            m_body2d.velocity = new Vector2(force.x * m_speed, m_body2d.velocity.y);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        //Animations

        //Death
        if (Input.GetKeyDown("e"))
        {
            if (!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }

        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        else if (e_AttackPointLeft.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && Time.time >= nextAttackTime || e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && Time.time >= nextAttackTime) {
            attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(force.x) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }

    void attack()
    {
        Vector2 distance = new Vector2 (0, 0);

        //Calculate when to attack
        if(currentWaypoint == 0)
        {
            distance = ((Vector2)targetCollider.transform.localPosition - m_body2d.position).normalized;
        }
        else if(currentWaypoint == 1)
        {
            distance = (m_body2d.position - (Vector2)targetCollider.transform.localPosition).normalized;
        }

        Debug.Log(distance.x);

        //Do damage
        if (health.curHealth <= 0f)
        {
            m_isDead = true;
            m_animator.SetTrigger("Death");
        }
        else if(distance.x <= attackRange)
        {
            m_animator.SetTrigger("Attack");
            target.GetComponent<HeroKnight>().takeDamage(attackDamage);
        }
        else
        {

        }
    }

    public void Death()
    {
        e_AttackPointLeft.SetActive(false);
        e_AttackPointRight.SetActive(false);
    }
}