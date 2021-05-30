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
    [HideInInspector]
    public bool mustPatrol;
    //bool reachedEndOfPath = false;
    public float nextWaypointDistance = 3f;
    public GameObject[] waypoints;

    private int turnTimerLeft = 10;
    private int turnTimerRight = 10;
    //private bool isAttacking = false;
    //private Vector2 distance;
    //private Vector2 direction;
    //private Vector2 force;
    private bool mustTurn = false;
    int currentWaypoint = 0;
    //private Path path;
    //private Seeker seeker;

    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    public LayerMask waypointLeftLayer;
    public LayerMask waypointRightLayer;
    public LayerMask playerLayer;

    public HealthComponent health;
    public Collider2D bodyCollider;
    public Collider2D groundCollider;
    public Transform groundDetection;
    public Collider2D targetCollider;
    public HeroKnight target;

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
        // Added these four:
        mustPatrol = true;
        //seeker = GetComponent<Seeker>();
        bodyCollider = GetComponent<Collider2D>();
        groundCollider = GetComponent<Collider2D>();
        groundDetection = GetComponent<Transform>();
        targetCollider = target.GetComponent<Collider2D>();
        //targetCollider = GetComponent<Collider2D>();
        //target = GetComponent<HeroKnight>();
        health = GetComponent<HealthComponent>();

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        //e_AttackPointLeft = GetComponent<GameObject>();
        //e_AttackPointRight = GetComponent<GameObject>();
        //InvokeRepeating("UpdatePath", 0f, 0.5f); // Added
    }

    /*void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(m_body2d.position, /*target.position waypoints[currentWaypoint].transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            if(currentWaypoint == 0)
            {
                path = p;
                currentWaypoint = 1;
            }

            else if (currentWaypoint == 1)
            {
                path = p;
                currentWaypoint = 0;
            }

            else
            {
                Debug.Log("Waypoint error");
            }
        }
    }*/

    void FixedUpdate()
    {
        /*if (path == null)
        {
            return;
        }*/

        /*if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }*/

        //Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - m_body2d.position).normalized;
        Vector2 direction = ((Vector2)waypoints[currentWaypoint].transform.position - m_body2d.position).normalized;
        Vector2 force = direction * m_speed * Time.deltaTime;

        m_body2d.AddForce(force);

        //float distance = Vector2.Distance(m_body2d.position, path.vectorPath[currentWaypoint]);

        /*if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }*/

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

        // -- Handle input and movement --
        //float inputX = Input.GetAxis("Horizontal");
        //direction = ((Vector2)path.vectorPath[currentWaypoint] - m_body2d.position).normalized;
        //force = direction * m_speed * Time.deltaTime;

        if (groundCollider.IsTouchingLayers(waypointLeftLayer) || e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && !mustTurn) //(groundCollider.IsTouchingLayers(obstacleLayer) || !groundCollider.IsTouchingLayers(groundLayer))
        {
            if (groundCollider.IsTouchingLayers(waypointLeftLayer))
            {
                mustTurn = true;
                currentWaypoint = 1;
                //Debug.Log("Hit left waypoint");
            }

            else if (e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
            {
                if(turnTimerLeft >= 10)
                {
                    mustTurn = true;
                    currentWaypoint = 1;
                    //Debug.Log("Attacking right side");
                }
                else if(turnTimerLeft <= 0)
                {
                    turnTimerLeft = 11;
                    //Debug.Log("turnTimerLeft: " + turnTimerLeft);
                }

                turnTimerLeft--;
                //Debug.Log("turnTimerLeft: " + turnTimerLeft);
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
                //Debug.Log("Hit right waypoint");
            }

            else if (e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
            {
                if (turnTimerRight >= 10)
                {
                    mustTurn = false;
                    currentWaypoint = 0;
                    //Debug.Log("Attacking right side");
                }
                else if (turnTimerRight <= 0)
                {
                    turnTimerRight = 11;
                    //Debug.Log("turnTimerRight: " + turnTimerRight);
                }

                turnTimerRight--;
                //Debug.Log("turnTimerRight: " + turnTimerRight);
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

        // Swap direction of sprite depending on walk direction
        if (mustTurn) //(inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (!mustTurn) //(inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Move
        if (!e_AttackPointLeft.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && !e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer))
        {
            m_body2d.velocity = new Vector2(/*inputX*/force.x * m_speed, m_body2d.velocity.y);
            //Debug.Log("Not currently attacking");
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
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
        else if (e_AttackPointLeft.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && Time.time >= nextAttackTime || e_AttackPointRight.GetComponent<Collider2D>().IsTouchingLayers(playerLayer) && Time.time >= nextAttackTime) { //(Input.GetMouseButtonDown(0)) {
            attack();
            nextAttackTime = Time.time + 1f / attackRate;
            //m_animator.SetTrigger("Attack");
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
        else if (Mathf.Abs(/*inputX*/force.x) > Mathf.Epsilon)
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

        if(currentWaypoint == 0)
        {
            distance = ((Vector2)targetCollider.transform.localPosition - m_body2d.position).normalized;
        }
        else if(currentWaypoint == 1)
        {
            distance = (m_body2d.position - (Vector2)targetCollider.transform.localPosition).normalized;
        }

        //(Vector2)e_AttackPointLeft.transform.localPosition).normalized;
        //Vector2 distance = ((Vector2)targetCollider.transform.position - m_body2d.position).normalized;
        Debug.Log(distance.x);

        if (health.curHealth <= 0f)
        {
            m_isDead = true;
            m_animator.SetTrigger("Death");
        }
        else if(distance.x <= attackRange)
        {
            //isAttacking = true;
            m_animator.SetTrigger("Attack");
            //Debug.Log("Player in range");
            //Debug.Log("Enemy is attacking");
            target.GetComponent<HeroKnight>().takeDamage(attackDamage);
            //target.takeDamage(attackDamage);
        }
        else
        {
            //isAttacking = false;
        }
    }

    public void Death()
    {
        e_AttackPointLeft.SetActive(false);
        e_AttackPointRight.SetActive(false);
    }

        // Update is called once per frame
        /*void Update () {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");
        direction = ((Vector2)path.vectorPath[currentWaypoint] - m_body2d.position).normalized;
        force = direction * m_speed * Time.deltaTime;

        if (groundCollider.IsTouchingLayers(obstacleLayer) || !groundCollider.IsTouchingLayers(groundLayer))
        {
            mustTurn = true;
        }
        else
        {
            mustTurn = false;
        }

        // Swap direction of sprite depending on walk direction
        if (mustTurn) //(inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (!mustTurn) //(inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Move
        m_body2d.velocity = new Vector2(//inputX Time.fixedDeltaTime * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e")) {
            if(!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }
            
        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        /*else if(Input.GetMouseButtonDown(0)) {
            m_animator.SetTrigger("Attack");
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded) {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }*/
}