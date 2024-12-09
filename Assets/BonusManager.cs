using JetBrains.Annotations;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : MonoBehaviour
{


    [SerializeField] private Button freeTurnButton;
    [SerializeField] private Button mergeGemsButton;
    [SerializeField] private int nbFreeTurn; // Nombre de coups gratuits disponibles

    private void Start()
    {
        // Assigner les m�thodes aux boutons UI
        freeTurnButton.onClick.AddListener(ActivateFreeTurnsBonus);
        mergeGemsButton.onClick.AddListener(ActivateMergeGemsBonus);
    }

    // M�thode pour le premier bouton : Activer les tours gratuits
    public void ActivateFreeTurnsBonus()
    {
        if (SettingSystem.instance.nbFreeTurneBonus <= 0)
        {
            Debug.Log("Pas de bonus 1 disponible !");
            return;
        }

        //if (nbFreeTurn > 0)
        //{
        //    Debug.Log("Le bonus de tours gratuits est d�j� en cours !");
        //    return;
        //}

        if (GameManager.instance.freeTurns == 0)
        {
            nbFreeTurn--;
            SettingSystem.instance.nbFreeTurneBonus--;
            GameManager.instance.ActivateFreeTurns(nbFreeTurn);
            SettingSystem.instance.SaveBonuses();
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSelectItem);
        }
        else 
        {
            Debug.Log("Le bonus de tours gratuits est d�j� en cours !");
            return;
        }


        
    }

    // M�thode pour le deuxi�me bouton : Fusionner les gemmes identiques
    public void ActivateMergeGemsBonus()
    {
        // V�rifie si le joueur a des bonus 2 disponibles
        if (SettingSystem.instance.nbAutoMergeBonus <= 0)
        {
            Debug.Log("Pas de bonus 2 disponible !");
            return;
        }

        // Active le bonus et diminue le nombre de bonus disponibles
        SettingSystem.instance.nbAutoMergeBonus--;
        GameManager.instance.MergeIdenticalGems();

        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSelectItem);

        // Sauvegarde la mise � jour avec la m�thode de SettingSystem
        SettingSystem.instance.SaveBonuses();
    }


}
