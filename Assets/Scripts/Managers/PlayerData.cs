using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Managers
{
    public static class PlayerData
    {
        private static int _countRequestsEp;
        private static int _countRequestsD;
        private static int _countRequestsA;

        private static int _evolvePoint;
        public static int EvolvePoint
        {
            get
            {
                if (_countRequestsEp != 0) return _evolvePoint;
                _countRequestsEp = 1;
                _evolvePoint = SaveManager.LoadindData("EvolvePoint", _evolvePoint);
                return _evolvePoint;
            }
            set
            {
                SaveManager.SavingData("EvolvePoint", value);
                _evolvePoint = value;
            }
        }

        private static int _diamond;
        public static int Diamond
        {
            get
            {
                if (_countRequestsD != 0)
                {
                    return _diamond;
                }
                
                _countRequestsD = 1;
                _diamond = SaveManager.LoadindData("Diamond", _diamond);
                return _diamond;
            }
            set
            {
                SaveManager.SavingData("Diamond", value);
                _diamond = value;
                UIManager.Instance.UpdateDiamondValue();
            }
        }

        private static int _ancestor;
        public static int Ancestor
        {
            get
            {
                if (_countRequestsA != 0) return _ancestor;
                _countRequestsA = 1;
                _ancestor = SaveManager.LoadindData("Ancestor", _ancestor);
                return _ancestor;
            }
            set
            {
                SaveManager.SavingData("Ancestor", value);
                _ancestor = value;
                UIManager.Instance.UpdateAncestorValue();
            }
        }
    }   
}
