using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Transform coin;
    public float counter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        coin = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        counter += 0.016f;
        if(counter >= 0.25f)
        {
            coin.Rotate(0f, 22.5f, 0f);
            counter = 0f;
        }
    }
}
