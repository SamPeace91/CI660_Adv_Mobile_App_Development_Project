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
        //Captures initial position of the controls for later use
        colliderToMove = GetComponent<Collider2D>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            //Obtains all of the current touch inputs
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                //Checks to see if the touch is interacting with the control's collider
                Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                if(colliderToMove == touchedCollider)
                {
                    moveAllowed = true;
                    Debug.Log("Moving is allowed");
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                //Only moves the controller if the collider is overlapping
                if (moveAllowed)
                {
                    transform.position = new Vector2(touchPosition.x, touchPosition.y);
                    Debug.Log("Should be moving");
                }
            }

            if(touch.phase == TouchPhase.Ended)
            {
                moveAllowed = false;
            }
        }
    }

    public void Reset()
    {
        //Reset the control's position to its original state
        transform.position = initialPosition;
    }
}
