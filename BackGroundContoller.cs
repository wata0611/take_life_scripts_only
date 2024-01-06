using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundContoller : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float width = 3.84f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.x >= width*2)
            transform.localPosition -= Vector3.right * width * 3;
        transform.localPosition += Vector3.right * speed * Time.deltaTime;
    }
}
