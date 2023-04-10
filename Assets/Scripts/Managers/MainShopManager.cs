using Managers.Interfaces;
using TMPro;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public class MainShopManager : Singleton<MainShopManager>
    {
        [SerializeField]
        private TextMeshProUGUI[] _costs;

        [SerializeField]
        private TextMeshProUGUI[] _currentValue;

        [SerializeField]
        private TextMeshProUGUI[] _level;

        [SerializeField]
        private GameObject[] _rewardButtons;
        private int _rewardCount=5;

        [Inject] 
        private ICurrencyManager _currencyManager;

        private static int _startCost = 15;
        private static float _multiplyCost = 1.2f;
        public static float newFoodPercent = 0.01f;
        private void Start()
        {
            int greatEyes = UpgradesManager.GreatEyes;
            int foodFinding = UpgradesManager.FoodFinding;
            int steelStomach = UpgradesManager.SteelStomach;
            int adrenalineGlands = BoostManager.AdrenalineGlands;
            int pathfinding = UpgradesManager.Pathfinding;
            int strongMuscles = UpgradesManager.StrongMuscles;
            int fastMetabolism = BoostManager.FastMetabolism;
            _costs[0].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, greatEyes))).ToString();
            _costs[1].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, foodFinding))).ToString();
            _costs[2].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, steelStomach))).ToString();
            _costs[3].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, adrenalineGlands))).ToString();
            _costs[4].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, pathfinding))).ToString();
            _costs[5].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, strongMuscles))).ToString();
            _costs[6].text = ((int)(_startCost * Mathf.Pow(_multiplyCost, fastMetabolism))).ToString();
            //_currentValue[0].text = greatEyes == 0 ? "0 sec" : (greatEyes == 1) ? (FieldManager.appleCooldown.ToString("f2")+" sec"): $"{FieldManager.appleCooldown - FieldManager.appleCooldown*Mathf.Pow(newFoodPercent, greatEyes-1) :f2} sec";
            _currentValue[1].text = foodFinding == 0 ? "0%" : $"{foodFinding}%";
            _currentValue[2].text = $"{1 + steelStomach}";
            _currentValue[3].text = adrenalineGlands == 0 ? "0%" : $"{100 + 10 * (adrenalineGlands)}%";
            _currentValue[4].text = $"{6 + pathfinding }x{6 + pathfinding }";
            _currentValue[5].text = strongMuscles == 0 ? "0" : $"{1 + 0.1 * (strongMuscles - 1)}";
            _currentValue[6].text = $"{300 + 10 * (fastMetabolism)}%";
            _level[0].text = greatEyes + " lvl";
            _level[1].text = foodFinding + " lvl";
            _level[2].text = steelStomach + " lvl";
            _level[3].text = adrenalineGlands + " lvl";
            _level[4].text = pathfinding + " lvl";
            _level[5].text = strongMuscles + " lvl";
            _level[6].text = fastMetabolism + " lvl";
        }
        public void BuyGreatEyes()
        {
            _currentValue[0].text =
                FieldManager.appleCooldown + " sec";
        }

        public void UpgradeGreatEyes()
        {
            int greatEyes = UpgradesManager.GreatEyes;
            int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, greatEyes));
            
            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(-cost);
            //UpgradesManager.AllCoins -= cost;
            UpgradesManager.GreatEyes = 1 + greatEyes;
            UIManager.Instance.UpdateCoinValue();
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, greatEyes + 1));
            _costs[0].text = cost.ToString();
            _level[0].text = (greatEyes + 1).ToString() + " lvl";
            FieldManager.Instance.UpgradeAppleCooldown();
            _currentValue[0].text = $"{FieldManager.appleCooldown:f2} sec";
            if (((greatEyes + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[0].SetActive(true);
        }

        public void UpgradeGreatEyesReward()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int greatEyes = UpgradesManager.GreatEyes;
                UpgradesManager.GreatEyes = 1 + greatEyes;
                UIManager.Instance.UpdateCoinValue();
                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, greatEyes + 1));
                _costs[0].text = cost.ToString();
                _level[0].text = (greatEyes + 1) + " lvl";
                FieldManager.Instance.UpgradeAppleCooldown();
                _currentValue[0].text = $"{FieldManager.appleCooldown:f2} sec";
                _rewardButtons[0].SetActive(false);
            });
        }


        public void UpgradeFoodFinding()
        {
            int foodFinding = UpgradesManager.FoodFinding;
            int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, foodFinding));

            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            
            _currencyManager.ChangeFood(- cost);
            
            //UpgradesManager.AllCoins -= cost;
            UpgradesManager.FoodFinding = 1 + foodFinding;

            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, foodFinding + 1));
            _currentValue[1].text = $"{1 + foodFinding }%";
            _costs[1].text = cost.ToString();
            _level[1].text = (foodFinding + 1) + " lvl";
            //UIManager.Instance.UpdateCoinValue();
            FieldManager.Instance.UpgradeFoodFinding();
            if (((foodFinding + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[1].SetActive(true);
        }

        public void UpgradeFoodFindingReward()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int foodFinding = UpgradesManager.FoodFinding;
                UpgradesManager.FoodFinding = 1 + foodFinding;

                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, foodFinding + 1));
                _currentValue[1].text = $"{1 + foodFinding }%";
                _costs[1].text = cost.ToString();
                _level[1].text = (foodFinding + 1) + " lvl";
                //UIManager.Instance.UpdateCoinValue();
                FieldManager.Instance.UpgradeFoodFinding();
                _rewardButtons[1].SetActive(false);
            });
        }

        public void UpgradeSteelStomach()
        {
            var steelStomach = UpgradesManager.SteelStomach;
            var cost = (int)(_startCost * Mathf.Pow(_multiplyCost, steelStomach));

            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(- cost);
            //UpgradesManager.AllCoins -= cost;
            
            UpgradesManager.SteelStomach = 1 + steelStomach;
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, steelStomach + 1));
            _currentValue[2].text = $"{LevelGrowManager.baseGrowForFood + (steelStomach + 1) * LevelGrowManager.upGrowForFood }";
            _costs[2].text = cost.ToString();
            _level[2].text = (steelStomach + 1) + " lvl";
            //UIManager.Instance.UpdateCoinValue();
            FieldManager.Instance.UpgradeSteelStomach();
            if (((steelStomach + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[2].SetActive(true);
        }
        public void UpgradeSteelStomachReward()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int steelStomach = UpgradesManager.SteelStomach;
                UpgradesManager.SteelStomach = 1 + steelStomach;
                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, steelStomach + 1));
                _currentValue[2].text = $"{LevelGrowManager.baseGrowForFood + (steelStomach + 1) * LevelGrowManager.upGrowForFood }";
                _costs[2].text = cost.ToString();
                _level[2].text = (steelStomach + 1) + " lvl";
                UIManager.Instance.UpdateCoinValue();
                FieldManager.Instance.UpgradeSteelStomach();
                _rewardButtons[2].SetActive(false);
            });
        }

        public void UpgradeAdrenalineGlands()
        {
            var adrenalineGlands = BoostManager.AdrenalineGlands;
            var cost = (int)(_startCost * Mathf.Pow(_multiplyCost, adrenalineGlands));

            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(- cost);
            //UpgradesManager.AllCoins -= cost;
            BoostManager.AdrenalineGlands = adrenalineGlands + 1;
            UIManager.Instance.UpdateCoinValue();
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, adrenalineGlands + 1));
            _currentValue[3].text = $"{100 + 10 * (adrenalineGlands + 1)}%";
            _costs[3].text = cost.ToString();
            _level[3].text = adrenalineGlands + 1 + " lvl";
            FieldManager.Instance.UpgradeBoostSpeed();
            if (((adrenalineGlands + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[3].SetActive(true);
        }

        public void UpgradeAdrenalineGlandsReward()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {

                var adrenalineGlands = BoostManager.AdrenalineGlands;
                BoostManager.AdrenalineGlands = adrenalineGlands + 1;
                UIManager.Instance.UpdateCoinValue();
                var cost = (int)(_startCost * Mathf.Pow(_multiplyCost, adrenalineGlands + 1));
                _currentValue[3].text = $"{100 + 10 * (adrenalineGlands + 1)}%";
                _costs[3].text = cost.ToString();
                _level[3].text = adrenalineGlands + 1+ " lvl";
                FieldManager.Instance.UpgradeBoostSpeed();
                _rewardButtons[3].SetActive(false);
            });
        }

        public void UpgradePathfinding()
        {
            int pathfinding = UpgradesManager.Pathfinding;
            int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, pathfinding));

            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(-cost);
            //UpgradesManager.AllCoins -= cost;
            UpgradesManager.Pathfinding += 1;
            _currentValue[4].text = $"{6 + pathfinding + 1}x{6 + pathfinding + 1}";
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, pathfinding + 1));
            _costs[4].text = cost.ToString();
            _level[4].text = pathfinding + 1 + " lvl";
            UIManager.Instance.UpdateCoinValue();
            FieldManager.Instance.ExpandField(FieldManager.Instance.FieldSize + 1);
            if (((pathfinding + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[4].SetActive(true);
        }

        public void UpgradePathfindingFree()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int pathfinding = UpgradesManager.Pathfinding;
                UpgradesManager.Pathfinding += 1;
                _currentValue[4].text = $"{6 + pathfinding + 1}x{6 + pathfinding + 1}";
                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, pathfinding + 1));
                _costs[4].text = cost.ToString();
                _level[4].text = pathfinding + 1+ " lvl";
                UIManager.Instance.UpdateCoinValue();
                FieldManager.Instance.ExpandField(FieldManager.Instance.FieldSize + 1);
                _rewardButtons[4].SetActive(false);
            });
        }
        public void UpgradeStrongMuscles()
        {
            int strongMuscles = UpgradesManager.StrongMuscles;
            int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, strongMuscles));
            
            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(-cost);
            //UpgradesManager.AllCoins -= cost;
            UpgradesManager.StrongMuscles = strongMuscles + 1;
            _currentValue[5].text = $"{1 + 0.1 * (strongMuscles)}";
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, strongMuscles + 1));
            _costs[5].text = cost.ToString();
            _level[5].text = strongMuscles + 1 + " lvl";
            UIManager.Instance.UpdateCoinValue();
            FieldManager.Instance.UpgradeStrongMuscles(strongMuscles + 1);
            if (((strongMuscles + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[5].SetActive(true);
        }

        public void UpgradeStrongMusclesFree()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int strongMuscles = UpgradesManager.StrongMuscles;
                UpgradesManager.StrongMuscles = strongMuscles + 1;
                _currentValue[5].text = $"{1 + 0.1 * (strongMuscles)}";
                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, strongMuscles + 1));
                _costs[5].text = cost.ToString();
                _level[5].text = strongMuscles + 1 + " lvl";
                UIManager.Instance.UpdateCoinValue();
                FieldManager.Instance.UpgradeStrongMuscles(strongMuscles + 1);
                _rewardButtons[5].SetActive(false);
            });
        }
        public void UpgradeFastMetabolism()
        {
            int fastMetabolism = BoostManager.FastMetabolism;
            int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, fastMetabolism));
            
            if (_currencyManager.FoodPointsRx.Value < cost)
            {
                return;
            }
            _currencyManager.ChangeFood(-cost);
            //UpgradesManager.AllCoins -= cost;
            BoostManager.FastMetabolism = fastMetabolism + 1;
            cost = (int)(_startCost * Mathf.Pow(_multiplyCost, fastMetabolism + 1));
            _currentValue[6].text = $"{300 + 10 * (fastMetabolism)}%";
            _costs[6].text = cost.ToString();
            _level[6].text = fastMetabolism + 1+ " lvl";
            UIManager.Instance.UpdateCoinValue();
            FieldManager.Instance.UpgradeBoostMetabolism();
            if (((fastMetabolism + 1) % _rewardCount) == 0 && !IAPManager.IsSubscribeEnable)
                _rewardButtons[6].SetActive(true);
        }
        public void UpgradeFastMetabolismFree()
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                int fastMetabolism = BoostManager.FastMetabolism;
                BoostManager.FastMetabolism = fastMetabolism + 1;
                int cost = (int)(_startCost * Mathf.Pow(_multiplyCost, fastMetabolism + 1));
                _currentValue[6].text = $"{300 + 10 * (fastMetabolism)}%";
                _costs[6].text = cost.ToString();
                _level[6].text = fastMetabolism + 1 + " lvl";
                UIManager.Instance.UpdateCoinValue();
                FieldManager.Instance.UpgradeBoostMetabolism();
                _rewardButtons[6].SetActive(false);
            });
        }

        public void LoadValue()
        {
            _startCost = (int)RemoteConfig.GetDouble("Evolution_Cost");
            _multiplyCost = (float)RemoteConfig.GetDouble("Evolution_Percent");
            _rewardCount = (int)RemoteConfig.GetDouble("Ads_Rewarded_Level");
            FieldManager.appleCooldown = (float)RemoteConfig.GetDouble("Evolution_NewFood_Speed");
            newFoodPercent = (float)RemoteConfig.GetDouble("Evolution_NewFood_Percent") / 100f;
            for (int i = 0; i < UpgradesManager.GreatEyes - 1; i++)
            {
                FieldManager.Instance.UpgradeAppleCooldown();
            }
            _currentValue[0].text = FieldManager.appleCooldown.ToString("f2") + " sec";

            int greatEyes = UpgradesManager.GreatEyes;
            int foodFinding = UpgradesManager.FoodFinding;
            int steelStomach = UpgradesManager.SteelStomach;
            int adrenalineGlands = BoostManager.AdrenalineGlands;
            int pathfinding = UpgradesManager.Pathfinding;
            int strongMuscles = UpgradesManager.StrongMuscles;
            int fastMetabolism = BoostManager.FastMetabolism;
            if (!IAPManager.IsSubscribeEnable)
            {
                _rewardButtons[0].SetActive((greatEyes > 0) && ((greatEyes % _rewardCount) == 0));
                _rewardButtons[1].SetActive((foodFinding > 0) && ((foodFinding % _rewardCount) == 0));
                _rewardButtons[2].SetActive((steelStomach > 0) && ((steelStomach % _rewardCount) == 0));
                _rewardButtons[3].SetActive((adrenalineGlands > 0) && ((adrenalineGlands % _rewardCount) == 0));
                _rewardButtons[4].SetActive((pathfinding > 0) && ((pathfinding % _rewardCount) == 0));
                _rewardButtons[5].SetActive((strongMuscles > 0) && ((strongMuscles % _rewardCount) == 0));
                _rewardButtons[6].SetActive((fastMetabolism > 0) && ((fastMetabolism % _rewardCount) == 0));
            }
        }
        public void HideRewardButtons()
        {
            foreach(var button in _rewardButtons)
            {
                button.SetActive(false);
            }
        }
    }
}