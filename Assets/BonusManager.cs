using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : MonoBehaviour
{


    [SerializeField] private Button freeTurnButton;
    [SerializeField] private Button mergeGemsButton;
    [SerializeField] private Button evolveGemButton;

    [SerializeField] private int freeTurnCount; // Nombre de coups gratuits disponibles
    [SerializeField] private int mergeUses;// Nombre de fusions disponibles
    [SerializeField] private int evolveUses; // Nombre d'évolutions disponibles

    private void Start()
    {
        // Assigner les méthodes aux boutons UI
        freeTurnButton.onClick.AddListener(ActivateFreeTurnsBonus);
        mergeGemsButton.onClick.AddListener(ActivateMergeGemsBonus);
        evolveGemButton.onClick.AddListener(ActivateEvolveGemMode);
    }

    // Méthode pour le premier bouton : Activer les tours gratuits
    public void ActivateFreeTurnsBonus()
    {
        if (freeTurnCount > 0)
        {
            freeTurnCount--;
            GameManager.instance.ActivateFreeTurns(5); // Donne 5 coups gratuits (modifiable)
        }
    }

    // Méthode pour le deuxième bouton : Fusionner les gemmes identiques
    public void ActivateMergeGemsBonus()
    {
        if (mergeUses > 0)
        {
            mergeUses--;
            GameManager.instance.MergeIdenticalGems();

        }
    }

    // Méthode pour le troisième bouton : Mode d'évolution
    public void ActivateEvolveGemMode()
    {
        if (evolveUses > 0)
        {
            evolveUses--;
            GameManager.instance.ActivateEvolutionMode();

        }
    }
}
