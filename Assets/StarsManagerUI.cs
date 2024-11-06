using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // Permet à Unity de sérialiser cette classe et d'afficher ses champs dans l'inspecteur
public class Datata
{
    public GameObject[] starsSprites;
    public bool isUnlocked; // Indique si le niveau est débloqué
    public bool animationTriggered; // Indique si l'animation a été lancée

    public Datata(int size)
    {
        starsSprites = new GameObject[3];
        isUnlocked = false; // Initialisation par défaut
        animationTriggered = false; // Initialisation par défaut
    }
}
public class StarsManagerUI : MonoBehaviour
{
    public Datata[] starsSpritesData;

    [SerializeField] private GameObject[] starsLocked;

    [SerializeField] private Animator animator2;
    [SerializeField] private Animator animator3;

    private void Start()
    {
        LoadData(); // Charger les données au démarrage
    }

    private void Update()
    {
        UpdateUI();
        SaveData(); // Sauvegarder les données à chaque frame (ou vous pouvez le faire à un moment spécifique)
    }

    private void UpdateUI()
    {
        if (SettingSystem.instance.nbStars >= 2)
        {
            animator2.SetBool("CanDance2", true);
            starsLocked[0].SetActive(false);
        }
        if (SettingSystem.instance.nbStars >= 4)
        {
            animator3.SetBool("canDance3", true);
            starsLocked[1].SetActive(false);
        }

        for (int levelIndex = 0; levelIndex < starsSpritesData.Length; levelIndex++)
        {
            for (int starIndex = 0; starIndex < starsSpritesData[levelIndex].starsSprites.Length; starIndex++)
            {
                starsSpritesData[levelIndex].starsSprites[starIndex].SetActive(SettingSystem.instance.donnees[levelIndex].bools[starIndex]);
            }
        }
    }

    private void SaveData()
    {
        // Sauvegarder les données de chaque niveau
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            // Sauvegarde de l'état du niveau (débloqué ou non)
            PlayerPrefs.SetInt($"Level_{i}_Unlocked", starsSpritesData[i].isUnlocked ? 1 : 0);
            // Sauvegarde de l'état de l'animation (déclenchée ou non)
            PlayerPrefs.SetInt($"Level_{i}_AnimationTriggered", starsSpritesData[i].animationTriggered ? 1 : 0);

            // Sauvegarde des étoiles de chaque niveau (actives ou non)
            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                // Enregistrer chaque étoile (active ou non) pour chaque niveau
                PlayerPrefs.SetInt($"Level_{i}_Star_{j}_Active", starsSpritesData[i].starsSprites[j].activeSelf ? 1 : 0);
            }
        }
        PlayerPrefs.Save(); // Sauvegarder les modifications
    }

    private void LoadData()
    {
        // Charger les données pour chaque niveau
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            // Charger l'état du niveau (débloqué ou non)
            starsSpritesData[i].isUnlocked = PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1;
            // Charger l'état de l'animation (déclenchée ou non)
            starsSpritesData[i].animationTriggered = PlayerPrefs.GetInt($"Level_{i}_AnimationTriggered", 0) == 1;

            // Charger l'état des étoiles pour chaque niveau
            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                bool isActive = PlayerPrefs.GetInt($"Level_{i}_Star_{j}_Active", 0) == 1;
                starsSpritesData[i].starsSprites[j].SetActive(isActive); // Restaurer l'état actif de chaque étoile
            }
        }
    }
}
