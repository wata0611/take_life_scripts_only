using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecialAttackController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int lifeSpanDamage = 10;
    [SerializeField] CapsuleCollider2D collider;
    [SerializeField] float limitSizeX = 5f;
    [SerializeField] float limitPosX = 0f;
    public AudioClip hitSe;
    BossController boss;
    float Timer = 0f;
    bool isRight = true;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();
        if (boss.moveState == BossController.MoveState.RIGHT)
            isRight = true;
        else if (boss.moveState==BossController.MoveState.LEFT)
            isRight = false;
        /**else
        {
            if (boss.GetPreDir() < 0)
                isRight = false;
            else if (boss.GetPreDir() > 0)
                isRight = true;
        }**/
        if (!isRight)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        if (collider.size.x < limitSizeX)
        {
            collider.size = new Vector2(collider.size.x + 1f / 3f, collider.size.y);
            if (collider.offset.x < limitPosX)
                collider.offset = new Vector2(collider.offset.x + 1f / 6f, collider.offset.y);
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
        if (Timer >= 3f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            if (player.Life > 0)
                player.Life--;
            Destroy(gameObject);
        }
        else if (collider.gameObject.tag == "Player2")
        {
            PlayerController2 player2 = collider.gameObject.GetComponent<PlayerController2>();
            if (player2.LifeSpan > 0)
                player2.LifeSpan -= lifeSpanDamage;
            if (player2.LifeSpan < 0)
                player2.LifeSpan = 0;
            Destroy(gameObject);
        }
    }
}
