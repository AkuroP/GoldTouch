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

    [SerializeField] private List<GameObject> levelButtons;

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
        foreach (var button in levelButtons)
        {
            // Vérifier si le bouton a un composant SceneLoader
            SceneLoader sceneLoader = button.GetComponent<SceneLoader>();
            if (sceneLoader == null) continue;

            // Vérifier si le joueur a suffisamment d'étoiles pour activer ce bouton
            if (SettingSystem.instance.nbStars >= sceneLoader.nbStarsNeeded)
            {
                // Activer le bouton et jouer son animation
                //button.SetActive(true);

                // Lancer l'animation du bouton via son Animator
                Animator buttonAnimator = button.GetComponent<Animator>();
                if (buttonAnimator != null)
                {
                    buttonAnimator.SetTrigger("Activate");
                }

                // Désactiver l'objet verrouillé correspondant si nécessaire
                int index = levelButtons.IndexOf(button);
                if (index < starsLocked.Length)
                {
                    starsLocked[index].SetActive(false);
                }
            }
            else
            {
                // Désactiver le bouton si le joueur n'a pas assez d'étoiles
                //button.SetActive(false);

                // Activer l'objet verrouillé correspondant si nécessaire
                int index = levelButtons.IndexOf(button);
                if (index < starsLocked.Length)
                {
                    starsLocked[index].SetActive(true);
                }
            }
        }

        // Gérer les étoiles des niveaux
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

            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                PlayerPrefs.SetInt($"Level_{i}_Star_{j}_Active", starsSpritesData[i].starsSprites[j].activeSelf ? 1 : 0);
            }
        }
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        for (int i = 0; i < starsSpritesData.Length; i++)
        {
            starsSpritesData[i].isUnlocked = PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1;
            starsSpritesData[i].animationTriggered = PlayerPrefs.GetInt($"Level_{i}_AnimationTriggered", 0) == 1;

            for (int j = 0; j < starsSpritesData[i].starsSprites.Length; j++)
            {
                bool isActive = PlayerPrefs.GetInt($"Level_{i}_Star_{j}_Active", 0) == 1;
                starsSpritesData[i].starsSprites[j].SetActive(isActive);
            }
        }
    }


}
