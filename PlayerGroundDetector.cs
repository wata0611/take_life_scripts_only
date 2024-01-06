using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetector : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerController2 player2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ground" || collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Boss")
        {
            if (!player.isGround)
            {
                player.isGround = true;
                if (!player.GetIsAttack())
                    player.animator.SetTrigger("Landing");
            }
            else if (!player2.isGround)
            {
                player2.isGround = true;
                if (!player2.GetIsAttack())
                    player2.animator.SetTrigger("Landing");
            }
        }
    }

}
