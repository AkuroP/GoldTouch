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

    private bool win;

    public bool Win => win;


    public GameObject _fx;

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
        textScoreToBeat.text = "Money to mine :\n" + scoreToBeat.ToString() + " ♦";
    }


    void Update()
    {
        if(actualScore >= scoreToBeat)
        {
            if (!win)
            {
                win = true;
                Debug.Log("You win");
                winScreen.SetActive(true);
                SettingSystem.instance.nbStars += StarsIncrementation();
            }
            
        }
        textActualScore.text = "Your money :\n" + actualScore + " ♦";   
    }

    public IEnumerator IncreaseScore(int score)
    {
        for (int i = 0; i < score; i++)
        {
            yield return new WaitForSeconds(.03f);
            actualScore += 1;
        }

    }

    public int StarsIncrementation()
    {
        int starsToAdd = 0;
        if (SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel != 0)
        {
            if (nbPlay < countForStars[0])
            {
                if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2]) 
                {
                    Debug.Log("3 stars");
                    if(SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel == 3)
                    {
                        starsToAdd = 3;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= starsToAdd;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2] = true;
                        return starsToAdd;
                    }
                    else
                    {
                        starsToAdd = SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel = 0;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2] = true;
                        return starsToAdd;
                    }
                    
                }
                else
                {
                    return starsToAdd;
                }
                

            }
            else if (nbPlay < countForStars[1] && nbPlay > countForStars[0])
            {
                if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1])
                {
                    Debug.Log("2 stars");
                    if (SettingSystem.instance.donnees[0].bools[0])
                    {
                        starsToAdd = 1;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= starsToAdd;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1] = true;
                        return starsToAdd;
                    }
                    else
                    {
                        starsToAdd = 2;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= starsToAdd;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1] = true;
                        return starsToAdd;
                    }
                    
                }
                else
                {
                    return starsToAdd;
                }

            }
            else
            {
                if (!SettingSystem.instance.donnees[0].bools[0])
                {
                    Debug.Log("1 stars");
                    if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1])
                    {
                        Debug.Log("1 stars");
                        starsToAdd = 1;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= 1;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0] = true;
                        return starsToAdd;

                    }
                    
                }
                else
                {
                    Debug.Log("1 stars");
                    return starsToAdd;
                }
            }

        }
        return starsToAdd;
        
    }

}
