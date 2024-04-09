using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


public class SceneLoader : MonoBehaviour
{

    [Scene]
    public string levelToLoad;


    public void LoadScene()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}