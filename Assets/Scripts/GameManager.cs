using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allGems;

    public GameObject[] AllGems => _allGems;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

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
    }
}
