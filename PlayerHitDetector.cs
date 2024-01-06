using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitDetector : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerController2 player2;
    [SerializeField] AudioSource audioSource;
    public AudioClip hitSe;

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
        if (collider.gameObject.tag == "Enemy")
        {
            if (collider.gameObject.name == "Enemy1" )
            {
                if (!player.hit)
                {
                    player.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<Enemy1Controller>().Life--;
                }
                else if (!player2.hit)
                {
                    player2.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<Enemy1Controller>().Life--;
                }
            }
            if(collider.gameObject.name == "Man1")
            {
                if (!player.hit)
                {
                    player.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<Man1Controller>().Life--;
                }
                else if (!player2.hit)
                {
                    player2.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<Man1Controller>().Life--;
                }
            }
            if (collider.gameObject.name == "wagon")
            {
                if (!player.hit)
                {
                    player.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<WagonController>().Life--;
                }
                else if (!player2.hit)
                {
                    player2.hit = true;
                    audioSource.PlayOneShot(hitSe);
                    collider.gameObject.GetComponent<WagonController>().Life--;
                }
            }
        }
    }
}
