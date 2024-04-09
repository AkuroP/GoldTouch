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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gems"))
        {
            GemsFusion collideGems = collision.gameObject.GetComponent<GemsFusion>();

            if (collideGems.gemsIndex == gemsIndex)
            {
                if (!gameObject.activeSelf || !collision.gameObject.activeSelf)
                {
                    return;
                }
                print("same gems");
                collision.gameObject.SetActive(false);
                Destroy((collision.gameObject));
                GameObject nextGems = Instantiate(GameManager.instance.AllGems[gemsIndex + 1]);
                nextGems.transform.position = transform.position;
                
                nextGems.GetComponent<GemsFusion>()._rb.AddExplosionForce(_explosionForce, nextGems.transform.position, _explosionRadius);

                gameObject.SetActive(false);
                Destroy(gameObject);

            }
        }
    }
}
