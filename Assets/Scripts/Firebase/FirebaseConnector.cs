using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Managers;
using UnityEngine;
using Utilities;

public class FirebaseConnector : Singleton<FirebaseConnector>
{
    public static bool IsRemoteActive { get; private set; }


    public void Awake()
    {
        RemoteConfig.OnConfigUpdate += BoostManager.LoadValues;
        RemoteConfig.OnConfigUpdate += AdsManager.Instance.LoadValue;
        RemoteConfig.OnConfigUpdate += Managers.MainShopManager.Instance.LoadValue;
        RemoteConfig.OnConfigUpdate += LevelGrowManager.LoadValue;
        RemoteConfig.OnConfigUpdate += Managers.EvolveShopManager.Instance.UpdateEvolveCost;
        RemoteConfig.OnConfigUpdate += AncestorsManager.Instance.UpdateCost;
        LoadFirebase();
    }

    private static void LoadFirebase()
    {
        Debug.Log("Started Loading");
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("internet NotReachable");
        }
        else
        {
            Debug.Log("Start firebase init.");

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                try
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                    if (task.IsCompleted)
                    {
                        Debug.Log("Firebase init successfully");

                        //DynamicLinksManager.Instance.Init();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }

                Debug.Log("Start remote config init.");
                
                FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWith(_ =>
                {
                    try
                    {
                        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(taskConfig =>
                        {
                            if (taskConfig.IsCompleted)
                            {
                                Debug.Log("Remote init successfully.");

                                IsRemoteActive = true;
                                IAPManager.Price = (int) RemoteConfig.GetLong("Price");
                                
                                IAPManager.Instance.IAPInitialization();
                                /*                                  if (RemoteConfig.GetString("Application_Version") != Application.version)
                                    {
                                        GameLoader.Instance.ChangeStateWarning(true);
                                        IsGameVersionChecked = false;
                                    }
                                    else IsGameVersionChecked = true;*/
                            }

                            Debug.Log(
                                $"Firebase config last fetch time {FirebaseRemoteConfig.DefaultInstance.Info.FetchTime}.");


                            FirebaseAnalytics.LogEvent("sign_up");
                            RemoteConfig.UpdateConfig();
                            Debug.Log($"Count Var :{FirebaseRemoteConfig.DefaultInstance.AllValues.Count}");
                            /*PlayGamesPlatform.DebugLogEnabled = true;
                                PlayGamesPlatform.Activate();
                                PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
                                {
                                    if (status == SignInStatus.Success)
                                    {
                                        Debug.Log("Sign_in_google");
                                        StateString = "Sign in Google.";
                                        IsPlayGameActive = true;
                                        PlayGamesPlatform.Instance.RequestServerSideAccess(false, (string authCode) =>
                                        {
                                            AuthCode = authCode;
                                            UserId = PlayGamesPlatform.Instance.GetUserId();
                                            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                                            Credential credential = PlayGamesAuthProvider.GetCredential(AuthCode);
                                            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    Debug.LogError("SignInWithCredentialAsync was canceled.");

                                                    StateString = "SignInWithCredentialAsync was canceled.";
                                                    IsFireBaseActive = true;
                                                    return;// throw new Exception("SignInWithCredentialAsync was canceled.");

                                                }
                                                if (task.IsFaulted)
                                                {
                                                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                                                    IsFireBaseActive = true;
                                                    return;// throw new Exception("SignInWithCredentialAsync encountered an error: " + task.Exception);

                                                }

                                                FirebaseUser user = task.Result;
                                                IsFirebaseAuthActive = true;
                                                */
                            /*                                              Debug.LogFormat("User signed in successfully: {0} ({1})",
                                                                                                                                 user.DisplayName, user.UserId);
                                                                                                                             if (user != null && PlayerConfig.nickname == "Guest")
                                                                                                                             {
                                                                                                                                 Debug.Log(user.DisplayName);
                                                                                                                                 PlayerConfig.nickname = user.DisplayName;
                                                                                                                             }*/
                            /*
                                                                           });
                                                                           IsFireBaseActive = true;
                                                                       });
                                                                   }
                                                                   else
                                                                   {
                                                                       IsFireBaseActive = true;
                                                                       //Debug.LogError("Authenticate is not success.");
                                                                       return;
                                                                   }
                               
                                                               });*/
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                });
            });
        }
    }
}