using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingSystem : MonoBehaviour
{
    
    [SerializeField]private  Animator animator;

    [SerializeField] private AudioManager audioManager;

    [SerializeField] private AudioMixer audioMixer;

    private bool animationForward = false;

    bool isVibration = true;


    public string exposedParameterName = "SFX";

    private float sfxBaseVolume;

    private void Start()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer non défini dans le script MixerVolumeGetter !");
            return;
        }

        // Obtenez le volume actuel de l'AudioMixer
        sfxBaseVolume = GetMixerVolume();
        Debug.Log("Volume de l'AudioMixer: " + sfxBaseVolume);
    }
    float GetMixerVolume()
    {
        float volume = 0f;
        bool result = audioMixer.GetFloat(exposedParameterName, out volume);
        if (!result)
        {
            Debug.LogError("Impossible d'obtenir le volume de l'AudioMixer pour le paramètre exposé: " + exposedParameterName);
        }
        return volume;
    }


    public void SystemMenu()
    {
        if (animationForward)
        {
            animator.SetBool("IsOn", false);
            animationForward = false;
        }
        else
        {
            animator.SetBool("IsOn", true);
            animationForward = true;
        }
        
    }
    
    public void LoadMenuSelection()
    {
        Debug.Log("la");
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnableVibration()
    {
        // Activer les vibrations du téléphone
        Handheld.Vibrate();
        isVibration = true;
        Debug.Log("Vibration activée");
    }

    // Fonction pour désactiver les vibrations
    public void DisableVibration()
    {
        // Désactiver les vibrations du téléphone
        isVibration = false;
        Debug.Log("Vibration désactivée");
    }

    public void EnableMusic()
    {
        audioManager.Unpaused(SoundState.MENU);

    }

    // Fonction pour désactiver les vibrations
    public void DisableMusic()
    {
        audioManager.Paused(SoundState.MENU);        

    }
    public void EnableSFX()
    {
        audioMixer.SetFloat(exposedParameterName, sfxBaseVolume);
    }

    // Fonction pour désactiver les vibrations
    public void DisableSFX()
    {
        audioMixer.SetFloat(exposedParameterName, -80f);

    }
}
