using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allGems;

    public GameObject[] AllGems => _allGems;


    [SerializeField] private int scoreToBeat;
    [SerializeField] private GameObject winScreen;

    [HideInInspector]public int actualScore;
    [HideInInspector]public int nbPlay;


    [SerializeField] private int[] countForStars = new int[2];
    public TextMeshProUGUI textScoreToBeat;
    public TextMeshProUGUI textActualScore;

    bool win;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    private void Start()
    {
        textScoreToBeat.text = "Score To Beat : " + scoreToBeat.ToString();
    }


    void Update()
    {
        if(actualScore > scoreToBeat)
        {
            if (!win)
            {
                win = true;
                Debug.Log("You win");
                winScreen.SetActive(true);
                StarsIncrementation();
                SettingSystem.instance.nbStars += StarsIncrementation();
            }
            
        }
        textActualScore.text = "My score : " + actualScore.ToString();
        
    }


    public int StarsIncrementation()
    {
        int starsToAdd = 0;
        if (nbPlay < countForStars[0])
        {
            Debug.Log("3 stars");
            starsToAdd = 3;
            return starsToAdd;

        }
        else if (nbPlay < countForStars[1] && nbPlay > countForStars[0])
        {
            Debug.Log("2 stars");
            starsToAdd = 2;
            return starsToAdd;

        }
        else
        {
            Debug.Log("1 star");
            starsToAdd = 1;
            return starsToAdd;
        }
    }
}
