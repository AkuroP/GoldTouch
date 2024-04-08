using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GemsManager : MonoBehaviour
{
    public GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    private void NextGem()
    {
        int nextGem = Random.Range(0, _gameManager.AllGems.Length);
        Debug.Log("Next gem is : " +  nextGem);
    }
}
