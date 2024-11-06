using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // Permet � Unity de s�rialiser cette classe et d'afficher ses champs dans l'inspecteur
public class Datata
{
    public GameObject[] starsSprites;
    public bool isUnlocked; // Indique si le niveau est d�bloqu�
    public bool animationTriggered; // Indique si l'animation a �t� lanc�e

    public Datata(int size)
    {
        starsSprites = new GameObject[3];
        isUnlocked = false; // Initialisation par d�faut
        animationTriggered = false; // Initialisation par d�faut
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
        LoadData(); // Charger les donn�es au d�marrage
    }

    private void Update()
    {
        UpdateUI();
        SaveData(); // Sauvegarder les donn�es � chaque frame (ou vous pouvez le faire � un moment sp�cifique)
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
        // Sauvegarder les donn�es de chaque niveau
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            // Sauvegarde de l'�tat du niveau (d�bloqu� ou non)
            PlayerPrefs.SetInt($"Level_{i}_Unlocked", starsSpritesData[i].isUnlocked ? 1 : 0);
            // Sauvegarde de l'�tat de l'animation (d�clench�e ou non)
            PlayerPrefs.SetInt($"Level_{i}_AnimationTriggered", starsSpritesData[i].animationTriggered ? 1 : 0);

            // Sauvegarde des �toiles de chaque niveau (actives ou non)
            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                // Enregistrer chaque �toile (active ou non) pour chaque niveau
                PlayerPrefs.SetInt($"Level_{i}_Star_{j}_Active", starsSpritesData[i].starsSprites[j].activeSelf ? 1 : 0);
            }
        }
        PlayerPrefs.Save(); // Sauvegarder les modifications
    }

    private void LoadData()
    {
        // Charger les donn�es pour chaque niveau
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            // Charger l'�tat du niveau (d�bloqu� ou non)
            starsSpritesData[i].isUnlocked = PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1;
            // Charger l'�tat de l'animation (d�clench�e ou non)
            starsSpritesData[i].animationTriggered = PlayerPrefs.GetInt($"Level_{i}_AnimationTriggered", 0) == 1;

            // Charger l'�tat des �toiles pour chaque niveau
            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                bool isActive = PlayerPrefs.GetInt($"Level_{i}_Star_{j}_Active", 0) == 1;
                starsSpritesData[i].starsSprites[j].SetActive(isActive); // Restaurer l'�tat actif de chaque �toile
            }
        }
    }
}
