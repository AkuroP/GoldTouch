using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


public class SceneLoader : MonoBehaviour
{

    [Scene]
    public string levelToLoad;

    public int nbStarsNeeded;

    [SerializeField] private int levelNumberLoaded;



    public void LoadScene()
    {
        if(nbStarsNeeded <= -1) SceneManager.LoadScene(levelToLoad);

        if (SettingSystem.instance.nbStars >= nbStarsNeeded)
        {
            AudioManager.instance.PlayRandom(SoundState.BUTTON);
            SceneManager.LoadScene(levelToLoad);
            SettingSystem.instance.levelNumber = levelNumberLoaded - 1;

        }

    }
}