using System;
using UnityEngine;
using TMPro;
using System.Globalization;
using Utilities;
using System.Collections;
using Managers.Interfaces;
using Zenject;

namespace Managers
{
    public class YouWereAway : Singleton<YouWereAway>
    {
        [SerializeField]
        private TextMeshProUGUI coins;

        [SerializeField]
        private TextMeshProUGUI timer;

        [Inject] 
        private ICurrencyManager _currencyManager;

        private static DateTime LastUserTimeInGame
        {
            get => DateTime.Parse(
                PlayerPrefs.GetString("LastTime", DateTime.Now.ToString()));
            set => PlayerPrefs.SetString("LastTime", value.ToString());
        }

        private static int _coinsPerHour;
        private static int _maxCoin;


        private static int InitReward()
        {
            var coin = _maxCoin <= (DateTime.Now - LastUserTimeInGame).Hours * _coinsPerHour
                ? _maxCoin
                : (DateTime.Now - LastUserTimeInGame).Hours * _coinsPerHour;
            return coin * (IAPManager.IsDoubleFoodBuying ? 2 : 1);
        }


        private void UpdateText()
        {
            Debug.Log("Hour " + (DateTime.Now - LastUserTimeInGame).Hours);
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            coins.text = "+" + InitReward().ToString("###,###,##0", CultureInfo.CreateSpecificCulture("es-ES"));
            timer.text = (DateTime.Now - LastUserTimeInGame).ToString(@"hh\:mm\:ss");
        }


        private IEnumerator OnApplicationPause(bool pause)
        {
            if (pause)
            {
                YouWereAway.LastUserTimeInGame = DateTime.Now;
            }
            else
            {
                while (!FirebaseConnector.IsRemoteActive)
                    yield return null;

                _coinsPerHour = (int) RemoteConfig.GetDouble("Start_Coin_per_hour");
                _maxCoin = _coinsPerHour * 8;
                if (InitReward() > 0)
                {
                    UpdateText();
                }
            }
        }

        public void EarnCoin()
        {
            //UpgradesManager.AllCoins += InitReward();
            _currencyManager.ChangeFood(InitReward());
            //UIManager.Instance.UpdateCoinValue();

            LastUserTimeInGame = DateTime.Now;
            gameObject.SetActive(false);
        }

        public void EarnCoinsX2()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                UpgradesManager.AllCoins += InitReward();
                UpgradesManager.AllCoins += InitReward();
                UIManager.Instance.UpdateCoinValue();
                gameObject.SetActive(false);

                LastUserTimeInGame = DateTime.Now;
            });
        }
    }
}