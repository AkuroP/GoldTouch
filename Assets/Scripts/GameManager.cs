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
    public TextMeshProUGUI scoreFinal;


    [SerializeField] private GameObject[] scoreStarsUI;

    private bool win;

    public bool Win => win;


    public GameObject _fx;

    public static GameManager instance;

    public int _combo;

    public bool _inCombo = false;
    public float _comboMaxTimer = 1f;
    private float _comboTimer;

    public TextMeshProUGUI _comboGO;

    [SerializeField]
    private GameObject[] _messages;

    [SerializeField]
    private bool _canPlay = true;
    public bool CanPlay {  get { return _canPlay; } set { _canPlay = value; } }

    [SerializeField]
    private GemsManager _gemsManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    private void Start()
    {
        textScoreToBeat.text = "/ " + scoreToBeat;
        ResetComboTimer();
    }

    public void ResetComboTimer() => _comboTimer = _comboMaxTimer;
    

    void Update()
    {
        textActualScore.text =  actualScore.ToString();
        scoreFinal.text =  actualScore.ToString();


        if (_inCombo)
        {
            if (_comboTimer > 0) _comboTimer -= Time.deltaTime;
            else
            {
                _canPlay = false;
                if (_combo < 2)
                {
                    _canPlay = true;
                    _gemsManager.NextGem();
                }
                else if (_combo >= 2 && _combo < 5) Instantiate(_messages[0], Vector3.zero, Quaternion.identity);
                else if(_combo >= 5) Instantiate(_messages[1], Vector3.zero, Quaternion.identity);
                _combo = 0;
                _inCombo = false;
            }

        }

        if (scoreToBeat <= -1) return;
        if(actualScore >= scoreToBeat)
        {
            EndGame();
            
        }
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
                if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1] && !SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2])
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
                if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0] && !SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1]&& !SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2])
                {
                        starsToAdd = 1;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= 1;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0] = true;
                    Debug.Log(SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0]);

                    return starsToAdd;
                }
                else
                {
                    Debug.Log("1 stars");
                    Debug.Log(SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0]);

                    return starsToAdd;
                }
            }

        }
        return starsToAdd;
        
    }

    public void EndGame()
    {
        if (!win)
        {
            win = true;
            Debug.Log("You win");
            winScreen.SetActive(true);
            AudioManager.instance.PlayRandom(SoundState.VICTORY);
            if (nbPlay < countForStars[0])
            {
                scoreStarsUI[2].SetActive(true);
            }

            else if (nbPlay < countForStars[1] && nbPlay > countForStars[0])
            {
                scoreStarsUI[1].SetActive(true);
            }
            else
            {
                scoreStarsUI[0].SetActive(true);
            }
            SettingSystem.instance.nbStars += StarsIncrementation();
        }
    }

}
