using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] obj;
    [SerializeField] Text centerText;
    [SerializeField] Text underText;
    [SerializeField] Image panel;
    [SerializeField] PlayerController player;
    [SerializeField] bool debug = false;
    bool start = false;
    bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!debug)
            SceneManager.UnloadScene("TitleScene");
        for (int i = 0; i < obj.Length; i++)
            obj[i].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Title()
    {
        if (Input.anyKey && !start)
        { 
            
        }
    }
}
