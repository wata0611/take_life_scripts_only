using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Man1Controller : NPC
{
    public int Life
    {
        set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (Life <= 0)
            {
                state = State.DEAD;
                animator.SetTrigger("Idle");
                animator.enabled = false;
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
        Life = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        if(state!=State.DEAD)
            Move();
    }

    protected override void Move()
    {
        if (!moveChange)
            StartCoroutine(MoveChangeTimer());
        switch (moveState)
        {
            case MoveState.RIGHT:
                transform.position += Vector3.right * speed * Time.deltaTime;
                break;
            case MoveState.LEFT:
                transform.position += Vector3.left * speed * Time.deltaTime;
                break;
        }
    }

}
