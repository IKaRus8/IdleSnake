using System.Collections.Generic;

namespace Firebase
{
    public static class DefaultValues
    {
        public static Dictionary<string, object> defaultValues = new()
        {
            {"Boost_Length",30},  
            {"Boost_Cooldown",360},
            {"Field_MaxSnakePercent",25},
            {"Field_MaxFoodPercent",25},
            {"Snake_Speed",0.5f},
            {"Snake_GrowthBase",2},
            {"Snake_GrowthMult",1.5f},
            {"Food_Start",1},
            {"Food_Percent",100},
            {"Evolution_Cost",15},
            {"Evolution_Percent",1.2f},
            {"Evolution_NewFood_Speed",10},
            {"Evolution_NewFood_Percent",0.1f},
            {"Ads_Rewarded_Level", 10},
            {"Start_Coin_her_hour",50}
        };
    }
}
