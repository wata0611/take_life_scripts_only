using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] CapsuleCollider2D collider;
    [SerializeField] float limitSizeX = 5f;
    [SerializeField] float limitPosX = 0;
    public AudioClip hitSe;
    PlayerController2 player;
    float Timer = 0f;
    bool isRight = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController2>();
        if (player.Direction() < 0)
            isRight = false;
        else if (player.Direction() > 0)
            isRight = true;
        else
        {
            if (player.GetPreDir() < 0)
                isRight = false;
            else if (player.GetPreDir() > 0)
                isRight = true;
        }
        if (!isRight)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        if (collider.size.x < limitSizeX)
        {
            collider.size = new Vector2(collider.size.x + 1f/3f, collider.size.y);
            if (collider.offset.x < limitPosX)
                collider.offset = new Vector2(collider.offset.x + 1f/6f, collider.offset.y);
            if (collider.offset.x > limitPosX)
                collider.offset = new Vector2(limitPosX, collider.offset.y);
        }
        if (collider.size.x > limitSizeX)
            collider.size = new Vector2(limitSizeX, collider.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRight)
            transform.position += Vector3.right * speed * Time.deltaTime;
        else
            transform.position += Vector3.left * speed * Time.deltaTime;
        Timer += Time.deltaTime;
        if (Timer >= 2f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {

            if (collider.gameObject.name == "Enemy1")
            {
                collider.gameObject.GetComponent<AudioSource>().PlayOneShot(hitSe);
                collider.gameObject.GetComponent<Enemy1Controller>().Life--;
            }
            if (collider.gameObject.name == "Man1")
            {
                collider.gameObject.GetComponent<AudioSource>().PlayOneShot(hitSe);
                collider.gameObject.GetComponent<Man1Controller>().Life--;
            }
            if (collider.gameObject.name == "wagon")
            {
                collider.gameObject.GetComponent<AudioSource>().PlayOneShot(hitSe);
                collider.gameObject.GetComponent<WagonController>().Life--;
            }
            Destroy(gameObject);
        }
        else if (collider.gameObject.tag == "Boss")
        {
            collider.gameObject.GetComponent<AudioSource>().PlayOneShot(hitSe);
            collider.gameObject.GetComponent<BossController>().Life--;
            Destroy(gameObject);
        }
    }
}
