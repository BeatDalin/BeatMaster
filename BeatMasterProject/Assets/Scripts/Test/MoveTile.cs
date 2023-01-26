using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTile : MonoBehaviour
{

    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1 * speed *  Time.deltaTime, 0, 0);
    }
}
