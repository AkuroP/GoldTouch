using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // Permet à Unity de sérialiser cette classe et d'afficher ses champs dans l'inspecteur
public class Datata
{
    public GameObject[] starsSprites; 

    public Datata(int size)
    {

        starsSprites = new GameObject[3]; 
    }
}
public class StarsManagerUI : MonoBehaviour
{
    public Datata[] starsSpritesData;

    [SerializeField] private GameObject[] starsLocked;
    private void Update()
    {
        if (SettingSystem.instance.nbStars >= 2)
        {
            starsLocked[0].SetActive(false);
        }
        if (SettingSystem.instance.nbStars >= 4)
        {
            starsLocked[0].SetActive(false);
        }

        if (SettingSystem.instance.donnees[0].bools[0])
        {
            starsSpritesData[0].starsSprites[0].SetActive(true);
        }
        if (SettingSystem.instance.donnees[0].bools[1])
        {
            starsSpritesData[0].starsSprites[1].SetActive(true);
        }
        if (SettingSystem.instance.donnees[0].bools[2])
        {
            starsSpritesData[0].starsSprites[2].SetActive(true);
        }




        if (SettingSystem.instance.donnees[1].bools[0])
        {
            starsSpritesData[0].starsSprites[0].SetActive(true);
        }
        if (SettingSystem.instance.donnees[1].bools[1])
        {
            starsSpritesData[0].starsSprites[1].SetActive(true);
        }
        if (SettingSystem.instance.donnees[1].bools[2])
        {
            starsSpritesData[0].starsSprites[2].SetActive(true);
        }




        if (SettingSystem.instance.donnees[2].bools[0])
        {
            starsSpritesData[0].starsSprites[0].SetActive(true);
        }
        if (SettingSystem.instance.donnees[2].bools[1])
        {
            starsSpritesData[0].starsSprites[1].SetActive(true);
        }
        if (SettingSystem.instance.donnees[2].bools[2])
        {
            starsSpritesData[0].starsSprites[2].SetActive(true);
        }
    }
}
