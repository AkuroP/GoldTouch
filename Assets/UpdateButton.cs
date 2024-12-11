using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class UpdateButton : MonoBehaviour
{
    
    IEnumerator Start()
    {
        while(true)
        {
            yield return null;
            if(IAPManager.storeController != null) break;
        }

        GetComponentInChildren<TMP_Text>().text = 
        IAPManager.storeController.products.WithID(GetComponent<CodelessIAPButton>().productId).metadata.localizedPriceString;

    }

}
