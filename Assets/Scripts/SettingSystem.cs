using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;



[System.Serializable] // Permet à Unity de sérialiser cette classe et d'afficher ses champs dans l'inspecteur
public class Data
{
    public int starsPerLevel; // Tableau d'entiers
    public bool[] bools; // Tableau de booléens

    // Constructeur pour initialiser les tableaux avec la taille spécifiée
    public Data(int size)
    {

        bools = new bool[3]; // Crée un tableau de 3 booléens pour chaque entier
    }
}

[System.Serializable]
public class Bonus
{
    public string bonusName; // Nom du bonus
    public int minAmount; // Montant minimum
    public int maxAmount; // Montant maximum

    public Bonus(string name, int min, int max)
    {
        bonusName = name;
        minAmount = min;
        maxAmount = max;
    }
}

public enum CofferType
{
    Standard,
    Rare,
    Legendary
}
public class SettingSystem : MonoBehaviour
{

    [SerializeField] private Animator animatorsetting;
    [SerializeField] private Animator animatorShop;


    [SerializeField] private TextMeshProUGUI freeCofferTimerText; // Texte pour afficher le temps restant
    private const string FreeCofferLastOpenKey = "FreeCofferLastOpenTime";
    private System.TimeSpan freeCofferCooldown = System.TimeSpan.FromHours(24);


    [SerializeField] private AudioMixer audioMixer;

    [Scene]
    public string sceneToKeepObjects;

    [Scene]
    public string showStars;


    private bool animationForward = false;

    bool isVibration = true;


    private string exposedParameterName = "SFX";

    private float sfxBaseVolume;

    public int nbStars;
    public TextMeshProUGUI nbStarsText;
    [SerializeField] private GameObject starsVisuel;
    [SerializeField] private GameObject homeButton;


    public Data[] donnees;


    public int levelNumber;

    public static SettingSystem instance;

    [SerializeField] private TextMeshProUGUI goldText;

    private int goldHardCurrency;

    private List<Bonus> bonuses;

    public int nbFreeTurneBonus;
    public int nbAutoMergeBonus;

    [SerializeField] private Animator animatorCoffer;

    [SerializeField] private GameObject[] closeCofferButton;

    [SerializeField] private TextMeshProUGUI freeLunchBonusText;
    [SerializeField] private TextMeshProUGUI mergeBonusText;

    [SerializeField] private Image freeLunchBonusImage;
    [SerializeField] private Image mergeBonusImage;

    private int lastFreeLunchBonus; // Stock temporaire pour le bonus Free Lunch gagné
    private int lastMergeBonus;

