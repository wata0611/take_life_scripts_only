using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2 : MonoBehaviour
{
    enum MoveState { Idle, Walk, Jump, Attack, Dead }
    [SerializeField] MoveState moveState = MoveState.Idle;
    [SerializeField] public bool isGround = true;
    [SerializeField] bool isAttack = false;
    [SerializeField] int direction = 1; //1が右、-1が左
    [SerializeField] float jumpThreshold = 2.0f;
    [SerializeField] float jumpForce = 0.5f;
    [SerializeField] float walkThreshold = 2.0f;
    [SerializeField] float walkForce = 0.5f;
    [SerializeField] float walkSpeed = 0.1f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] public Animator animator;
    [SerializeField] int maxLifeSpan = 1000;
    [SerializeField] float attackTimer = 1.0f;
    [SerializeField] float damageStart = 0.2f;
    [SerializeField] float damageFinish = 0.7f;
    [SerializeField] int lifeSpan;
    [SerializeField] float lifeTime;
    [SerializeField] float stateEffect = 1.0f;
    [SerializeField] float preDir = 0f;
    [SerializeField] bool invinsible = false;
    [SerializeField] float invinsibleTime = 2.0f;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] public bool hit = false;
    [SerializeField] BoxCollider2D attackCollider;
    [SerializeField] GameObject skelton;
    [SerializeField] CameraController camera;
    [SerializeField] MiddleGroundController[] middleGround;
    [SerializeField] float deadTime = 1.0f;
    [SerializeField] GameObject specialAttack;
    [SerializeField] float sAttackPosDifY = 0.2f;
    [SerializeField] Text lifeSpanCount;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float limitMinPosX = -1.5f;
    [SerializeField] GameObject changeHuman;
    [SerializeField] GameObject changeHuman2;
    [SerializeField] float limitSpeedX = 2f;
    [SerializeField] float limitSpeedY = 2f;
    [SerializeField] float speed = 0.1f;
    [SerializeField] float speedForce = 0.5f;
    [SerializeField] public bool isPowerUp = false;
    [SerializeField] GameObject swords;
    public AudioClip attackSe;
    public AudioClip changeHumanSe;
    bool once = false;
    int tmpLifeSpan;
    MoveState preMoveState = MoveState.Idle;

    public bool GetInvinsible()
    {
        return invinsible;
    }

    public bool GetIsAttack()
    {
        return isAttack;
    }

    public float GetPreDir()
    {
        return preDir;
    }

    IEnumerator InvinsibleTimer()
    {
        invinsible = true;
        yield return new WaitForSeconds(invinsibleTime);
        invinsible = false;
        renderer.color = new Color(1f, 1f, 1f, 1f);
    }

    public int LifeSpan
    {
        set
        {
            lifeSpan = Mathf.Clamp(value, 0, maxLifeSpan);
            if (lifeSpan <= 0)
            {
                animator.SetTrigger("Dead");
                StartCoroutine(DeadTimer());
            }
            else if (tmpLifeSpan != lifeSpan)
            {
                if (tmpLifeSpan > lifeSpan)
                    StartCoroutine(InvinsibleTimer());
                tmpLifeSpan = lifeSpan;
            }
            lifeSpanCount.text=lifeSpan.ToString();
        }

        get
        {
            return lifeSpan;
        }
    }

    void Awake()
    {
        preMoveState = moveState;
        lifeSpan = maxLifeSpan;
        attackCollider.enabled = false;
        changeHuman.SetActive(false);
        changeHuman2.SetActive(false);
        swords.SetActive(false);
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        StartCoroutine(ChangeHuman());
    }

    IEnumerator ChangeHuman()
    {
        changeHuman.SetActive(true);
        audioSource.PlayOneShot(changeHumanSe);
        yield return new WaitForSeconds(1f/2);
        changeHuman.SetActive(false);
    }

    IEnumerator ChangeHuman2()
    {
        changeHuman2.SetActive(true);
        audioSource.PlayOneShot(changeHumanSe);
        yield return new WaitForSeconds(1f / 2);
        changeHuman2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveState != MoveState.Dead)
        {
            if (!isPowerUp)
            {
                ChangeMoveState();
                ChangeAnimation();
                Move();
            }
            else
            {
                if (!once)
                {
                    once = true;
                    rb.gravityScale = 0.1f;
                    swords.SetActive(true);
                    StartCoroutine(ChangeHuman2());
                }
                Move2();
                ChangeAnimation2();
            }
            if (invinsible)
                renderer.color = new Color(1f, 1f, 1f, Mathf.Sin(Time.time * 15) / 2 + 0.5f);
        }
    }

    public float Direction()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.localScale = new Vector3(-0.6f, transform.localScale.y, 1f);
            if (preDir < 0f)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            preDir = 0.6f;
            return 0.6f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = new Vector3(0.6f, transform.localScale.y, 1f);
            if (preDir > 0f)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            preDir = -0.6f;
            return -0.6f;
        }
        else
            return 0f;
    }

    void ChangeMoveState()
    {
        if (!isAttack)
        {
            if (Mathf.Abs(rb.velocity.y) > jumpThreshold)
                isGround = false;

            if (isGround)
            {
                if (Direction() != 0)
                    moveState = MoveState.Walk;
                else
                    moveState = MoveState.Idle;
            }
            else
                moveState = MoveState.Jump;
        }
        else
            moveState = MoveState.Attack;
    }

    void ChangeAnimation()
    {
        if (moveState != preMoveState)
        {
            switch (moveState)
            {
                case MoveState.Idle:
                    animator.SetTrigger("Idle");
                    stateEffect = 1.0f;
                    break;
                case MoveState.Walk:
                    animator.SetTrigger("Walk");
                    stateEffect = 1.0f;
                    break;
                case MoveState.Jump:
                    animator.SetTrigger("Jump");
                    stateEffect = 0.5f;
                    break;
                case MoveState.Attack:
                    animator.SetTrigger("Attack");
                    break;
                default:
                    animator.SetTrigger("Idle");
                    break;
            }
            preMoveState = moveState;
        }
    }

    void ChangeAnimation2()
    {
        if (moveState != preMoveState)
        {
            switch (moveState)
            {
                case MoveState.Attack:
                    animator.SetTrigger("Attack2");
                    break;
                default:
                    animator.SetTrigger("Idle");
                    break;
            }
            preMoveState = moveState;
        }
    }

    void Move()
    {
        if (!isAttack)
        {
            if (isGround)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rb.AddForce(transform.up * jumpForce);
                    isGround = false;
                }
            }

            float speedX = Mathf.Abs(rb.velocity.x);
            if (speedX < walkThreshold)
                rb.AddForce(transform.right * Direction() * walkForce * stateEffect);
            else
                transform.position += new Vector3(walkSpeed * Time.deltaTime * Direction() * stateEffect, 0f, 0f);

            if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(AttackTimer());
        }
    }

    void Move2()
    {
        if (!isAttack)
        {
            moveState = MoveState.Idle;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float speedX = Mathf.Abs(rb.velocity.x);
            if (speedX < limitSpeedX)
                rb.AddForce(transform.right * speedForce);
            else
                transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            transform.localScale = new Vector3(-0.6f, transform.localScale.y, transform.localScale.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float speedX = Mathf.Abs(rb.velocity.x);
            if (-speedX > -limitSpeedX)
                rb.AddForce(-transform.right * speedForce);
            else
                transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            transform.localScale = new Vector3(0.6f, transform.localScale.y, transform.localScale.z);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            float speedY = Mathf.Abs(rb.velocity.y);
            if (speedY < limitSpeedY)
                rb.AddForce(transform.up * speedForce);
            else
                transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            float speedY = Mathf.Abs(rb.velocity.y);
            if (-speedY > -limitSpeedY)
                rb.AddForce(-transform.up * speedForce);
            else
                transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.A))
            StartCoroutine(AttackTimer2());
    }

    IEnumerator AttackTimer()
    {
        if (!isAttack)
        {
            isAttack = true;
            moveState = MoveState.Attack;
            yield return new WaitForSeconds(damageStart);
            attackCollider.enabled = true;
            audioSource.PlayOneShot(attackSe);
            Vector3 attackPos = new Vector3(transform.position.x,transform.position.y+sAttackPosDifY,transform.position.z);
            Instantiate(specialAttack, attackPos, Quaternion.identity);
            yield return new WaitForSeconds(damageFinish - damageStart);
            attackCollider.enabled = false;
            yield return new WaitForSeconds(attackTimer - damageFinish);
            isAttack = false;
            hit = false;
        }
    }

    IEnumerator AttackTimer2()
    {
        if (!isAttack)
        {
            isAttack = true;
            moveState = MoveState.Attack;
            animator.SetTrigger("Attack2");
            audioSource.PlayOneShot(attackSe);
            Vector3 attackPos = new Vector3(transform.position.x, transform.position.y + sAttackPosDifY, transform.position.z);
            Instantiate(specialAttack, attackPos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            isAttack = false;
        }
    }

    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(1.0f);
        skelton.SetActive(true);
        skelton.transform.position = transform.position;
        camera.human = false;
        isPowerUp = false;
        for(int i=0;i<middleGround.Length;i++)
            middleGround[i].human = false;
        gameObject.SetActive(false);
    }
}
