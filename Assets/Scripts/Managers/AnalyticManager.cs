using UnityEngine;
using Firebase.Analytics;

namespace Managers
{

    public static class AnalyticManager
    {
        public static void SavePlayerMoney(int money)
        {
            FirebaseAnalytics.SetUserProperty("Coins", money.ToString());
        }

        #region Snake

        public static void Snake_Growth()
        {
            string eventString ="Snake_Growth";
            Debug.Log(eventString);
            FirebaseAnalytics.LogEvent(eventString);
        }

        public static void Snake_Growth_Size(int size)
        {
            string eventString = $"Snake_Growth_{size:d3}";
            Debug.Log(eventString);
            FirebaseAnalytics.LogEvent(eventString);
        }

        public static void Snake_Modification(string modification)
        {
            string eventString = "Snake_Modification";
            Debug.Log(eventString);
            FirebaseAnalytics.LogEvent(eventString, new Parameter("Modification", modification));
        } 
        
        public static void Snake_Ancestor_Modification(string modification)
        {

            const string eventString = "Snake_Ancestor_Modification";
            Debug.Log(eventString);
            FirebaseAnalytics.LogEvent(eventString, new Parameter("Modification", modification));
        }

        #endregion
        #region Field

        public static void Field_Expand()
        {
            const string eventString = "Field_Expand";
            Debug.Log(eventString);
            FirebaseAnalytics.LogEvent(eventString);
        }

        #endregion
    }
}