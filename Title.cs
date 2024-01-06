using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] Text underText;
    [SerializeField] float loadStartTimer=0.1f;
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceHumanBGM;
    [SerializeField] AudioSource audioSourceSe;
    [SerializeField] float interval = 1.0f;
    [SerializeField] float seInterval = 4f;
    [SerializeField] GameObject skelton;
    [SerializeField] GameObject human;
    [SerializeField] GameObject botanical;
    public AudioClip startSe;
    bool start = false;
    float nextTime;
    private AsyncOperation async;
    GameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadStart());
        nextTime = Time.time;
        if (gameMaster.clearHuman)
        {
            audioSourceBGM.enabled = false;
            audioSourceHumanBGM.enabled = true;
            skelton.SetActive(false);
            human.SetActive(true);
            botanical.SetActive(false);
        }
        else if (gameMaster.clearInf)
        {
            audioSourceBGM.enabled = false;
            audioSourceHumanBGM.enabled = true;
            skelton.SetActive(false);
            human.SetActive(false);
            botanical.SetActive(true);
        }
        else 
        {
            audioSourceBGM.enabled = true;
            audioSourceHumanBGM.enabled = false;
            skelton.SetActive(true);
            human.SetActive(false);
            botanical.SetActive(false);
        }
    }

    IEnumerator LoadStart()
    {
        yield return new WaitForSeconds(loadStartTimer);
        async = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        async.allowSceneActivation = false;
    }

    IEnumerator TitleSETimer()
    {
        if (!start)
        {
            start = true;
            audioSourceBGM.enabled = false;
            audioSourceSe.PlayOneShot(startSe);
            yield return new WaitForSeconds(seInterval);
            async.allowSceneActivation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            panel.color = Color.Lerp(panel.color, Color.black, Time.deltaTime / 1.5f);
            if (Time.time > nextTime)
            {
                underText.enabled = !underText.enabled;
                nextTime += interval/2;
            }
        }
        else
        {
            if (Time.time > nextTime)
            {
                underText.enabled = !underText.enabled;
                nextTime += interval;
            }
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(TitleSETimer());
        }
    }
}
