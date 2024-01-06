using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleGroundController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Rigidbody2D player2Rb;
    [SerializeField] float speed = 1f;
    [SerializeField] float width = 3.84f;
    [SerializeField] int num = 3;
    [SerializeField] public bool human = false;
    [SerializeField] CameraController camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!camera.stop)
        {
            if (!human)
                transform.localPosition += Vector3.left * playerRb.velocity.x * speed * Time.deltaTime;
            else
                transform.localPosition += Vector3.left * player2Rb.velocity.x * speed * Time.deltaTime;
            Move();
        }
    }

    void Move()
    {
        if (transform.localPosition.x < -width * num/2)
            transform.localPosition += Vector3.right * width * num;
        else if (transform.localPosition.x > width * num/2)
            transform.localPosition += Vector3.left * width * num;
            
    }
}
