using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using static UnityEngine.Purchasing.IAPButton;

public class IAPButtonController : MonoBehaviour
{

    public void OnPurchaseComplete(Product Purchased_product)
    {
        print("Successfully Purchased =>" + Purchased_product.definition.id.ToString());
        print("Purchased Recepit =>" + Purchased_product.receipt.ToString());
        print("Purchased transaction Id =>" + Purchased_product.transactionID.ToString());
        StartCoroutine(SendDatatoAppserver());
    }


    public void OnPurchaseFailure(Product Failed_product, PurchaseFailureReason reason)
    {
        print("Failed Purchased =>" + Failed_product.definition.id.ToString());
        print("Failed due to  ===" + reason);
    }

    IEnumerator SendDatatoAppserver()
    {
        print("Sending data to app server");
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        string Apiversion_URL = "https://app.xplorabox.com/api/get_app_version";
        WWWForm formdata = new WWWForm();
        formdata.AddField("device_id", SystemInfo.deviceUniqueIdentifier.ToString());
        UnityWebRequest send_to_appserver = UnityWebRequest.Post(Apiversion_URL, formdata);
        send_to_appserver.SetRequestHeader("Accept", "application/json");
        yield return send_to_appserver.SendWebRequest();
        if (send_to_appserver.isNetworkError || send_to_appserver.isNetworkError)
        {
            print("There is a problem in app version check" + send_to_appserver.error);
        }
        else
        {
            if (send_to_appserver.isDone)
            {
                print("responce==========" + send_to_appserver.downloadHandler.text.ToString());
                ApploaderController.ApploaderController_Instance.Deactivate_Apploader();

            }
        }
    }


    

}
