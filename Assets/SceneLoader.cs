using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


public class SceneLoader : MonoBehaviour
{

    [Scene]
    public string levelToLoad;

    [SerializeField] private int nbStarsNeeded;

    [SerializeField] private int levelNumberLoaded;

    public void LoadScene()
    {
        if(SettingSystem.instance.nbStars >= nbStarsNeeded)
        {
            SceneManager.LoadScene(levelToLoad);
            SettingSystem.instance.levelNumber = levelNumberLoaded - 1;
        }

    }
}