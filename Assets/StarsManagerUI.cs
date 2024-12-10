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

    [SerializeField] private List<GameObject> levelButtons;

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
        foreach (var button in levelButtons)
        {
            // V�rifier si le bouton a un composant SceneLoader
            SceneLoader sceneLoader = button.GetComponent<SceneLoader>();
            if (sceneLoader == null) continue;

            // V�rifier si le joueur a suffisamment d'�toiles pour activer ce bouton
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

                // D�sactiver l'objet verrouill� correspondant si n�cessaire
                int index = levelButtons.IndexOf(button);
                if (index < starsLocked.Length)
                {
                    starsLocked[index].SetActive(false);
                }
            }
            else
            {
                // D�sactiver le bouton si le joueur n'a pas assez d'�toiles
                //button.SetActive(false);

                // Activer l'objet verrouill� correspondant si n�cessaire
                int index = levelButtons.IndexOf(button);
                if (index < starsLocked.Length)
                {
                    starsLocked[index].SetActive(true);
                }
            }
        }

        // G�rer les �toiles des niveaux
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
