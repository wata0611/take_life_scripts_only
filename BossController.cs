using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossController : Enemy
{
    public enum MoveState { LEFT, RIGHT, IDLE}
    [SerializeField] public MoveState moveState = MoveState.IDLE;
    [SerializeField] GameObject specialAttack;
    [SerializeField] float sAttackPosDifY = 0.2f;
    [SerializeField] float speed;
    [SerializeField] GameObject clearCanvas;
    [SerializeField] Image clearPanel;
    [SerializeField] Image clearPanel2;
    [SerializeField] Text clearUpperText;
    [SerializeField] Text clearCenterText;
    [SerializeField] Text clearUnderText;
    [SerializeField] float seInterval = 5f;
    [SerializeField] float clearInterval = 0.5f;
    [SerializeField] AudioSource audioSourceSe;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceInfBGM;
    [SerializeField] Transform specialAttackPos;
    //[SerializeField] float 
    public AudioClip demainSe;
    public AudioClip clearSe;
    bool once = false;
    bool isSpecialAttack = false;
    float nextInterval = 0f;
    bool sceneChange = false;
    GameMaster gameMaster;

    public int Life
    {
        set
        {
            life = Mathf.Clamp(value, 0, maxLife);
            if (life <= 0)
            {
                state = State.DEAD;
                StartCoroutine(DeadTimer());
            }
            /**if(life <= maxLife / 2)
            {
                animator.SetTrigger("Stan");
                isSpecialAttack = true;
            }**/
        }

        get
        {
            return life;
        }
    }

    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        preState = preState;
        prePos = transform.position;
        Life = maxLife;
        attackCollider = GetComponent<BoxCollider2D>();
        clearCanvas.SetActive(false);
        audioSourceInfBGM.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {

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
        else
            Dead();
    }

    protected override void ChangeState()
    {
        if (!isAttack)
        {
            if (Direction(scaleX) != 0)
                state = State.WALK;
            else
                state = State.IDLE;

            if (TargetDistance(targetPos.position) < attackRange)
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
        if (!isAttack && !isSpecialAttack)
        {
            if (transform.position.x < targetPos.position.x)
            {
                moveState = MoveState.RIGHT;
                transform.localScale = new Vector3(scaleX, transform.localScale.y, 1f);
                state = State.WALK;
            }
            else
            {
                moveState = MoveState.LEFT;
                transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1f);
                state = State.WALK;
            }
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && !hit)
        {
            hit = true;
            if (player.Life > 0)
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
            if (transform.position.x < targetPos.position.x)
            {
                moveState = MoveState.RIGHT;
                transform.localScale = new Vector3(scaleX, transform.localScale.y, 1f);
            }
            else
            {
                moveState = MoveState.LEFT;
                transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1f);
            }
            state = State.ATTACK;
            yield return new WaitForSeconds(damageStart);
            attackCollider.enabled = true;
            Vector3 attackPos = new Vector3(transform.position.x, transform.position.y + sAttackPosDifY, transform.position.z);
            Instantiate(specialAttack, attackPos, Quaternion.identity);
            yield return new WaitForSeconds(damageFinish - damageStart);
            attackCollider.enabled = false;
            state = State.IDLE;
            yield return new WaitForSeconds(attackTime - damageFinish);
            isAttack = false;
            hit = false;
        }
    }

    void SpecialAttack()
    {
        if (transform.position != specialAttackPos.position)
            transform.position = Vector3.MoveTowards(transform.position, specialAttackPos.position, Time.deltaTime);
        else
            StartCoroutine(SpecialAttackTimer());
    }

    IEnumerator SpecialAttackTimer()
    {
        yield return new WaitForSeconds(1f);
        isSpecialAttack = false;
    }

    void Dead()
    {
        if (!once)
        {
            once = true;
            audioSourceBGM.enabled = false;
            //audioSourceSe.PlayOneShot(clearSe);
            nextInterval = Time.time;
            clearCanvas.SetActive(true);
            clearPanel.color = new Color(1f, 1f, 1f, 100f / 255f);
            clearCenterText.text = "あなたのランクは　";
            Debug.Log(playerObj.activeSelf);
            Debug.Log(player2Obj.activeSelf);
            if (!player2Obj.activeSelf)
            {
                clearCenterText.text += "骸　";
                gameMaster.clearHuman = false;
                gameMaster.clearInf = false;
            }
            else if (!playerObj.activeSelf)
            {
                if (player2.LifeSpan >= 150)
                {
                    gameMaster.clearHuman = false;
                    gameMaster.clearInf = true;
                    clearCenterText.text += "不死　";
                }
                else if (player2.LifeSpan >= 100)
                {
                    gameMaster.clearHuman = true;
                    gameMaster.clearInf = false;
                    clearCenterText.text += "超長寿　";
                }
                else if (player2.LifeSpan >= 80)
                {
                    gameMaster.clearHuman = true;
                    gameMaster.clearInf = false;
                    clearCenterText.text += "長寿　";
                }
                else if (player2.LifeSpan >= 40)
                {
                    gameMaster.clearHuman = true;
                    gameMaster.clearInf = false;
                    clearCenterText.text += "普通　";
                }
                else if (player2.LifeSpan >= 0)
                {
                    gameMaster.clearHuman = true;
                    gameMaster.clearInf = false;
                    clearCenterText.text += "早世　";
                }
            }
            clearCenterText.text += "です。";
            if (gameMaster.clearInf)
                audioSourceInfBGM.enabled = true;
            else
                audioSourceSe.PlayOneShot(clearSe);
        }
        else
        {
            if (!sceneChange)
                clearPanel.color = Color.Lerp(clearPanel.color, new Color(1f, 1f, 1f, 100f / 255f), Time.deltaTime);
            else
                clearPanel2.color = Color.Lerp(clearPanel2.color, new Color(0f, 0f, 0f, 1f), Time.deltaTime * 2);
            //clearCenterText.color = Color.Lerp(clearCenterText.color, new Color(1f, 1f, 1f, 1f), Time.deltaTime);
            if (Time.time > nextInterval)
            {
                clearUnderText.enabled = !clearUnderText.enabled;
                nextInterval += clearInterval;
            }
            if (clearUnderText.enabled)
                clearUnderText.color = Color.Lerp(clearUnderText.color, new Color(1f, 1f, 1f, 1f), Time.deltaTime * 2);
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(ChangeSceneSETimer());
        }
    }

    IEnumerator ChangeSceneSETimer()
    {
        if (!sceneChange)
        {
            sceneChange = true;
            audioSourceBGM.enabled = false;
            audioSourceSe.PlayOneShot(demainSe);
            yield return new WaitForSeconds(seInterval);
            SceneManager.LoadScene("TitleScene");
        }
    }
}
