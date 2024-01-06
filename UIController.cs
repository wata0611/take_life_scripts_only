using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject[] uiMeat;
    [SerializeField] GameObject[] uiLife;
    int tmpMeatCount;
    int tmpLife;

    // Start is called before the first frame update
    void Start()
    {
        InitializeUIMeat();
        InitializeUILife();
    }

    // Update is called once per frame
    void Update()
    {
        UIMeatController();
        UILifeController();
    }

    void InitializeUIMeat()
    {
        tmpMeatCount = player.MeatCount;
        for (int i = 0; i < uiMeat.Length; i++)
            uiMeat[i].SetActive(false);
    }

    void InitializeUILife()
    {
        tmpLife = player.Life;
    }

    void UIMeatController()
    {
        if (tmpMeatCount != player.MeatCount)
        {
            for (int i = 1; i <= player.MeatCount; i++)
                uiMeat[i-1].SetActive(true);
            for (int i = player.MeatCount + 1; i <= player.GetMaxMeatCount(); i++)
                uiMeat[i - 1].SetActive(false);
            tmpMeatCount = player.MeatCount;
        }
    }

    void UILifeController()
    {
        if (tmpLife != player.Life)
        {
            for (int i = 1; i <= player.Life; i++)
                uiLife[i - 1].SetActive(true);
            for (int i = player.Life + 1; i <= player.GetMaxLife(); i++)
                uiLife[i - 1].SetActive(false);
            tmpLife = player.Life;
        }
    }

}
