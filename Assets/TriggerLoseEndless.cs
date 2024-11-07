using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class TriggerLoseEndless : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{


// ADS
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;  


    [SerializeField] private float outOfTriggerTimer = 3.0f; // Temps avant la fin si une gem reste hors de la zone
    private float currentTimer = 0f;
    private bool isTimerRunning = false;

    private HashSet<GameObject> gemsInsideTrigger = new HashSet<GameObject>();

    private void Start()
    {
        
        LoadAd(); 

    }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isEndless)  // V�rifie que GameManager.instance existe
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
                Debug.Log("D�but du timer car un objet est dans la zone");
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
            Debug.Log($"Timer: {currentTimer}");  // Affiche le compte � rebours

            if (currentTimer <= 0f)
            {
                Debug.Log("Endless finished");
                ShowAd(); // Lance la pub si endless
                Debug.Log("Endless Ads");
                Debug.Log("Perdu - timer �coul�");
                isTimerRunning = false;  // Stop le timer apr�s la perte
            }
        }
    }








        //ADS

        // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

        // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
    }

        // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.

        GameManager.instance.EndGame();
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        
        GameManager.instance.EndGame();
    }
 
    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) {GameManager.instance.EndGame();}
}