    [SerializeField] private string uiElementNameToDisable;
    private GameObject cachedUIElement;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }




    private void Start()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer non défini dans le script MixerVolumeGetter !");
            return;
        }
        DontDestroyOnLoad(gameObject);

        sfxBaseVolume = GetMixerVolume();
        LoadBoolValues();
        InitializeGold();
        LoadStars();
        LoadBonuses();

        UpdateStarsText();
        UpdateGoldText();
        InitializeBonuses();
        UpdateFreeCofferUI();

    }

    private void OnApplicationQuit()
    {
        Save();
        SaveGold();
        SaveBonuses();
    }

    private void LoadBoolValues()
    {
        for (int level = 0; level < donnees.Length; level++)
        {

            for (int i = 0; i < donnees[level].bools.Length; i++)
            {
                string key = $"Level_{level}_Bool_{i}";

                if (PlayerPrefs.HasKey(key))
                {
                    donnees[level].bools[i] = PlayerPrefs.GetInt(key) == 1;
                }
            }
            Debug.Log("Booléens chargés pour le niveau : " + level);
        }

    }

    public void Save()
    {
        PlayerPrefs.SetInt("NbStars", nbStars);
        PlayerPrefs.Save();

    }

    public void LoadStars()
    {
        if (PlayerPrefs.HasKey("NbStars"))
        {

            nbStars = PlayerPrefs.GetInt("NbStars");
            Debug.Log("Nombre d'étoiles chargé: " + nbStars);
        }
        else
        {
            Debug.Log("Aucune donnée d'étoiles trouvée, utilisant le nombre par défaut.");
            nbStars = 0;
        }
    }

    public void SaveBonuses()
    {
        PlayerPrefs.SetInt("nbFreeTurneBonus", nbFreeTurneBonus);
        PlayerPrefs.SetInt("nbAutoMergeBonus", nbAutoMergeBonus);
        PlayerPrefs.Save();
    }

    // Méthode pour charger les valeurs des bonus
    public void LoadBonuses()
    {
        // Charger les bonus si les clés existent dans PlayerPrefs
        nbFreeTurneBonus = PlayerPrefs.HasKey("nbFreeTurneBonus") ? PlayerPrefs.GetInt("nbFreeTurneBonus") : 0;
        nbAutoMergeBonus = PlayerPrefs.HasKey("nbAutoMergeBonus") ? PlayerPrefs.GetInt("nbAutoMergeBonus") : 0;

        Debug.Log("Bonuses chargés: nbFreeTurneBonus = " + nbFreeTurneBonus + ", nbAutoMergeBonus = " + nbAutoMergeBonus);
    }

    private void UpdateStarsText()
    {
        nbStarsText.text = nbStars.ToString();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != sceneToKeepObjects)
        {
            DisableObjects();
        }
        else
        {
            EnableObjects();
        }

        if (SceneManager.GetActiveScene().name == showStars)
        {
            ShowTotalStars();
        }
        else
        {
            UnShowTotalStars();
        }


        nbStarsText.text = nbStars.ToString();
        UpdateGoldText();
        UpdateFreeCofferUI();
    }


    float GetMixerVolume()
    {
        float volume = 0f;
        bool result = audioMixer.GetFloat(exposedParameterName, out volume);
        if (!result)
        {
            Debug.LogError("Impossible d'obtenir le volume de l'AudioMixer pour le paramètre exposé: " + exposedParameterName);
        }
        return volume;
    }


    public void SystemMenu()
    {
        if (animationForward)
        {
            animatorsetting.SetBool("IsOn", false);
            animationForward = false;
        }
        else
        {
            animatorsetting.SetBool("IsOn", true);
            animationForward = true;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
        AudioManager.instance.PlayRandom(SoundState.BACKBUTTON);
    }
    public void BackHomeMenu()
    {
        SceneManager.LoadScene("LevelSelection");
        AudioManager.instance.PlayRandom(SoundState.BACKBUTTON);
    }
    public void Settings()
    {
        AudioManager.instance.PlayRandom(SoundState.SETTINGS);
    }

    public void Shop()
    {
        AudioManager.instance.PlayRandom(SoundState.BUTTON);
        animatorShop.SetBool("IsShop", true);

        CacheAndDisableUIElement(uiElementNameToDisable);
    }
    public void ShopBack()
    {
        AudioManager.instance.PlayRandom(SoundState.BUTTON);
        animatorShop.SetBool("IsShop", false);
        ReactivateCachedUIElement();
        Debug.Log("je evien du shop l'ami");
    }

    public void EnableVibration()
    {
        // Activer les vibrations du téléphone
        Handheld.Vibrate();
        isVibration = true;
        AudioManager.instance.PlayRandom(SoundState.BUTTON);

        Debug.Log("Vibration activée");
    }

    public void Vibrate()
    {
        if (isVibration) Handheld.Vibrate();
    }

    // Fonction pour désactiver les vibrations
    public void DisableVibration()
    {
        // Désactiver les vibrations du téléphone
        isVibration = false;
        AudioManager.instance.PlayRandom(SoundState.BACKBUTTON);
        Debug.Log("Vibration désactivée");
    }

    public void EnableMusic()
    {
        AudioManager.instance.Unpaused(SoundState.MENU);
        AudioManager.instance.PlayRandom(SoundState.BUTTON);
    }

    public void DisableMusic()
    {
        AudioManager.instance.Paused(SoundState.MENU);
        AudioManager.instance.PlayRandom(SoundState.BACKBUTTON);


    }
    public void EnableSFX()
    {
        audioMixer.SetFloat(exposedParameterName, sfxBaseVolume);
        AudioManager.instance.PlayRandom(SoundState.BUTTON);
    }

    public void DisableSFX()
    {
        audioMixer.SetFloat(exposedParameterName, -80f);
        AudioManager.instance.PlayRandom(SoundState.BACKBUTTON);

    }



    void DisableObjects()
    {
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("ObjectsToDisable");
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }
    void EnableObjects()
    {
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("ObjectsToDisable");
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
        }
    }


    void ShowTotalStars()
    {
        starsVisuel.SetActive(true);

    }
    void UnShowTotalStars()
    {
        starsVisuel.SetActive(false);

    }


    private void InitializeGold()
    {
        // Charger la valeur de PlayerPrefs si elle existe, sinon initialiser avec une valeur par défaut (ex. : 100)
        goldHardCurrency = PlayerPrefs.GetInt("GoldHardCurrency", 500); // Valeur par défaut 100
    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt("GoldHardCurrency", goldHardCurrency);
        PlayerPrefs.Save();
    }

    // Méthode pour mettre à jour l'UI du texte de la monnaie
    public void UpdateGoldText()
    {
        // Supposons que vous ayez un TextMeshProUGUI nommé goldText
        goldText.text = goldHardCurrency.ToString();
    }

    public void AddGold(int amount)
    {
        goldHardCurrency += amount;
        SaveGold();
        UpdateGoldText();
    }

    public bool SpendGold(int amount)
    {
        if (goldHardCurrency >= amount)
        {
            goldHardCurrency -= amount;
            SaveGold();
            UpdateGoldText();
            return true;
        }
        else
        {
            Debug.Log("Pas assez de monnaie !");
            return false;
        }
    }

    public void OnCofferButtonPress(int cofferTypeIndex)
    {
        // Convertir l'index en type de coffre
        CofferType cofferType = (CofferType)cofferTypeIndex;

        int cofferPrice = GetCofferPrice(cofferType);

        if (goldHardCurrency >= cofferPrice)
        {
            Debug.Log("Coffre acheté !");
            goldHardCurrency -= cofferPrice;
            GenerateRandomBonuses(cofferType);
            SaveGold();
            UpdateGoldText();
            PlayCofferAnimation(cofferType);
        }
        else
        {
            Debug.Log("Pas assez de gold pour acheter le coffre !");
        }
    }
    private int GetCofferPrice(CofferType cofferType)
    {
        switch (cofferType)
        {
            case CofferType.Rare:
                return 750;
            case CofferType.Legendary:
                return 2000;
            default:
                return 250;
        }
    }
    private void PlayCofferAnimation(CofferType cofferType)
    {
        UpdateBonusTexts();

        switch (cofferType)
        {
            case CofferType.Standard:
                animatorCoffer.SetBool("IsOpen1", true);
                closeCofferButton[0].SetActive(true);
                break;
            case CofferType.Rare:
                animatorCoffer.SetBool("IsOpen2", true);
                closeCofferButton[1].SetActive(true);
                break;
            case CofferType.Legendary:
                animatorCoffer.SetBool("IsOpen3", true);
                closeCofferButton[2].SetActive(true);
                break;
        }
    }

    public void CloseCofferAnimation(int cofferTypeIndex)
    {
        freeLunchBonusText.gameObject.SetActive(false);
        mergeBonusText.gameObject.SetActive(false);
        freeLunchBonusImage.gameObject.SetActive(false);
        mergeBonusImage.gameObject.SetActive(false);

        CofferType cofferType = (CofferType)cofferTypeIndex;

        switch (cofferType)
        {
            case CofferType.Standard:
                animatorCoffer.SetBool("IsOpen1", false);
                closeCofferButton[0].SetActive(false);
                break;
            case CofferType.Rare:
                animatorCoffer.SetBool("IsOpen2", false);
                closeCofferButton[1].SetActive(false);
                break;
            case CofferType.Legendary:
                animatorCoffer.SetBool("IsOpen3", false);
                closeCofferButton[2].SetActive(false);
                break;
        }
    }

    private void InitializeBonuses()
    {
        bonuses = new List<Bonus>
        {
            new Bonus("FreeLunch", 1, 1), // 1 à 3 exemplaires de Bonus1
            new Bonus("MergeBonus", 1, 1)  // 1 à 5 exemplaires de Bonus2
        };
    }

    private void GenerateRandomBonuses(CofferType cofferType)
    {
        int minBonusCount, maxBonusCount;

        // Définir le nombre de bonus selon le type de coffre
        switch (cofferType)
        {
            case CofferType.Rare:
                minBonusCount = 3;
                maxBonusCount = 6;
                break;
            case CofferType.Legendary:
                minBonusCount = 6;
                maxBonusCount = 12;
                break;
            default: // Standard
                minBonusCount = 1;
                maxBonusCount = 3;
                break;
        }

        // Calculer le nombre de bonus à générer
        int bonusCount = Random.Range(minBonusCount, maxBonusCount + 1);
        Debug.Log($"Type de coffre: {cofferType}, Nombre de bonus à générer: {bonusCount}");

        // Variables pour suivre le nombre total de chaque type de bonus
        int totalFreeLunchBonus = 0;
        int totalMergeBonus = 0;

        lastFreeLunchBonus = 0;
        lastMergeBonus = 0;

        // Générer chaque bonus
        for (int i = 0; i < bonusCount; i++)
        {
            // Sélectionner un bonus aléatoire
            Bonus selectedBonus = bonuses[Random.Range(0, bonuses.Count)];

            // Générer une quantité aléatoire pour ce bonus
            int amount = Random.Range(selectedBonus.minAmount, selectedBonus.maxAmount + 1);
            Debug.Log($"Bonus généré : {selectedBonus.bonusName}, Quantité : {amount}");

            // Ajouter la quantité au type de bonus correspondant
            if (selectedBonus.bonusName == "FreeLunch")
            {
                nbFreeTurneBonus += amount;
                totalFreeLunchBonus += amount;
                lastFreeLunchBonus += amount;
            }
            else if (selectedBonus.bonusName == "MergeBonus")
            {
                nbAutoMergeBonus += amount;
                totalMergeBonus += amount;
                lastMergeBonus += amount;
            }
        }

        // Log final pour le nombre total de chaque bonus
        Debug.Log($"Total FreeLunch Bonus : {totalFreeLunchBonus}");
        Debug.Log($"Total Merge Bonus : {totalMergeBonus}");

        SaveBonuses();
    }

    private void UpdateBonusTexts()
    {
        // Met à jour les textes en fonction des bonus obtenus
        if (lastFreeLunchBonus > 0)
        {
            freeLunchBonusText.text = $"X: {lastFreeLunchBonus}";
            freeLunchBonusText.gameObject.SetActive(true);
            freeLunchBonusImage.gameObject.SetActive(true);

        }
        else
        {
            freeLunchBonusText.gameObject.SetActive(false);
            freeLunchBonusImage.gameObject.SetActive(false);

        }

        if (lastMergeBonus > 0)
        {
            mergeBonusText.text = $"X: {lastMergeBonus}";
            mergeBonusText.gameObject.SetActive(true);
            mergeBonusImage.gameObject.SetActive(true);
        }
        else
        {
            mergeBonusText.gameObject.SetActive(false);
            mergeBonusImage.gameObject.SetActive(false);

        }
    }

    private void CacheAndDisableUIElement(string elementName)
    {
        // Si nous avons déjà une référence au GameObject, inutile de la chercher à nouveau
        if (cachedUIElement == null)
        {
            cachedUIElement = GameObject.Find(elementName);
        }

        if (cachedUIElement != null)
        {
            cachedUIElement.SetActive(false); // Désactive le GameObject UI
            Debug.Log($"UI élément '{elementName}' désactivé et mis en cache.");
        }
        else
        {
            Debug.LogWarning($"UI élément '{elementName}' non trouvé dans la scène.");
        }
    }

    private void ReactivateCachedUIElement()
    {
        if (cachedUIElement != null)
        {
            cachedUIElement.SetActive(true); // Réactive le GameObject UI
            Debug.Log($"UI élément '{cachedUIElement.name}' réactivé depuis le cache.");
        }
        else
        {
            Debug.LogWarning("Aucun élément UI n'a été mis en cache pour réactivation.");
        }
    }


    private DateTime GetLastFreeCofferOpenTime()
    {
        if (PlayerPrefs.HasKey(FreeCofferLastOpenKey))
        {
            string savedTime = PlayerPrefs.GetString(FreeCofferLastOpenKey);
            return DateTime.Parse(savedTime);
        }
        return DateTime.MinValue; // Pas encore ouvert
    }

    private void SetLastFreeCofferOpenTime(DateTime time)
    {
        PlayerPrefs.SetString(FreeCofferLastOpenKey, time.ToString());
        PlayerPrefs.Save();
    }

    private bool IsFreeCofferAvailable()
    {
        DateTime lastOpenTime = GetLastFreeCofferOpenTime();
        return DateTime.Now - lastOpenTime >= freeCofferCooldown;
    }

    private TimeSpan GetTimeUntilFreeCoffer()
    {
        DateTime lastOpenTime = GetLastFreeCofferOpenTime();
        return freeCofferCooldown - (DateTime.Now - lastOpenTime);
    }

    private void UpdateFreeCofferUI()
    {
        if (IsFreeCofferAvailable())
        {
            freeCofferTimerText.text = "Free chest Ready !";
        }
        else
        {
            TimeSpan timeLeft = GetTimeUntilFreeCoffer();
            if (timeLeft.TotalSeconds > 0)
            {
                freeCofferTimerText.text = string.Format("Next free chest in : {0:D2}:{1:D2}:{2:D2}",
                    timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
            }
        }
    }

    public void OpenFreeCoffer()
    {
        if (IsFreeCofferAvailable())
        {
            Debug.Log("Coffre gratuit ouvert !");
            SetLastFreeCofferOpenTime(DateTime.Now);

            // Générer les bonus du coffre gratuit (comme le coffre standard)
            GenerateRandomBonuses(CofferType.Standard);

            // Jouer l'animation d'ouverture
            PlayCofferAnimation(CofferType.Standard);
        }
        else
        {
            Debug.Log("Le coffre gratuit n'est pas encore prêt !");
        }
    }


}
