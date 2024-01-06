using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    protected enum State { IDLE, WALK, DEAD}
    protected enum MoveState { LEFT,RIGHT,IDLE}
    [SerializeField] protected MoveState moveState = MoveState.IDLE;
    [SerializeField] protected State state = State.IDLE;
    [SerializeField] protected Transform playerPos;
    [SerializeField] protected Transform player2Pos;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected PlayerController2 player2;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float deadTime = 1.0f;
    [SerializeField] protected float moveChangeTime = 3f;
    [SerializeField] protected float scaleX = 0.6f;
    [SerializeField] protected float detectRange = 1.0f;
    [SerializeField] protected int maxLife = 1;
    [SerializeField] protected int life = 1;
    [SerializeField] protected float speedCoefficient = 0.5f;
    [SerializeField] protected int key = 1;
    [SerializeField] protected GameObject[] dropItem;
    [SerializeField] protected float popForce=22;
    [SerializeField] protected bool moveChange = false;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected float limitMinPosX;
    [SerializeField] protected float limitMaxPosX;
    bool dead = false;
    protected Vector3 prePos;
    protected State preState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected float Direction(float scaleX)
    {
        if (prePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, 1f);
            return scaleX;
        }
        else if (prePos.x > transform.position.x)
        {
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1f);
            return -scaleX;
        }
        else
            return 0f;
    }

    protected float TargetDirection(Vector3 target)
    {
        return target.x - transform.position.x;
    }

    protected float TargetDistance(Vector3 target)
    {
        return Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2)
            + Mathf.Pow(target.y - transform.position.y, 2));
    }

    protected bool DetectTarget(Vector3 target)
    {
        if (TargetDirection(target) * Direction(scaleX) * key > 0 && TargetDistance(target) <= detectRange)
            return true;
        return false;
    }

    protected void ChangeAnimation()
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
            }
            preState = state;
        }
    }

    protected abstract void Move();

    protected IEnumerator DeadTimer()
    {
        if (!dead)
        {
            dead = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            yield return new WaitForSeconds(deadTime);
            for (int i = 0; i < dropItem.Length; i++)
                Instantiate(dropItem[i]
                    , new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z)
                    , Quaternion.identity)
                    .GetComponent<Rigidbody2D>().AddForce(new Vector3(0f, 1f, 0f) * popForce);
            Destroy(gameObject);
        }
    }

    protected IEnumerator MoveChangeTimer()
    {
        moveChange = true;
        moveState = (MoveState)Random.Range(0, 3);
        switch (moveState)
        {
            case MoveState.LEFT:
                animator.SetTrigger("Walk");
                transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1f);
                break;
            case MoveState.RIGHT:
                animator.SetTrigger("Walk");
                transform.localScale = new Vector3(+scaleX, transform.localScale.y, 1f);
                break;
            default:
                animator.SetTrigger("Idle");
                break;
        }
        yield return new WaitForSeconds(moveChangeTime);
        moveChange = false;
    }
}
