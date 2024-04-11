using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInGame : MonoBehaviour
{

    public void DestroySelf()
    {
        GameManager.instance.CanPlay = true;
        Destroy(gameObject);
    }

    public void DeactivateCombo()
    {
        GameManager.instance._comboGO.gameObject.SetActive(false);
    }


}
