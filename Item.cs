using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//抽象クラス
public abstract class Item : MonoBehaviour
{
    [SerializeField] protected PlayerController player;
    [SerializeField] protected PlayerController2 player2;
    [SerializeField] protected int addLife = 1;
    [SerializeField] protected int addLifeSpan = 10;
    public AudioClip eatSe;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void Effect1();
    protected abstract void Effect2();

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(eatSe);
            Effect1();
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player2")
        {
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(eatSe);
            Effect2();
            Destroy(gameObject);
        }
    }

}
