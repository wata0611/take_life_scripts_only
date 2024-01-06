using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//抽象クラス
public abstract class Enemy : MonoBehaviour
{
    protected enum State { IDLE,WALK,ATTACK,DEAD}
    [SerializeField] protected State state = State.IDLE;
    [SerializeField] protected GameObject playerObj;
    [SerializeField] protected GameObject player2Obj;
    [SerializeField] protected Transform playerPos;
    [SerializeField] protected Transform player2Pos;
    [SerializeField] protected Transform targetPos;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected PlayerController2 player2;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float attackTime = 1.0f;
    [SerializeField] protected float damageStart = 1.0f;
    [SerializeField] protected float damageFinish = 1.0f;
    [SerializeField] protected float deadTime = 1.0f;
    [SerializeField] protected float detectRange = 1.0f;
    [SerializeField] protected float attackRange = 0.5f;
    [SerializeField] public float scaleX = 0.5f;
    [SerializeField] protected float hitPower = 5f;
    [SerializeField] protected bool isAttack = false;
    [SerializeField] protected int maxLife = 1;
    [SerializeField] protected int life;
    [SerializeField] protected float speedCoefficient = 0.5f;
    [SerializeField] protected int key = 1;
    [SerializeField] protected bool detect = false;
    [SerializeField] protected bool hit = false;
    [SerializeField] protected int lifeSpanDamage;
    [SerializeField] GameObject[] dropItem;
    [SerializeField] float popForce;
    protected State preState;
    protected float preDir = 0f;
    protected Vector3 prePos;
    protected BoxCollider2D attackCollider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //歩いている向き
    public float Direction(float scaleX)
    {
        if (prePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, 1f);
            //preDir = scaleX;
            return scaleX;
        }
        else if (prePos.x > transform.position.x)
        {
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1f);
            //preDir = -scaleX;
            return -scaleX;
        }
        else
            return 0f;
    }

    //ターゲットが右にいるか左にいるか確認
    //返り値が正なら右、負なら左
    protected float TargetDirection(Vector3 target)
    {
        return target.x - transform.position.x;
    }

    protected float TargetDistance(Vector3 target)
    {
        return Mathf.Sqrt(Mathf.Pow(target.x-transform.position.x,2)
            +Mathf.Pow(target.y-transform.position.y,2));
    }

    protected bool DetectTarget(Vector3 target)
    {
        //Debug.Log(TargetDirection(target));
        //Debug.Log(Direction(scaleX) * key);
        //Debug.Log(TargetDirection(target) * Direction(scaleX) * key > 0);
        if (TargetDirection(target) * Direction(scaleX) * key > 0 && TargetDistance(target) <= detectRange)
            return true;
        return false;
    }

    protected abstract void ChangeState();
    protected abstract void ChangeAnimation();
    protected abstract void Move();
    protected abstract IEnumerator AttackTimer();
    protected virtual IEnumerator DeadTimer()
    {
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(deadTime);
        if (gameObject.tag == "Enemy")
        {
            for (int i = 0; i < dropItem.Length; i++)
                Instantiate(dropItem[i]
                        , new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z)
                        , Quaternion.identity)
                        .GetComponent<Rigidbody2D>().AddForce(new Vector3(0f, 1f, 0f) * popForce);
        }
        state = State.DEAD;
        if(gameObject.tag=="Enemy")
            Destroy(gameObject);
    }
}
