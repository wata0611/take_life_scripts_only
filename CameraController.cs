using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] Transform player2Pos;
    [SerializeField] PlayerController player;
    [SerializeField] float limitMinX;
    [SerializeField] float limitMaxX;
    [SerializeField] public bool human = false;
    GameMaster gameMaster;
    public bool stop = false;
 
    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(playerPos.position.x + 0.5f, transform.position.y, transform.position.z);
        if (gameMaster.clearInf)
            limitMinX = -3f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!human)
        {
            if (limitMinX <= playerPos.position.x && playerPos.position.x <= limitMaxX)
            {
                stop = false;
                if (player.Direction() < 0)
                    transform.position = new Vector3(playerPos.position.x + player.GetAddDif() + 0.5f, transform.position.y, transform.position.z);
                else if (player.Direction() > 0)
                    transform.position = new Vector3(playerPos.position.x + 0.5f, transform.position.y, transform.position.z);
                else
                {
                    if (player.GetPreDir() < 0)
                        transform.position = new Vector3(playerPos.position.x + player.GetAddDif() + 0.5f, transform.position.y, transform.position.z);
                    else if (player.GetPreDir() > 0)
                        transform.position = new Vector3(playerPos.position.x + 0.5f, transform.position.y, transform.position.z);
                }
            }
            else
                stop = true;
        }
        else
        {
            if (limitMinX <= player2Pos.position.x && player2Pos.position.x <= limitMaxX)
            {
                stop = false;
                transform.position = new Vector3(player2Pos.position.x + +0.5f, transform.position.y, transform.position.z);
            }
            else
                stop = true;
        }
    }
}
