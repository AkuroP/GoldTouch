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
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            PlayerPrefs.SetInt($"Level_{i}_Unlocked", starsSpritesData[i].isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt($"Level_{i}_AnimationTriggered", starsSpritesData[i].animationTriggered ? 1 : 0);
        }
        PlayerPrefs.Save(); // Ne pas oublier de sauvegarder
    }

    private void LoadData()
    {
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            starsSpritesData[i].isUnlocked = PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1;
            starsSpritesData[i].animationTriggered = PlayerPrefs.GetInt($"Level_{i}_AnimationTriggered", 0) == 1;
        }
    }
}
