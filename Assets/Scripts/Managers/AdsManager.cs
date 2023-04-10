using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class AdsManager : Singleton<AdsManager>
    {
        private const string ADUnitRewardExpensive = "";
        private const string ADUnitRewardNormal = "";
        private const string ADUnitRewardCheep = "";
        private const string ADUnitInterstitialExpensive = "";
        private const string ADUnitInterstitialNormal = "";
        private const string ADUnitInterstitialCheep = "";

        public static bool isGrowInterstitial = true;
        public static bool isEvolveInterstitial = true;

        private Coroutine _load;

        [SerializeField]
        private GameObject _notFoundReward;

        private void Awake()
        {
            MobileAds.Initialize((initStatus) =>
            {
                Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            MonoBehaviour.print("Adapter: " + className + " not ready.");
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            MonoBehaviour.print("Adapter: " + className + " is initialized.");
                            break;
                    }
                }
            });
            InitInterstitial();
            InitRewarded();
            Debug.Log("Ads init successful");
        }

        #region Interstital
        private InterstitialAd _interstitialExpensive;
        private InterstitialAd _interstitialNormal;
        private InterstitialAd _interstitialCheep;

        private void InitInterstitial()
        {
            // Create an empty ad request.
            string adUnitId = ADUnitInterstitialExpensive;
            string adUnitIdNormal = ADUnitInterstitialNormal;
            string adUnitIdCheep = ADUnitInterstitialCheep;
            this._interstitialExpensive = new InterstitialAd(adUnitId);
            this._interstitialNormal = new InterstitialAd(adUnitIdNormal);
            this._interstitialCheep = new InterstitialAd(adUnitIdCheep);
            RequestInterstitial();
        }
        AdRequest requestExp;
        AdRequest requestNormal;
        AdRequest requestCheep;
        private void RequestInterstitial()
        {
            requestExp = new AdRequest.Builder().Build();
            requestNormal = new AdRequest.Builder().Build();
            requestCheep = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            this._interstitialExpensive.LoadAd(requestExp);
            this._interstitialNormal.LoadAd(requestNormal);
            this._interstitialCheep.LoadAd(requestCheep);
        }


        public void ShowInterstitial()
        {
            if (IAPManager.IsSubscribeEnable) return;
            if (this._interstitialExpensive.IsLoaded())
            {
                _interstitialExpensive.Show();
                requestExp = new AdRequest.Builder().Build();
                this._interstitialExpensive.LoadAd(requestExp);
            }
            else if (this._interstitialNormal.IsLoaded())
            {
                _interstitialNormal.Show();

                requestNormal = new AdRequest.Builder().Build();
                this._interstitialNormal.LoadAd(requestNormal);
            }
            else if (this._interstitialCheep.IsLoaded())
            {
                _interstitialCheep.Show();

                requestCheep = new AdRequest.Builder().Build();
                this._interstitialCheep.LoadAd(requestCheep);
            }
            else
            {
                Debug.Log("Interstitial is not ready yet");
                StartCoroutine(TryToLoadInterstitialVideo());
            }
        }


        private IEnumerator TryToLoadInterstitialVideo()
        {
            for (int i = 0; i < 5; i++)
            {
                if (this._interstitialExpensive.IsLoaded())
                {
                    _interstitialExpensive.Show();
                    AdRequest request = new AdRequest.Builder().Build();
                    this._interstitialExpensive.LoadAd(request);
                    yield break;
                }
                else if (this._interstitialNormal.IsLoaded())
                {
                    _interstitialNormal.Show();

                    AdRequest request = new AdRequest.Builder().Build();
                    this._interstitialNormal.LoadAd(request);
                    yield break;
                }
                else if (this._interstitialCheep.IsLoaded())
                {
                    _interstitialCheep.Show();

                    AdRequest request = new AdRequest.Builder().Build();
                    this._interstitialCheep.LoadAd(request);
                    yield break;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
        #endregion

        #region Rewarded
        private RewardedAd _rewardedAdExpensive;
        private RewardedAd _rewardedAdNormal;
        private RewardedAd _rewardedAdCheep;

        private void InitRewarded()
        {
#if UNITY_ANDROID
            string adUnitIdExpensive = ADUnitRewardExpensive;
            string adUnitIdNormal = ADUnitRewardNormal;
            string adUnitIdCheep = ADUnitRewardCheep;
#elif UNITY_IPHONE
        string adUnitId = "R-M-DEMO-rewarded-client-side-rtb";
#else
        string adUnitId = "unexpected_platform";
#endif
            this._rewardedAdExpensive = new RewardedAd(adUnitIdExpensive);
            this._rewardedAdNormal = new RewardedAd(adUnitIdNormal);
            this._rewardedAdCheep = new RewardedAd(adUnitIdCheep);
            _rewardedAdExpensive.OnAdClosed += HandleRewarded;
            _rewardedAdNormal.OnAdClosed += HandleRewarded;
            _rewardedAdCheep.OnAdClosed += HandleRewarded;
            RequestRewarded();
        }

        private void HandleRewarded(object sender, EventArgs args)
        {
            RequestRewarded();
        }
        AdRequest requestExpensiveReward;
        AdRequest requestNormalReward;
        AdRequest requestCheepReward;
        private void RequestRewarded()
        {
            // Create an empty ad request.
            requestExpensiveReward = new AdRequest.Builder().Build();
            requestNormalReward = new AdRequest.Builder().Build();
            requestCheepReward = new AdRequest.Builder().Build();
            // Load the rewarded with the request.
            this._rewardedAdExpensive.LoadAd(requestExpensiveReward);
            this._rewardedAdNormal.LoadAd(requestNormalReward);
            this._rewardedAdCheep.LoadAd(requestCheepReward);
        }


        public void ShowRewardedAd(Action action = null)
        {
            if (this._rewardedAdExpensive.IsLoaded())
            {
                _rewardedAdExpensive.Show();
                action?.Invoke();
                requestExpensiveReward = new AdRequest.Builder().Build();
                this._rewardedAdExpensive.LoadAd(requestExpensiveReward);
            }
            else if (_rewardedAdNormal.IsLoaded())
            {
                _rewardedAdNormal.Show();
                action?.Invoke();
                requestNormalReward = new AdRequest.Builder().Build();
                this._rewardedAdNormal.LoadAd(requestNormalReward);
            }
            else if (_rewardedAdCheep.IsLoaded())
            {
                _rewardedAdCheep.Show();
                action?.Invoke();
                requestCheepReward = new AdRequest.Builder().Build();
                this._rewardedAdCheep.LoadAd(requestCheepReward);
            }
            else
            {
                StartCoroutine(ShowRewardNotLoadText());
            }
        }

        private IEnumerator ShowRewardNotLoadText()
        {
            _notFoundReward.SetActive(true);
            
            yield return new WaitForSeconds(2f);
            Debug.Log("RewardNotLoad");
            
            _notFoundReward.SetActive(false);
        }
        #endregion

        public void LoadValue()
        {
            isEvolveInterstitial = RemoteConfig.GetBool("Ads_is_Evolve_Interstitial");
            isGrowInterstitial = RemoteConfig.GetBool("Ads_is_Grow_Interstitial");
        }

        //private IEnumerator TryToLoadRewardVideo(Action action = null)
        //{
        //    if (_rewardLoadingScreen == null) { _rewardLoadingScreen = Instantiate(_rewardLoadingScreenPrefab, GameObject.FindGameObjectWithTag("Boards").transform); }
        //    else
        //    {
        //        _rewardLoadingScreen.SetActive(true);
        //    }
        //    for (int i = 0; i < 5; i++)
        //    {
        //        if (_rewardedAd.IsLoaded())
        //        {
        //            _rewardedAd.Show();
        //            _actionEarn = action;
        //            _rewardLoadingScreen.SetActive(false);
        //            _load = null;
        //            yield break;
        //        }
        //        yield return new WaitForSeconds(0.5f);
        //    }
        //    _rewardLoadingScreen.SetActive(false);
        //    _load = null;
        //    if (_error == null) { _error = Instantiate(_errorPrefab, GameObject.FindGameObjectWithTag("Boards").transform); }
        //    else
        //    {
        //        _error.SetActive(true);
        //    }
        //    yield return new WaitForSeconds(2f);
        //    _error.SetActive(false);
        //}
    }
}