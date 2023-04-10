using System.Collections.Generic;
using Managers.Interfaces;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public class RatingManager : Singleton<RatingManager>
    {
        [SerializeField]
        private GameObject _ratingPrefab;

        [SerializeField]
        private Transform _parentRating;

        [Inject] 
        private ISnakeLevelProvider _snakeLevelProvider;

        private List<PlaceRating> _rating;

        private readonly string[] _nicknames = new[]
        {
            "DarK_Knigt", "Lemonk", "Crtal", "lou_Tep", "MrZadrot", "_LegenDa_", "Cemea", "Zigzag", "SmoKKeR",
            "DIVERSANT_", "K_I_N_G", "SkILLzAr", "MrGameFun", "leha2019",
            "Enigma", "Black_Wolf", "Good_Joker", "ReversAlpha", "GADZILO", "Cherry_Pie", "ByRaShKa", "Vito_Scaletta",
            "MaxTheGamer", "DELIRIOUSPLAY", "NyanCat", "RedHulk", "FireKitty", "Klaimmor", "EzzzBOX",
            "GhOsTiK", "MonAmour", "SkyDeaD", "DeMpDeeZ", "Enroxes", "SpeedBeast", "Howfaralice", "FlooMeer",
            "Mist(er)ror", "HouruSinkara", "BaRsIk", "RaVeNCroW", "DantASS", "Artcross", "DarkWolfMaster",
            "BallisticAmmo", "SkOrPiOnUs",
            "JopaTekilla", "NoNameForever", "EziosTaNKprrak", "DEAD__Е_Y_E", "DEADShotGunDEAD", "Mr.Bean", "Fox121",
            "lena6", "dontcry", "253255", "dady.cat", "565jsadfhhfd", "NoName", "Alex234", "NoGameNoLife", "Killer333"
        };

        [SerializeField]
        private Color[] _cupColors;

        private int _playerPlace;

        private void OnEnable()
        {
            int currentLevel = _snakeLevelProvider.CurrentLevelRx.Value;
            if (_rating == null)
            {
                bool isYouInRating = false;
                _rating = new List<PlaceRating>();
                int i = 1;
                for (int j = 300; j > 0; j -= 5, i++)
                {
                    if (currentLevel > j && !isYouInRating)
                    {
                        isYouInRating = true;
                        _playerPlace = i - 1;
                        _rating.Add(Instantiate(_ratingPrefab, _parentRating).GetComponent<PlaceRating>());
                        _rating[i - 1].Init("YOU!",
                            i.ToString(), currentLevel.ToString(), i > 3 ? Color.white : _cupColors[i - 1]);
                        i++;
                    }

                    _rating.Add(Instantiate(_ratingPrefab, _parentRating).GetComponent<PlaceRating>());
                    _rating[i - 1].Init(_nicknames[i - 1],
                        i.ToString(), j.ToString(), i > 3 ? Color.white : _cupColors[i - 1]);
                }

                if (isYouInRating) return;
                _playerPlace = i - 1;
                _rating.Add(Instantiate(_ratingPrefab, _parentRating).GetComponent<PlaceRating>());
                _rating[i - 1].Init("YOU!",
                    i.ToString(), currentLevel.ToString(), Color.white);
            }
            else
            {
                for (int i = 0; i < _rating.Count; i++)
                {
                    if (_playerPlace <= i || int.Parse(_rating[i].Score.text) >= currentLevel) continue;
                    _rating[_playerPlace].Init("YOU!",
                        i.ToString(), currentLevel.ToString(), i > 3 ? Color.white : _cupColors[i]);
                    (_rating[i], _rating[_playerPlace]) = (_rating[_playerPlace], _rating[i]);
                    _playerPlace = i;
                    _rating[i].transform.SetSiblingIndex(i);
                }
            }
            _parentRating.GetComponent<RectTransform>().localPosition = new Vector3(0,(770>(110*_playerPlace))?0:(110*_playerPlace-770),0);
        }
    }
}