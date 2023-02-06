using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    private Rigidbody2D _rb2;
    private ParticleController _particleController;

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        _rb2 = GetComponent<Rigidbody2D>();
        _particleController = GetComponent<ParticleController>();
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.LeftArrow))
    //     {
    //         _rb2.velocity = Vector2.left * speed + _rb2.velocity.y * Vector2.up;
    //     }
    //     else if (Input.GetKey(KeyCode.RightArrow))
    //     {
    //         _rb2.velocity = Vector2.right * speed + _rb2.velocity.y * Vector2.up;
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         _rb2.AddForce(Vector2.up * (speed * 2), ForceMode2D.Impulse);
    //     }
    // }
}
