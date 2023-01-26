using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTile : MonoBehaviour
{
    public float speed = 4f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(new Vector3(speed *  Time.deltaTime, 0, 0));
    }
}
