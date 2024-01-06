using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Item
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (GameObject.FindGameObjectWithTag("Player2") != null)
            player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController2>();
    }

    protected override void Effect1()
    {
        //player.Life=0;
    }
    protected override void Effect2()
    {
        player2.LifeSpan += addLifeSpan;
    }
}
