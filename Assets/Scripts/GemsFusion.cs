using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsFusion : MonoBehaviour
{
    public int gemsIndex;
    public Rigidbody _rb;

    public float _explosionForce;
    public float _explosionRadius;

    [SerializeField] private int fusionScore;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gems"))
        {
            GemsFusion collideGems = collision.gameObject.GetComponent<GemsFusion>();

            if (collideGems.gemsIndex == gemsIndex)
            {
                if (!transform.root.gameObject.activeSelf || !collision.transform.root.gameObject.activeSelf)
                {
                    return;
                }
                print("same gems");
                //Debug.Log(collideGems.name);
                GameManager.instance._combo += 1;
                GameManager.instance._inCombo = true;

                GameManager.instance.ResetComboTimer();

                if (GameManager.instance._combo > 1)
                {
                    GameManager.instance._comboGO.gameObject.SetActive(true);
                    GameManager.instance._comboGO.text = "Combo ! x" + GameManager.instance._combo;
                }

                collision.transform.root.gameObject.SetActive(false);
                Destroy((collision.transform.root.gameObject));
                GameObject nextGems = Instantiate(GameManager.instance.AllGems[gemsIndex + 1]);
                nextGems.transform.position = transform.position;
                AudioManager.instance.PlayRandom(SoundState.FUSION);

                GameManager.instance.StartCoroutine(GameManager.instance.IncreaseScore(fusionScore));
                nextGems.GetComponentInChildren<GemsFusion>()._rb.AddExplosionForce(_explosionForce, nextGems.transform.position, _explosionRadius);
                Instantiate(GameManager.instance._fx, nextGems.transform.position, Quaternion.identity);
                
                if(SettingSystem.instance != null) SettingSystem.instance.Vibrate();

                transform.root.gameObject.SetActive(false);
                Destroy(transform.root.gameObject);

            }
        }
    }
}
