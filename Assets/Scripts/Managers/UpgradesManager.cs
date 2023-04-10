using UnityEngine;

namespace Managers
{
    //TODO: bind as service
    public static class UpgradesManager
    {
        public static int AllCoins
        {
            get => PlayerPrefs.GetInt("AllCoins", 0);
            set => PlayerPrefs.SetInt("AllCoins", value);
        }
        public static int GreatEyes
        {
            get => PlayerPrefs.GetInt("GreatEyes", 0);
            set => PlayerPrefs.SetInt("GreatEyes", value);
        } 
        public static int FoodFinding
        {
            get => PlayerPrefs.GetInt("FoodFinding", 0);
            set => PlayerPrefs.SetInt("FoodFinding", value);
        }
        
        public static int MaxFood
        {
            get => PlayerPrefs.GetInt("MaxFood", 0);
            set => PlayerPrefs.SetInt("MaxFood", value);
        } 
        public static int SteelStomach
        {
            get => PlayerPrefs.GetInt("SteelStomach", 0);
            set => PlayerPrefs.SetInt("SteelStomach", value);
        } 
        public static int Pathfinding
        {
            get => PlayerPrefs.GetInt("Pathfinding", 0);
            set => PlayerPrefs.SetInt("Pathfinding", value);
        }
        public static int StrongMuscles
        {
            get => PlayerPrefs.GetInt("StrongMuscles", 0);
            set => PlayerPrefs.SetInt("StrongMuscles", value);
        }
       
        public static void Reset()
        {
            GreatEyes = 0;
            FoodFinding = 0;
            MaxFood = 0;
            SteelStomach = 0;
            Pathfinding = 0;
            StrongMuscles = 0;
            AllCoins = 0;
            UIManager.Instance.UpdateCoinValue();
        }
        public static bool OpenedAncestors
        {
            get => bool.Parse(PlayerPrefs.GetString("OpenedAncestors", false.ToString()));
            set => PlayerPrefs.SetString("OpenedAncestors", value.ToString());
        }
    }
}
