using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Messaging;
using System;
using Unity.Notifications.Android;


public class FirebaseInit : MonoBehaviour
{

    #region[Members Vars]
    AndroidNotificationChannel Xplorabox_NotificationChannel;
    public string Channel_ID;
    public string Channel_Name;
    #endregion

    #region[MonoBehaviour]
    
    private void Awake()
    {
        SetupNotificationChannel();
    }
    
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
    #endregion
   
    #region Firebase Methods
    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            Debug.Log("Received a new message from: " + e.Message.From);
            Debug.Log(e.Message.Notification.Title);
            Debug.Log(e.Message.Notification.Body);
            create_notification(e.Message.Notification.Title, e.Message.Notification.Body);
        }
        catch (Exception E)
        {
            print("Recieved exception on OnMessageReceived " + E);
        }
    }

    #endregion
    
    #region NotificationBuilder

    #region[Setup_SetupNotificationChannel]
    public void SetupNotificationChannel()
    {
        Channel_ID = "Xplorabox";
        Channel_Name = "Xplorabox_Notification";
    }
    #endregion

    #region[Notifications Builder]
    public void create_notification(string Notification_Tittle, string Notification_Text)
    {
        Xplorabox_NotificationChannel = new AndroidNotificationChannel()
        {
            Id = Channel_ID,
            Name = Channel_Name,
            Description = "For Generic notifications",
            Importance = Importance.High,
        };

        AndroidNotificationCenter.RegisterNotificationChannel(Xplorabox_NotificationChannel);

        AndroidNotification notification = new AndroidNotification()
        {
            Title = Notification_Tittle,
            Text = Notification_Text,
            FireTime = DateTime.Now.AddSeconds(0.5f),
            SmallIcon = "icon_small",
        };

        var c = AndroidNotificationCenter.SendNotification(notification, Channel_ID);

        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler = delegate (AndroidNotificationIntentData data)
        {
            var msg = "Notification recieved :" + data.Id + "\n";
            msg += "\n Title :: " + data.Notification.Title;
            msg += "\n Body :: " + data.Notification.Text;
            msg += "\n .channel :: " + data.Channel;
            Debug.Log(msg);
        };

        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            Debug.Log("App was opened with notification!");
        }
    }

    #endregion

    #endregion
}



