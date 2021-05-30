using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableControls : MonoBehaviour
{
    bool moveAllowed;
    public Collider2D colliderToMove;
    Vector2 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        colliderToMove = GetComponent<Collider2D>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position; //Camera.main.ScreenToWorldPoint(touch.position);
            //touchPosition.z = 0f;

            if(touch.phase == TouchPhase.Began)
            {
                //Debug.Log("Touching began");
                Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                if(colliderToMove == touchedCollider)
                {
                    moveAllowed = true;
                    Debug.Log("Moving is allowed");
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                //Debug.Log("Touch movement sensed");
                //Debug.Log(touchPosition);
                if (moveAllowed)
                {
                    transform.position = new Vector2(touchPosition.x, touchPosition.y);
                    Debug.Log("Should be moving");
                }
            }

            if(touch.phase == TouchPhase.Ended)
            {
                moveAllowed = false;
                //Debug.Log("Moving not allowed");
            }
        }
    }

    public void Reset()
    {
        transform.position = initialPosition;
    }
}
