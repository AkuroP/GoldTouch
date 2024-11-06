using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLoseEndless : MonoBehaviour
{
    [SerializeField] private float outOfTriggerTimer = 3.0f; // Temps avant la fin si une gem reste hors de la zone
    private float currentTimer = 0f;
    private bool isTimerRunning = false;

    private HashSet<GameObject> gemsInsideTrigger = new HashSet<GameObject>();

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isEndless)  // Vérifie que GameManager.instance existe
        {
            HandleEndlessMode();
        }
        Debug.Log(currentTimer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gems"))
        {
            gemsInsideTrigger.Add(other.gameObject);

            if (!isTimerRunning)
            {
                isTimerRunning = true;
                currentTimer = outOfTriggerTimer;
                Debug.Log("Début du timer car un objet est dans la zone");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gems"))
        {
            gemsInsideTrigger.Remove(other.gameObject);

            if (gemsInsideTrigger.Count == 0)
            {
                isTimerRunning = false;
                currentTimer = outOfTriggerTimer;
                Debug.Log("Stop du timer car la zone est vide");
            }
        }
    }

    private void HandleEndlessMode()
    {
        if (isTimerRunning)
        {
            currentTimer -= Time.deltaTime;
            Debug.Log($"Timer: {currentTimer}");  // Affiche le compte à rebours

            if (currentTimer <= 0f)
            {
                GameManager.instance.EndGame();
                Debug.Log("Perdu - timer écoulé");
                isTimerRunning = false;  // Stop le timer après la perte
            }
        }
    }
}
