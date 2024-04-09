using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allGems;

    public GameObject[] AllGems => _allGems;

    public static GameManager instance;

    [SerializeField] private int scoreToBeat;
    [SerializeField] private GameObject winScreen;

    [HideInInspector]public int actualScore;

    public TextMeshProUGUI textScoreToBeat;
    public TextMeshProUGUI textActualScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    private void Start()
    {
        textScoreToBeat.text = "Score To Beat : " + scoreToBeat.ToString();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newGems = Instantiate(_allGems[0]);
            newGems.transform.position = transform.position;

            GemsFusion newGemsScript = newGems.GetComponent<GemsFusion>();
            newGemsScript.gemsIndex = 0;
        }
        if(actualScore > scoreToBeat)
        {
            Debug.Log("You win");
            winScreen.SetActive(true);
        }
        textActualScore.text = "My score : " + actualScore.ToString();
        Debug.Log(actualScore);
        Debug.Log(scoreToBeat);
    }
}
