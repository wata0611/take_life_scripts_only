using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChanger : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] Transform player2Pos;
    [SerializeField] Transform cameraPos;
    [SerializeField] AudioSource[] bgm;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject sin;
    BoxCollider2D[] colliders;
    GameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (gameMaster.clearInf)
            wall.GetComponents<BoxCollider2D>()[1].enabled = false;
        else
            sin.SetActive(false);
        colliders = GetComponents<BoxCollider2D>();
        colliders[1].enabled = false;
        bgm[1].enabled = false;
        boss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && playerPos.position.x >= transform.position.x)
        {
            cameraPos.position = new Vector3(19f, cameraPos.position.y, cameraPos.position.z);
            colliders[0].enabled = false;
            colliders[1].enabled = true;
            bgm[0].enabled = false;
            bgm[1].enabled = true;
            boss.SetActive(true);
        }
        else if (collider.gameObject.tag == "Player2" && player2Pos.position.x >= transform.position.x)
        {
            Debug.Log(collider.gameObject.name);
            cameraPos.position = new Vector3(19f, cameraPos.position.y, cameraPos.position.z);
            colliders[0].enabled = false;
            colliders[1].enabled = true;
            bgm[0].enabled = false;
            bgm[1].enabled = true;
            boss.SetActive(true);
        }

    }
}
