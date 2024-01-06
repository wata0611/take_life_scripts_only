using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    enum MoveState { Idle, Walk, Jump, Attack, Dead}
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
    [SerializeField] int maxLife = 3;
    [SerializeField] float attackTimer = 1.0f;
    [SerializeField] float damageStart = 0.2f;
    [SerializeField] float damageFinish = 0.7f;
    [SerializeField] int life;
    [SerializeField] float lifeTime;
    [SerializeField] float stateEffect = 1.0f;
    [SerializeField] float preDir = 0f;
    [SerializeField] float addDif=0.5f;
    [SerializeField] bool invinsible = false;
    [SerializeField] float invinsibleTime = 2.0f;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] public bool hit = false;
    [SerializeField] BoxCollider2D attackCollider;
    [SerializeField] int meatCount = 0;
    [SerializeField] int maxMeatCount = 3;
    [SerializeField] GameObject human;
    [SerializeField] CameraController camera;
    [SerializeField] MiddleGroundController[] middleGround;
    [SerializeField] float humanPosDifY = -0.25f;
    [SerializeField] float humanPosDifX = -0.1f;
    [SerializeField] float limitMinPosX = -1.5f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceBossBGM;
    [SerializeField] AudioSource audioSourceDeadBGM;
    [SerializeField] GameObject deadCanvas;
    [SerializeField] Image deadPanel;
    [SerializeField] Image deadPanel2;
    [SerializeField] Text deadCenterText;
    [SerializeField] Text deadUnderText;
    [SerializeField] float deadInterval = 0.5f;
    [SerializeField] float loadStartTimer = 0.1f;
    [SerializeField] float seInterval = 5f;
    public AudioClip attackSe;
    public AudioClip demainSe;
    int tmpLife;
    bool once = false;
    MoveState preMoveState = MoveState.Idle;
    float nextInterval = 0f;
    bool sceneChange = false;
    GameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    public bool GetInvinsible()
    {
        return invinsible;
    }

    public bool GetIsAttack()
    {
        return isAttack;
    }

    public float GetAddDif()
    {
        return addDif;
    }

    public float GetPreDir()
    {
        return preDir;
    }

    public int GetMaxMeatCount()
    {
        return maxMeatCount;
    }

    public int GetMaxLife()
    {
        return maxLife;
    }

    IEnumerator InvinsibleTimer()
    {
        invinsible = true;
        yield return new WaitForSeconds(invinsibleTime);
        invinsible = false;
        renderer.color = new Color(1f, 1f, 1f, 1f);
    }

    public int Life
    {
        set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (life <= 0)
            {
                animator.SetTrigger("Dead");
                moveState = MoveState.Dead;
                audioSourceBGM.enabled = false;
                audioSourceBossBGM.enabled = false;
                audioSourceDeadBGM.enabled = true;
            }
            else if (tmpLife != life)
            {
                if (tmpLife > life)
                    StartCoroutine(InvinsibleTimer());
                tmpLife = life;
            }
        }

        get
        {
            return life;
        }
    }

    public int MeatCount
    {
        set
        {
            meatCount = Mathf.Clamp(value, 0, maxMeatCount);
            if (meatCount >= 3)
            {
                meatCount = 0;
                Life = maxLife;
                human.SetActive(true);
                human.GetComponent<PlayerController2>().LifeSpan = 50;
                if(Direction()>0)
                    human.transform.position = new Vector3(transform.position.x+humanPosDifX
                        , transform.position.y + humanPosDifY
                        , transform.position.z);
                else if(Direction()<0)
                    human.transform.position = new Vector3(transform.position.x - humanPosDifX
    , transform.position.y + humanPosDifY
    , transform.position.z);
                else
                {
                    if(GetPreDir()>0)
                        human.transform.position = new Vector3(transform.position.x + humanPosDifX
    , transform.position.y + humanPosDifY
    , transform.position.z);
                    else if(GetPreDir()<0)
                        human.transform.position = new Vector3(transform.position.x - humanPosDifX
, transform.position.y + humanPosDifY
, transform.position.z);
                }
                camera.human = true;
                for(int i=0;i < middleGround.Length; i++)
                    middleGround[i].human = true;
                gameObject.SetActive(false);
            }
        }

        get
        {
            return meatCount;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        preMoveState = moveState;
        life = maxLife;
        tmpLife = life;
        meatCount = 0;
        if(gameMaster.clearHuman || gameMaster.clearInf)
            MeatCount = 3;
        attackCollider.enabled = false;
        deadCanvas.SetActive(false);
        audioSourceDeadBGM.enabled = false;
        //clearCanvas.SetActive(false);
    }

    IEnumerator ChangeSceneSETimer()
    {
        if (!sceneChange)
        {
            sceneChange = true;
            audioSourceDeadBGM.enabled = false;
            audioSource.PlayOneShot(demainSe);
            yield return new WaitForSeconds(seInterval);
            SceneManager.LoadScene("TitleScene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveState != MoveState.Dead)
        {
            ChangeMoveState();
            ChangeAnimation();
            Move();
            if (invinsible)
                renderer.color = new Color(1f, 1f, 1f, Mathf.Sin(Time.time * 15) / 2 + 0.5f);
        }
        else
            Dead();
    }

    public float Direction()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 1f);
            if(preDir < 0f)
                transform.position = new Vector3(transform.position.x + addDif, transform.position.y, transform.position.z);
            preDir = 0.5f;
            return 0.5f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = new Vector3(-0.5f, transform.localScale.y, 1f);
            if(preDir > 0f) 
                transform.position = new Vector3(transform.position.x - addDif, transform.position.y, transform.position.z);
            preDir = -0.5f;
            return -0.5f;
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
        if (moveState!=preMoveState)
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

    IEnumerator AttackTimer()
    {
        if (!isAttack)
        {
            isAttack = true;
            moveState = MoveState.Attack;
            yield return new WaitForSeconds(damageStart);
            audioSource.PlayOneShot(attackSe);
            attackCollider.enabled = true;
            yield return new WaitForSeconds(damageFinish - damageStart);
            attackCollider.enabled = false;
            yield return new WaitForSeconds(attackTimer-damageFinish);
            isAttack = false;
            hit = false;
        }
    }
    void Dead()
    {
        if (!once)
        {
            once = true;
            nextInterval = Time.time;
            deadCanvas.SetActive(true);
            gameMaster.clearHuman = false;
            gameMaster.clearInf = false;
        }
        else
        {
            if(!sceneChange)
                deadPanel.color = Color.Lerp(deadPanel.color, new Color(0f, 0f, 0f, 100f / 255f),Time.deltaTime);
            else
                deadPanel2.color = Color.Lerp(deadPanel2.color, new Color(0f, 0f, 0f, 1f), Time.deltaTime*2);
            deadCenterText.color = Color.Lerp(deadCenterText.color, new Color(1f, 0f, 0f,1f), Time.deltaTime);
            if(Time.time > nextInterval)
            {
                deadUnderText.enabled = !deadUnderText.enabled;
                nextInterval += deadInterval;
            }
            if (deadUnderText.enabled)
                deadUnderText.color = Color.Lerp(deadUnderText.color, new Color(1f, 1f, 1f, 1f), Time.deltaTime * 2);
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(ChangeSceneSETimer());
        }
    }


}
