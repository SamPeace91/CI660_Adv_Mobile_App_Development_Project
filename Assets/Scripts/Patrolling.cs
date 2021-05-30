using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrolling : MonoBehaviour
{
    public float speed;
    //public float distance;
    [HideInInspector]
    public bool mustPatrol;

    //private Vector2 force;
    //private bool movingRight = true;
    private bool mustTurn;

    public LayerMask obstacleLayer;
    public LayerMask groundLayer;

    public Animator animator;
    public Collider2D bodyCollider;
    public Collider2D groundCollider;
    public Transform groundDetection;
    public Rigidbody2D rb;

    private void Start()
    {
        mustPatrol = true;
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        groundCollider = GetComponent<Collider2D>();
        groundDetection = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (mustPatrol)
        {
            animator.SetTrigger("Run");
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            if(groundCollider.IsTouchingLayers(obstacleLayer) || !groundCollider.IsTouchingLayers(groundLayer))
            {
                mustTurn = true;
            }
            else
            {
                mustTurn = false;
            }
            //mustTurn = !Physics2D.OverlapCircle(groundDetection.position, 0.1f, groundLayer);
        }

        rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void Patrol()
    {
        if (mustTurn) //|| bodyCollider.IsTouchingLayers(groundLayer))
        {
            Flip();
        }

        //rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        speed *= -1;
        mustPatrol = true;
    }

    /*private void FixedUpdate()
    {
        if (movingRight)
        {
            force = Vector2.right * speed * Time.deltaTime;
            rb.AddForce(force);
        }
        else
        {
            force = Vector2.left * speed * Time.deltaTime;
            rb.AddForce(force);
        }

        //transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

        if (groundInfo.collider == false)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }*/
}
