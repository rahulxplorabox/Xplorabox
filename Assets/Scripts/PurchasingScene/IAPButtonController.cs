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
    public InputField mystatus;
    string purchaseDetails;
   

    public void OnPurchaseComplete(Product Purchased_product)
    {
        print("Successfully Purchased =>" + Purchased_product.definition.id.ToString());
        var decoded_receipt = (Dictionary<string, object>)MiniJson.JsonDecode(Purchased_product.receipt);
        if (null != decoded_receipt)
        {
            var store = (string)decoded_receipt["Store"];
            var payload = (string)decoded_receipt["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
            //print("Sandeep ka Payload =>" + payload.ToString());

            var decoded_payload = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            var decoded_Json = (string)decoded_payload["json"];
            print("decoded Parload ka json =>" + decoded_Json.ToString());
            purchaseDetails = decoded_Json.ToString();
            mystatus.text = purchaseDetails;
        }
        StartCoroutine(SendDatatoAppserver(purchaseDetails));
    }

    public void OnPurchaseFailure(Product Failed_product, PurchaseFailureReason reason)
    {
        print("Failed Purchased =>" + Failed_product.definition.id.ToString());
        print("Failed due to  ===" + reason);
    }

    IEnumerator SendDatatoAppserver(string receipt)
    {
        print("Sending data to app server");
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        string OnPurchaseCompleteUrl = "https://sandbox.app.xplorabox.com/api/success-purchase";
       
        WWWForm formdata = new WWWForm();
        var deviceId = SystemInfo.deviceUniqueIdentifier.ToString();
        var userId = PlayerPrefs.GetString("_SubmitedDetailsUserId");
        var localAccessToken = PlayerPrefs.GetString("_LocalAccessToken");
        if (!string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(receipt) &&!string.IsNullOrEmpty(localAccessToken))
        {
            print("userId Id=== " + userId);
            print("receipt === " + receipt);
            print("Device Id=== " + deviceId);
           
            print("Local AccessToken =" + localAccessToken);
            formdata.AddField("app_user_id", userId);
            formdata.AddField("receipt", receipt);
            formdata.AddField("device_id", deviceId);
            UnityWebRequest send_to_appserver = UnityWebRequest.Post(OnPurchaseCompleteUrl, formdata);
            send_to_appserver.SetRequestHeader("Accept", "application/json");
            send_to_appserver.SetRequestHeader("Authorization", "Bearer " + localAccessToken);
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
        else
        {
            print("Parameters are not valid");
        }
    }


    

}
