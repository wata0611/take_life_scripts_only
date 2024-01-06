using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSord : MonoBehaviour
{
    [SerializeField] GameObject[] sord;
    [SerializeField] float radius = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < sord.Length; i++)
        {
            Debug.Log(i);
            float theta = (Time.time + (2 * Mathf.PI * i) / sord.Length);
            if ( Mathf.PI / 2 <= theta % (2 * Mathf.PI) && theta % (2*Mathf.PI) <= 3*Mathf.PI/2)
                sord[i].GetComponent<SpriteRenderer>().sortingOrder = -1;
            else
                sord[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
            sord[i].transform.localPosition = new Vector3(radius * Mathf.Sin(theta),0.2f,0f);
        }
    }
}
