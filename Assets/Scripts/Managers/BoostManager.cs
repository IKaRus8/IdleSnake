using System;
using System.Collections;
using Managers.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace Managers
{
    public class BoostManager : Singleton<BoostManager>
    {
        public static bool isBoostSpeed;
        public static bool isFullControl;
        public static bool isBoostMetabolism;
        private static bool isBoostSpeedCooldown;
        private static bool isBoostMetabolismCooldown;
        private static bool isControlCooldown;

        private static int boostTime;
        private static int boostCooldownTime;

        [SerializeField]
        private Image _speedCooldown;

        [SerializeField]
        private Image _metabolismCooldown;
        [SerializeField]
        private Image _controlCooldown;

        private static Image metabolismCooldown;
        private static Image speedCooldown;
        private static Image controlCooldown;

        [SerializeField]
        private TextMeshProUGUI _timerSpeed;

        [SerializeField]
        private TextMeshProUGUI _timerMetabolism;
        [SerializeField]
        private TextMeshProUGUI _timerControl;

        [Inject] 
        private ISoundManager _soundManager;

        private static TextMeshProUGUI timerSpeed;
        private static TextMeshProUGUI timerMetabolism;
        private static TextMeshProUGUI timerControl;

        public static int AdrenalineGlands
        {
            get => PlayerPrefs.GetInt("AdrenalineGlands", 0);
            set => PlayerPrefs.SetInt("AdrenalineGlands", value);
        }

        public static int FastMetabolism
        {
            get => PlayerPrefs.GetInt("FastMetabolism", 0);
            set => PlayerPrefs.SetInt("FastMetabolism", value);
        }
        public static int FullControl
        {
            get => PlayerPrefs.GetInt("FullControl", 0);
            set => PlayerPrefs.SetInt("FullControl", value);
        }
        private void Start()
        {
            Snake.Instance.SetBloomAmount(0);
            speedCooldown = _speedCooldown;
            metabolismCooldown = _metabolismCooldown;
            controlCooldown = _controlCooldown;
            timerSpeed = _timerSpeed;
            timerMetabolism = _timerMetabolism;
            timerControl = _timerControl;
        }
        public static void ResetProgress()
        {
            AdrenalineGlands = 0;
            FastMetabolism = 0;
            FullControl = 0;
        }
        
        public void UseSpeedBoost()
        {
            if (isBoostSpeedCooldown)
            {
                return;
            }
            
            _soundManager.PlayBoost();
            
            isBoostSpeed = true;
            isBoostSpeedCooldown = true;
            timerSpeed.gameObject.SetActive(true);
            TutorialManager.Instance.HideBoostHand();
            Instance.StartCoroutine(BoostSpeed(0));
            if (!isBoostMetabolism && !isFullControl)
                Instance.StartCoroutine(StartBloom());
        }
        
        public void UseControlBoost()
        {
            if (isControlCooldown)
            {
                return;
            }
            
            _soundManager.PlayBoost();
            
            isFullControl = true;
            isControlCooldown = true;
            timerControl.gameObject.SetActive(true);
            TutorialManager.Instance.HideBoostHand();
            Instance.StartCoroutine(BoostSpeed(2));
            if (!isBoostMetabolism && !isBoostSpeed)
                Instance.StartCoroutine(StartBloom());
        }

        private static IEnumerator StartBloom()
        {
            float i = 0;
            const float step = 0.01f;
            while (i < 0.05f)
            {
                yield return null;
                i += step;
                Snake.Instance.SetBloomAmount(i);
            }
        }

        private static IEnumerator StopBloom()
        {
            float i = 0.05f;
            const float step = 0.01f;
            while (i > 0)
            {
                yield return null;
                i -= step;
                Snake.Instance.SetBloomAmount(i);
            }
        }

        private static IEnumerator BoostSpeed(int boost)
        {
            int i = 0;
            while (i < boostTime)
            {
                i++;
                switch (boost)
                {
                    case 0:
                        speedCooldown.fillAmount = (float) i / boostTime;
                        timerSpeed.text = TimeSpan.FromSeconds(boostTime - i).ToString(@"mm\:ss");
                        break;
                    case 1:
                        metabolismCooldown.fillAmount = (float) i / boostTime;
                        timerMetabolism.text = TimeSpan.FromSeconds(boostTime - i).ToString(@"mm\:ss");
                        break;
                    case 2:
                        controlCooldown.fillAmount = (float)i / boostTime;
                        timerControl.text = TimeSpan.FromSeconds(boostTime - i).ToString(@"mm\:ss");
                        break;
                }

                yield return new WaitForSecondsRealtime(1f);
            }


            switch (boost)
            {
                case 0:
                    isBoostSpeed = false;
                    Instance.StartCoroutine(CooldownBoost(0));
                    break;
                case 1:
                    isBoostMetabolism = false;
                    Instance.StartCoroutine(CooldownBoost(1));
                    break;
                case 2:
                    isFullControl = false;
                    Instance.StartCoroutine(CooldownBoost(2));
                    break;
            }

            if (!isBoostSpeed && !isBoostMetabolism && !isFullControl)
            {
                Instance.StartCoroutine(StopBloom());
            }

        }

        private static IEnumerator CooldownBoost(int boost)
        {
            int i = 0;

            AdsManager.Instance.ShowInterstitial();
            while (i < boostCooldownTime)
            {
                i++;
                switch (boost)
                {
                    case 0:
                        timerSpeed.text = TimeSpan.FromSeconds(boostCooldownTime - i).ToString(@"mm\:ss");
                        speedCooldown.fillAmount = (float) (boostCooldownTime - i) / boostCooldownTime;
                        break;
                    case 1:
                        timerMetabolism.text = TimeSpan.FromSeconds(boostCooldownTime - i).ToString(@"mm\:ss");
                        metabolismCooldown.fillAmount = (float) (boostCooldownTime - i) / boostCooldownTime;
                        break;
                    case 2:
                        timerControl.text = TimeSpan.FromSeconds(boostCooldownTime - i).ToString(@"mm\:ss");
                        controlCooldown.fillAmount = (float) (boostCooldownTime - i) / boostCooldownTime;
                        break;
                }

                yield return new WaitForSecondsRealtime(1f);
            }

            switch (boost)
            {
                case 0:
                    isBoostSpeedCooldown = false;
                    timerSpeed.gameObject.SetActive(false);
                    break;
                case 1:
                    isBoostMetabolismCooldown = false;
                    timerMetabolism.gameObject.SetActive(false);
                    break;
                case 2:
                    isControlCooldown = false;
                    timerControl.gameObject.SetActive(false);
                    break;
            }
        }

        public void UseMetabolismBoost()
        {
            if (isBoostMetabolismCooldown)
            {
                return;
            }
            
            _soundManager.PlayBoost();
            
            isBoostMetabolism = true;
            isBoostMetabolismCooldown = true;
            timerMetabolism.gameObject.SetActive(true);
            TutorialManager.Instance.HideBoostHand();
            Instance.StartCoroutine(BoostSpeed(1));
            if (!isBoostSpeed && !isFullControl)
            {
                Instance.StartCoroutine(StartBloom());
            }

        }

        public static void LoadValues()
        {
            boostTime = (int) RemoteConfig.GetDouble("Boost_Length");
            boostCooldownTime = (int) RemoteConfig.GetDouble("Boost_Cooldown");
        }
    }
}