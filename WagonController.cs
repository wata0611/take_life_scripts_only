using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonController : MonoBehaviour
{
    [SerializeField] GameObject[] dropItem;
    int maxLife = 1;
    int life;

    public int Life
    {
        set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (life <= 0)
            {
                for (int i = 0; i < dropItem.Length; i++)
                    Instantiate(dropItem[i]
                        , new Vector3(transform.position.x+i*0.01f, transform.position.y + 0.2f, transform.position.z)
                        , Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(transform.up * 50f);
                StartCoroutine(DeadTimer());
            }
        }

        get
        {
            return life;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
