using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Controller : Enemy
{
    [SerializeField] float limitMaxPosX;
    [SerializeField] float limitMinPosX;

    public int Life
    {
        set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (Life <= 0)
            {
                state = State.DEAD;
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
        preState = state;
        prePos = transform.position;
        Life = maxLife;
        attackCollider = GetComponents<BoxCollider2D>()[1];
        attackCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.DEAD)
        {
            if (player2Obj.activeSelf == false)
                targetPos = playerPos;
            else if (playerObj.activeSelf == false)
                targetPos = player2Pos;
            ChangeState();
            ChangeAnimation();
            Move();
        }
    }

    protected override void ChangeState()
    {
        if (!isAttack)
        {
            if (Direction(scaleX) != 0)
                state = State.WALK;
            else
                state = State.IDLE;

            if (DetectTarget(targetPos.position) && TargetDistance(targetPos.position) < attackRange)
                StartCoroutine(AttackTimer());
        }
        prePos = transform.position;
    }

    protected override void ChangeAnimation()
    {
        if (preState != state)
        {
            switch (state)
            {
                case State.IDLE:
                    animator.SetTrigger("Idle");
                    break;
                case State.WALK:
                    animator.SetTrigger("Walk");
                    break;
                case State.ATTACK:
                    animator.SetTrigger("Attack");
                    break;
                default:
                    animator.SetTrigger("Idle");
                    break;
            }
            preState = state;
        }
    }

    protected override void Move()
    {
        if (TargetDistance(targetPos.position)<=detectRange){
            if (!isAttack)
                transform.position = Vector3.MoveTowards(transform.position
                    , new Vector3(targetPos.position.x, transform.position.y, transform.position.z)
                    , Time.deltaTime * speedCoefficient);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && !hit)
        {
            hit = true;
            if(player.Life > 0)
                player.Life--;
        }
        else if (collider.gameObject.tag == "Player2" && !hit)
        {
            hit = true;
            if (player2.LifeSpan > 0)
                player2.LifeSpan -= lifeSpanDamage;
            if (player2.LifeSpan < 0)
                player2.LifeSpan = 0;
        }
    }

    protected override IEnumerator AttackTimer()
    {
        if (!isAttack)
        {
            isAttack = true;
            state = State.ATTACK;
            yield return new WaitForSeconds(damageStart);
            attackCollider.enabled = true;
            yield return new WaitForSeconds(damageFinish-damageStart);
            attackCollider.enabled = false;
            yield return new WaitForSeconds(attackTime-damageFinish);
            isAttack = false;
            hit = false;
        }
    }
}
