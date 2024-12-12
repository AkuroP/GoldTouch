using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using Unity.Services.RemoteConfig;





public class GameManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener

{
    
    // ADS
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;  

    bool isAdsPlaying = false;
    
    
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
    public TextMeshProUGUI[] textActualNbPlay;

    public TextMeshProUGUI scoreFinal;
    public TextMeshProUGUI[] scoreForStars;


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


    [HideInInspector]public int freeTurns = 0;
    //private bool isEvolutionMode = false;

    [Header("Bonus Text/Nombre")]
    [SerializeField] private TextMeshProUGUI textFreeTurnBonus;
    [SerializeField] private TextMeshProUGUI textAutoMergeBonus;


    public bool isEndless = false;


    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //Ads
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

    }

    private void Start()
    {
        textScoreToBeat.text = "/ " + scoreToBeat;
        ResetComboTimer();
        LoadStarsPerLevel();
        UpdateBonusTexts();

        // Vérifiez si les publicités sont désactivées
        if (AreAdsDisabled())
        {
            Debug.Log("Ads have been disabled.");
        }
        else
        {
            LoadAd(); // Charger les publicités uniquement si elles ne sont pas désactivées
        }
        if (!isEndless)
        {
            scoreForStars[0].text = countForStars[0].ToString();
            scoreForStars[1].text = countForStars[1].ToString();
            //scoreForStars[2].text = countForStars[1].ToString();
        }

    }



    public void ResetComboTimer() => _comboTimer = _comboMaxTimer;


    private void Update()
    {
        textActualScore.text = actualScore.ToString();
        for(int i = 0; i < textActualNbPlay.Length; i++)
        {
            textActualNbPlay[i].text = nbPlay.ToString();
        }
        scoreFinal.text = actualScore.ToString();

        UpdateBonusTexts();

        if (_inCombo)
        {
            if (_comboTimer > 0) _comboTimer -= Time.deltaTime;
            else
            {
                HandleComboEnd();
            }
        }

        
        if (scoreToBeat > 0 && actualScore >= scoreToBeat && !isAdsPlaying)
        {

            // Verifier et modifier le highscore
            if (actualScore > PlayerPrefs.GetInt("Highscore"))
            {
                PlayerPrefs.SetInt("Highscore", actualScore);
            }

            int randomInt = UnityEngine.Random.Range(0, 9);
            Debug.Log("si > 5 pub : " + randomInt);

            if (!isEndless)
            {
                Debug.Log("Level finished");
                if (randomInt > RemoteConfigService.Instance.appConfig.GetInt("abtest_percentage_interstitialads") )
                {
                    ShowAd(); // Lance la pub si le nombre est compris entre 0 et 2
                    isAdsPlaying = true;
                     Debug.Log("Pub =" + isAdsPlaying);
                     Debug.Log("abtest_percentage_interstitialads = " + RemoteConfigService.Instance.appConfig.GetInt("abtest_percentage_interstitialads"));

                }

                else
                {
                     
                    isAdsPlaying = true;

                    Debug.Log("Pub =" + isAdsPlaying);

                    EndGame();

                }
            }            
            
        }

        Debug.Log(SettingSystem.instance.donnees[0].starsPerLevel);
    }
    private void HandleComboEnd()
    {
        _canPlay = false;
        if (_combo < 2)
        {
            _canPlay = true;
            _gemsManager.NextGem();
        }
        else if (_combo >= 2 && _combo < 5) Instantiate(_messages[0], Vector3.zero, Quaternion.identity);
        else if (_combo >= 5) Instantiate(_messages[1], Vector3.zero, Quaternion.identity);
        _combo = 0;
        _inCombo = false;
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
                    if (SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel == 3)
                    {
                        starsToAdd = 3;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= starsToAdd;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2] = true;
                        SaveBoolValues();
                        return starsToAdd;
                    }
                    else
                    {
                        starsToAdd = SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel = 0;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2] = true;
                        SaveBoolValues();
                        return starsToAdd;
                    }

                }
                else
                {
                    SaveBoolValues();
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
                        SaveBoolValues();
                        return starsToAdd;
                    }
                    else
                    {
                        starsToAdd = 2;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= starsToAdd;
                        SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1] = true;
                        SaveBoolValues();
                        return starsToAdd;
                    }

                }
                else
                {
                    SaveBoolValues();
                    return starsToAdd;
                }

            }
            else
            {
                if (!SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0] && !SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[1] && !SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[2])
                {
                    starsToAdd = 1;
                    SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel -= 1;
                    SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0] = true;
                    Debug.Log(SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0]);
                    SaveBoolValues();
                    return starsToAdd;
                }
                else
                {
                    Debug.Log("1 stars");
                    Debug.Log(SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].bools[0]);
                    SaveBoolValues();
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
            if (!isEndless)
            {
                UpdateStarUI();

            }
            else
            {
                UpdateStarUIEndless();
            }
            SettingSystem.instance.nbStars += StarsIncrementation();
            SaveStarsPerLevel();

            isAdsPlaying = false;
        }
    }

    private void UpdateStarUI()
    {
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
    }

    private void UpdateStarUIEndless()
    {
        scoreStarsUI[3].SetActive(true);

    }
    public void ActivateFreeTurns(int freeTurnCount)
    {
        freeTurns = freeTurnCount;
        Debug.Log("free");

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
        // Sauvegarde les étoiles pour le niveau actuel
        PlayerPrefs.SetInt("StarsPerLevel_" + SettingSystem.instance.levelNumber, SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel);
        PlayerPrefs.Save();
    }

    private void LoadStarsPerLevel()
    {
        // Charge les étoiles pour le niveau actuel
        if (PlayerPrefs.HasKey("StarsPerLevel_" + SettingSystem.instance.levelNumber))
        {
            int stars = PlayerPrefs.GetInt("StarsPerLevel_" + SettingSystem.instance.levelNumber);
            SettingSystem.instance.donnees[SettingSystem.instance.levelNumber].starsPerLevel = stars;
        }
    }




    private void UpdateBonusTexts()
    {
        if (SettingSystem.instance != null)
        {
            textFreeTurnBonus.text = SettingSystem.instance.nbFreeTurneBonus.ToString();
            textAutoMergeBonus.text = SettingSystem.instance.nbAutoMergeBonus.ToString();
        }
    }

    private void SaveBoolValues()
    {
        // Par exemple pour levelNumber 2 et bools[0], bools[1], bools[2]
        int level = SettingSystem.instance.levelNumber;

        for (int i = 0; i < SettingSystem.instance.donnees[level].bools.Length; i++)
        {
            // Utilise une clé unique pour chaque booléen : "Level_<levelNumber>_Bool_<index>"
            string key = $"Level_{level}_Bool_{i}";
            PlayerPrefs.SetInt(key, SettingSystem.instance.donnees[level].bools[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }


    //ADS

        // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

        // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        if (AreAdsDisabled())
        {
            Debug.Log("Ads are disabled. Skipping ad display.");
            EndGame(); // Continue le flux de jeu sans montrer la publicité
            return;
        }

        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
    }

        // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        EndGame();
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        EndGame();
    }
 
    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) {EndGame();}

    private bool AreAdsDisabled()
    {
        return PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
    }
    

}
