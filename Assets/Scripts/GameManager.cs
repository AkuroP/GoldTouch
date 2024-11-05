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

    private int freeTurns = 0;
    private bool isEvolutionMode = false;



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
        LoadStarsPerLevel();
    }

    public void ResetComboTimer() => _comboTimer = _comboMaxTimer;
    

    void Update()
    {
        textActualScore.text =  actualScore.ToString();
        scoreFinal.text =  actualScore.ToString();


        Debug.Log(nbPlay);
        Debug.Log(freeTurns);

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
    public void ActivateFreeTurns(int freeTurnCount)
    {
        freeTurns = freeTurnCount;
        Debug.Log("feee");

    }

    public void IncrementTurn()
    {
        if (freeTurns > 0)
        {
            freeTurns--;
        }
        else
        {
            nbPlay++;
        }
    }

    // Fonctionnalité du deuxième bouton : Fusionner les gemmes identiques
    public void MergeIdenticalGems()
    {
        // Récupère toutes les gemmes présentes dans la scène
        GemsFusion[] activeGems = FindObjectsOfType<GemsFusion>();

        // Si aucune gemme n'est trouvée, on arrête la méthode
        if (activeGems.Length == 0)
        {
            Debug.LogWarning("Aucune gemme active trouvée dans la scène !");
            return;
        }

        Dictionary<int, List<GameObject>> gemGroups = new Dictionary<int, List<GameObject>>();

        // Récupérer la position de spawn depuis le GemsManager
        Vector3 spawnPosition = _gemsManager._spawnPoint.position;

        // Ajout des gemmes dans des groupes basés sur leur type
        foreach (var gemFusion in activeGems)
        {
            // Ignore la gemme si elle est à la même position que _spawnPoint
            if (gemFusion.transform.position == spawnPosition)
            {
                continue;
            }

            int gemTypeIndex = gemFusion.gemsIndex;
            if (!gemGroups.ContainsKey(gemTypeIndex))
                gemGroups[gemTypeIndex] = new List<GameObject>();

            gemGroups[gemTypeIndex].Add(gemFusion.gameObject);
        }

        // Fusion des gemmes par groupes de 2 dans chaque groupe de type de gemme
        foreach (var group in gemGroups.Values)
        {
            for (int i = 0; i < group.Count - 1; i += 2) // Boucle par étapes de 2
            {
                GameObject gem1 = group[i];
                GameObject gem2 = group[i + 1];

                // Vérifie si les gemmes sont nulles (au cas où une gemme aurait été détruite précédemment)
                if (gem1 == null || gem2 == null) continue;

                // Appel de FusionnerGroupe avec une liste de 2 gemmes
                _gemsManager.FusionnerGroupe(new List<GameObject> { gem1, gem2 });
            }
        }
    }

    private void SaveStarsPerLevel()
    {
        // Conversion des étoiles en chaîne et sauvegarde
        string starsString = string.Join(",", SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel);
        PlayerPrefs.SetString("StarsPerLevel_" + SettingSystem.instance.levelNumber, starsString);
        PlayerPrefs.Save();
        Debug.Log("Étoiles sauvegardées pour le niveau " + SettingSystem.instance.levelNumber + ": " + starsString);
    }

    private void LoadStarsPerLevel()
    {
        // Chargement des étoiles à partir de PlayerPrefs
        if (PlayerPrefs.HasKey("StarsPerLevel_" + SettingSystem.instance.levelNumber))
        {
            string starsString = PlayerPrefs.GetString("StarsPerLevel_" + SettingSystem.instance.levelNumber);
            string[] starsArray = starsString.Split(',');
            for (int i = 0; i < starsArray.Length; i++)
            {
                if (int.TryParse(starsArray[i], out int star))
                {
                    SettingSystem.instance.donnees[i].starsPerLevel = star;
                }
            }
            Debug.Log("Étoiles chargées pour le niveau " + SettingSystem.instance.levelNumber + ": " + starsString);
        }
    }

   
}
