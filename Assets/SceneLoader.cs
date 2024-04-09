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

    public void LoadScene()
    {
        //if(currentStarsCount > nbStarsNeeded)
        //{
            SceneManager.LoadScene(levelToLoad);
        //}
    }
}