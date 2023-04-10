using System.Collections;
using System.Collections.Generic;
using Managers.Interfaces;
using Signals;
using UI.Interfaces;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public class FieldManager : Singleton<FieldManager>, IAppleSpawner
    {
        [Header("Popups"), SerializeField]
        private GameObject _expandFieldPopup;

        [Header("Field obj"), SerializeField]
        private Transform _fieldParent;

        [SerializeField]
        private GameObject _cellPrefab;

        [Header("Apple Properties"), SerializeField]
        private GameObject _applePrefab;

        [SerializeField]
        private GameObject _pineapplePrefab;

        [SerializeField]
        private GameObject _diamondPrefab;

        [SerializeField]
        private GameObject[] _rockPrefab;

        [SerializeField]
        private GameObject[] _logPrefab;

        [SerializeField]
        private GameObject _appleParent;

        public static float appleCooldown = 10;
        private const float GoodCooldown = 10;

        [SerializeField]
        private float _appleMax;

        [SerializeField]
        private float _goodMax;

        [Header("Properties"), SerializeField]
        private float _cellSize;

        [SerializeField]
        private int _fieldSize;

        private static bool isSnakeStan;
        public static GameObject SnakeStanObj;
        public int FieldSize => _fieldSize;

        [SerializeField]
        private List<Color> _colors = new();

        private ISnakeLevelProvider _snakeLevelProvider;
        private SignalBus _signalBus;

        private int _currentColor;
        public int _maxFoodParameter = 10;
        private List<List<Cell>> _cellList = new();

        public List<Vector2Int> _cellSnake = new();

        private List<Apple> _appleList = new();
        private List<Diamond> _diamondList = new();
        private List<Good> _goodList = new();

        private int _foodFinding;

        private int _pointForFood;

        public static float speedSnakeForTime = 0.5f;

        private int _speedBoost;

        private int _metabolismBoost;

        private Camera _mainCamera;

        public static float percentMaxFood = 0.25f;
        public static float percentMaxGood = 0.25f;

        private static Vector2Int targetCell;

        private static bool isMoveToTarget;

        [Inject]
        private void Construct(
            SignalBus signalBus,
            ISnakeLevelProvider snakeLevelProvider)
        {
            _signalBus = signalBus;
            _snakeLevelProvider = snakeLevelProvider;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            
            InitializeField();
            StartCoroutine(InitializeSnakeOnField());
            
            _foodFinding = UpgradesManager.FoodFinding;
            _pointForFood = UpgradesManager.SteelStomach;
            
            StartCoroutine(MovementProcess());
            StartCoroutine(AppleSpawnProcess());
            StartCoroutine(GoodsSpawnProcess());
            
            ExpandField(FieldSize + UpgradesManager.Pathfinding);
            
            _speedBoost = BoostManager.AdrenalineGlands;
            _metabolismBoost = BoostManager.FastMetabolism;
            
            for (var i = 0; i < UpgradesManager.StrongMuscles; i++)
            {
                UpgradeStrongMuscles(i);
            }
            
            _signalBus.Fire<GameStartedSignal>();
        }

        public void InitializeField()
        {
            if (isSnakeStan)
            {
                Snake.Instance.SnakeStopStan();
                isSnakeStan = false;
            }

            _currentColor = 0;
            _cellList = new List<List<Cell>>();
            for (int i = _fieldParent.childCount; i > 0; i--)
            {
                Destroy(_fieldParent.GetChild(i - 1).gameObject);
            }

            foreach (var apple in _appleList)
            {
                Destroy(apple.appleObject);
            }

            _appleList = new List<Apple>();

            foreach (var apple in _goodList)
            {
                Destroy(apple.goodObject);
            }

            _goodList = new List<Good>();
            foreach (var apple in _diamondList)
            {
                Destroy(apple.Object);
            }

            _diamondList = new List<Diamond>();

            float startPosition = -(float) _fieldSize / 2 * _cellSize;

            for (int i = _fieldSize - 1; i >= 0; i--)
            {
                _cellList.Add(new List<Cell>());
                if (_fieldSize % 2 == 0) _currentColor = (_colors.Count - 1 == _currentColor) ? 0 : _currentColor + 1;
                for (int j = _fieldSize - 1; j >= 0; j--)
                {
                    GameObject cellObject = Instantiate(_cellPrefab, _fieldParent, true);
                    cellObject.transform.position = new Vector3(startPosition + (_fieldSize - 1 - i + 0.5f) * _cellSize,
                        startPosition + (_fieldSize - 1 - j + 0.5f) * (_cellSize), 1);
                    cellObject.name = $"Cell[{i}][{j}]";
                    Cell cell = cellObject.GetComponent<Cell>();
                    cell.InitDefault();

                    cell.cellSprite.color = _colors[_currentColor];
                    _currentColor = (_colors.Count - 1 == _currentColor) ? 0 : _currentColor + 1;
                    cell.cellObject = cellObject;
                    _cellList[_fieldSize - 1 - i].Add(cell);
                }
            }
        }

        public IEnumerator InitializeSnakeOnField()
        {
            for (int i = Snake.Instance.Segments.Count - 1; i >= 0; i--)
            {
                Snake.Segment segment = Snake.Instance.Segments[i];
                Snake.Instance.Segments.Remove(segment);
                Destroy(segment.segmentTransform.gameObject);
            }

            _cellSnake = new List<Vector2Int>();
            Vector2Int startId = new Vector2Int(_fieldSize / 2, _fieldSize / 2);
            Snake.Instance.Head.position = _cellList[startId.x][startId.y].cellObject.transform.position;
            _cellSnake.Add(startId);
            Vector2Int direction = PathFinder.RandomDirection();
            Vector2Int target = -direction;
            Vector2Int nextCell = startId + direction;
            startId = nextCell;


            foreach (var segment in Snake.Instance.Segments)
            {
                segment.segmentTransform.position =
                    _cellList[startId.x][startId.y].cellObject.transform.position;
                _cellSnake.Add(startId);
                while (_cellSnake.Contains(nextCell))
                {
                    direction = PathFinder.RandomDirection();
                    nextCell = new Vector2Int(Mathf.Clamp(startId.x + direction.x, 0, _fieldSize - 1),
                        Mathf.Clamp(startId.y + direction.y, 0, _fieldSize - 1));
                }

                startId = nextCell;
            }

            startId = _cellSnake[0];
            yield return null;
            Snake.Instance.UpdateSnakeSprites(_cellList[startId.x + target.x][startId.y + target.y].cellObject.transform
                .position);
            //Snake.Instance.UpdateSnakeSprites(_cellList[startId.x + target.x][startId.y + target.y].cellObject.transform
            //    .position);
        }

        public void MoveSnakeDirection(Vector2Int direction)
        {
            Vector2Int nextCell = _cellSnake[0] + direction;
            nextCell = new Vector2Int(Mathf.Clamp(nextCell.x, 0, _fieldSize - 1),
                Mathf.Clamp(nextCell.y, 0, _fieldSize - 1));
            if (nextCell == _cellSnake[0] || nextCell == _cellSnake[1]) return;
            Snake.Instance.MoveToTarget(_cellList[nextCell.x][nextCell.y].cellObject.transform.position);
            _cellSnake.Insert(0, nextCell);
            _cellSnake.RemoveAt(_cellSnake.Count - 1);
        }

        public void StartMoveDirection(Cell cell)
        {
            var x = _cellList.IndexOf(_cellList.Find(x => x.Find(y => y == cell)));
            
            var vectorCell = new Vector2Int(x, _cellList[x].IndexOf(_cellList[x].Find(cellX => cellX == cell)));
            if (vectorCell == _cellSnake[0])
            {
                return;
            }

            isMoveToTarget = true;
            targetCell = vectorCell;
        }
        
        private void MoveSnakeId(Vector2Int cell)
        {
            if (cell == _cellSnake[0])
            {
                return;
            }

            var nextCell = _cellSnake[0] + PathFinder.DirectionToCell(cell);
            if (nextCell == targetCell)
            {
                isMoveToTarget = false;
            }
            
            var foundGood = _goodList.Find(x => x.goodId == nextCell);
                
            if (foundGood != null)
            {
                Snake.Instance.SnakeStartStan();
                isSnakeStan = true;
                SnakeStanObj = foundGood.goodObject;
                foundGood.goodObject.GetComponent<GoodCell>().StartStan();
                TutorialManager.Instance.ShowDestroyHand();
                return;
            }

            CheckItems(nextCell);

            Snake.Instance.MoveToTarget(_cellList[nextCell.x][nextCell.y].cellObject.transform.position);
            _cellSnake.Insert(0, nextCell);
            _cellSnake.RemoveAt(_cellSnake.Count - 1);
        }

        public void RemoveGood(GameObject thing)
        {
            var thingCurrent = _goodList.Find(x => x.goodObject == thing);
            if (SnakeStanObj == thing)
            {
                Snake.Instance.SnakeStopStan();
                isSnakeStan = false;
            }

            _goodList.Remove(thingCurrent);
        }

        public void ShowPopup()
        {
            _expandFieldPopup.SetActive(true);
        }

        public void OnCloseExpandPopup()
        {
            _expandFieldPopup.SetActive(false);
            ExpandField(6 + UpgradesManager.Pathfinding);
        }

        private void MoveSnakeRandom()
        {
            var maxCell = _fieldSize - 1;
            
            var direction = PathFinder.RandomDirection();
            var nextCell = _cellSnake[0] + direction;
            
            nextCell = new Vector2Int(
                Mathf.Clamp(nextCell.x, 0, maxCell),
                Mathf.Clamp(nextCell.y, 0, maxCell));
            
            while (nextCell == _cellSnake[0] || nextCell == _cellSnake[1])
            {
                direction = PathFinder.RandomDirection();
                
                nextCell = new Vector2Int(
                    Mathf.Clamp(_cellSnake[0].x + direction.x, 0, maxCell),
                    Mathf.Clamp(_cellSnake[0].y + direction.y, 0, maxCell));
            }

            var foundGood = _goodList.Find(x => x.goodId == nextCell);
                
            if (foundGood != null)
            {
                Snake.Instance.SnakeStartStan();
                isSnakeStan = true;
                SnakeStanObj = foundGood.goodObject;
                foundGood.goodObject.GetComponent<GoodCell>().StartStan();
                TutorialManager.Instance.ShowDestroyHand();
                return;
            }

            CheckItems(nextCell);

            Snake.Instance.MoveToTarget(_cellList[nextCell.x][nextCell.y].cellObject.transform.position);
            
            _cellSnake.Insert(0, nextCell);
            _cellSnake.RemoveAt(_cellSnake.Count - 1);
        }

        private void CheckItems(Vector2Int nextCell)
        {
            var foundApple = _appleList.Find(x => x.appleId == nextCell);

            if (foundApple != null)
            {
                Destroy(foundApple.appleObject);
                _appleList.Remove(foundApple);

                Debug.Log($"Removed apple! Apple list count :{_appleList.Count}");

                UIManager.Instance.UpdateFoodValue($"{_appleList.Count}/{Mathf.Floor(_appleMax - 1)}");

                var fruitValue = (int) ((foundApple.point * LevelGrowManager.baseGrowForFood +
                                         LevelGrowManager.upGrowForFood * _pointForFood) *
                                        (BoostManager.isBoostMetabolism
                                            ? (3 + (_metabolismBoost - 1) * 0.1f)
                                            : 1));
                
                _snakeLevelProvider.EatApple(fruitValue, foundApple.fruitID);
            }
            else
            {
                var foundDiamond = _diamondList.Find(x => x.Id == nextCell);

                if (foundDiamond != null)
                {
                    Destroy(foundDiamond.Object);
                    _diamondList.Remove(foundDiamond);
                    PlayerData.Diamond += 1;
                }
            }
        }


        //private void MoveEnemyRandom(Enemy enemy)
        //{
        //    Vector2Int direction = PathFinder.RandomDirection();
        //    Vector2Int nextCell = enemy.cell + direction;
        //    nextCell = new Vector2Int(Mathf.Clamp(nextCell.x, 0, _fieldSize - 1),
        //        Mathf.Clamp(nextCell.y, 0, _fieldSize - 1));
        //    while (nextCell == enemy.cell)
        //    {
        //        direction = PathFinder.RandomDirection();
        //        nextCell = new Vector2Int(Mathf.Clamp(_cellSnake[0].x + direction.x, 0, _fieldSize - 1),
        //            Mathf.Clamp(_cellSnake[0].y + direction.y, 0, _fieldSize - 1));
        //    }


        //    Apple foundApple = _appleList.Find(x => x.appleId == nextCell);

        //    if (foundApple != null)
        //    {
        //        Destroy(foundApple.appleObject);
        //        _appleList.Remove(foundApple);
        //        UIManager.Instance.UpdateFoodValue($"{_appleList.Count}/{Mathf.Floor(_appleMax - 1)}");
        //    }
        //    else
        //    {
        //        Diamond foundDiamond = _diamondList.Find(x => x.Id == nextCell);
        //        if (foundDiamond != null)
        //        {
        //            Destroy(foundDiamond.Object);
        //            _diamondList.Remove(foundDiamond);
        //        }

        //        Good foundGood = _goodList.Find(x => x.goodId == nextCell);
        //        if (foundGood != null)
        //        {
        //            Snake.Instance.SnakeStartStan();
        //            isSnakeStan = true;
        //            SnakeStanObj = foundGood.goodObject;
        //            return;
        //        }
        //    }


        //    Snake.Instance.MoveToTarget(_cellList[nextCell.x][nextCell.y].cellObject.transform.position);
        //    _cellSnake.Insert(0, nextCell);
        //    _cellSnake.RemoveAt(_cellSnake.Count - 1);
        //}

        public void SpawnRandomApple()
        {
            if (_appleList.Count >= Mathf.Floor(_appleMax - 1))
            {
                return;
            }
            
            var id = new Vector2Int(Random.Range(0, _fieldSize), Random.Range(0, _fieldSize));

            while (_cellSnake.IndexOf(id) != -1
                   || _appleList.Find(x => x.appleId == id) != null
                   || _goodList.Find(x => x.goodId == id) != null)
            {
                id = new Vector2Int(Random.Range(0, _fieldSize), Random.Range(0, _fieldSize));
            }
            
            if (AncestorsManager.grades.isOpen[4])
            {
                if (Random.Range(0, 100) < 2)
                {
                    SpawnDiamond(id);
                    return;
                }
            }

            SpawnApple(id);
            UIManager.Instance.UpdateFoodValue($"{_appleList.Count}/{Mathf.Floor(_appleMax - 1)}");
        }

        private void SpawnRandomGood()
        {
            Vector2Int id = new Vector2Int(Random.Range(0, FieldSize - 1), Random.Range(0, 6));
            while (_cellSnake.IndexOf(id) != -1 || _appleList.Find(x => x.appleId == id) != null ||
                   _goodList.Find(x => x.goodId == id) != null)
                id = new Vector2Int(Random.Range(0, FieldSize - 1), Random.Range(0, 6));
            SpawnGood(id);
        }

        private void SpawnDiamond(Vector2Int id)
        {
            GameObject obj = Instantiate(_diamondPrefab, _appleParent.transform, true);
            Diamond diamond = new Diamond
            {
                Object = obj,
            };
            obj.transform.position = (Vector2) _cellList[id.x][id.y].cellObject.transform.position;
            diamond.Id = id;
            _diamondList.Add(diamond);
        }

        private void SpawnApple(Vector2Int id)
        {
            bool isPineapple = Random.Range(0, 1.0f) < _foodFinding * 0.01f;
            GameObject obj = Instantiate(!isPineapple ? _applePrefab : _pineapplePrefab, _appleParent.transform, true);
            Apple apple = new Apple
            {
                appleObject = obj,
                point = isPineapple ? 10 : 1,
                fruitID = isPineapple ? 1 : 0
            };
            obj.transform.position = (Vector2) _cellList[id.x][id.y].cellObject.transform.position;
            apple.appleId = id;
            _appleList.Add(apple);
        }

        private void SpawnGood(Vector2Int id)
        {
            bool isRock = Random.Range(0, 1.0f) < 0.5f;

            int index = Random.Range(0, isRock ? _rockPrefab.Length : _logPrefab.Length);

            GameObject obj = Instantiate(isRock ? _rockPrefab[index] : _logPrefab[index], _appleParent.transform, true);
            Good good = new Good
            {
                goodObject = obj,
            };
            obj.GetComponent<GoodCell>().InitDefault();
            obj.transform.position = (Vector2) _cellList[id.x][id.y].cellObject.transform.position;
            good.goodId = id;
            _goodList.Add(good);
        }

        private IEnumerator MovementProcess()
        {
            while (true)
            {
                if (Snake.Instance.Segments.Count < _snakeLevelProvider.CurrentLevelRx.Value)
                {
                    Snake.Instance.AddSegment();
                }
                
                if (!isSnakeStan)
                {
                    if (isMoveToTarget)
                    {
                        MoveSnakeId(targetCell);
                    }
                    else
                    {
                        MoveSnakeRandom();
                    }
                }
                
                yield return new WaitForSecondsRealtime(speedSnakeForTime 
                                                        / (BoostManager.isBoostSpeed 
                    ? 2 + (_speedBoost - 1) * 0.1f 
                    : 1));
            }
        }

        private IEnumerator AppleSpawnProcess()
        {
            while (true)
            {
                if (UpgradesManager.GreatEyes > 0 && _appleList.Count < _appleMax - 1)
                {
                    SpawnRandomApple();
                }
                
                yield return new WaitForSecondsRealtime(appleCooldown);
            }
        }

        private IEnumerator GoodsSpawnProcess()
        {
            while (true)
            {
                if (AncestorsManager.grades.isOpen[2] && _goodList.Count < _goodMax - 1)
                {
                    SpawnRandomGood();
                }
                
                yield return new WaitForSecondsRealtime(GoodCooldown);
            }
        }

        private void NewMaxFood(float count = 0.01f)
        {
            _appleMax = _fieldSize * _fieldSize * (percentMaxFood + count);
            _goodMax = _fieldSize * _fieldSize * (percentMaxGood);
            
            UIManager.Instance.UpdateFoodValue($"{_appleList.Count}/{Mathf.Floor(_appleMax - 1)}");
            
            if (UpgradesManager.MaxFood >= _maxFoodParameter)
            {
                IAPManager.Instance.CurrentMaxFood();
            }
        }

        public void BuyNewMaxFood()
        {
            if (PlayerData.Diamond < 20)
            {
                return;
            }
            
            PlayerData.Diamond -= 20;
            UpgradesManager.MaxFood++;
            
            NewMaxFood(UpgradesManager.MaxFood / 100f);
        }

        public void ExpandField(int newSize)
        {
            _fieldSize = newSize;
            _mainCamera.orthographicSize = 3.3f + (_fieldSize - 6) * 0.5f;
            _mainCamera.transform.position = new Vector3(0, -1 - (_fieldSize - 6) * 0.15f, -10);
            float startPosition = -_fieldSize / 2f * _cellSize;
            NewMaxFood(UpgradesManager.MaxFood / 100f);
            for (int i = _fieldSize - 1; i >= 0; i--)
            {
                if (_cellList.Count < _fieldSize) _cellList.Add(new List<Cell>());
                if (_fieldSize % 2 == 0) _currentColor = (_colors.Count - 1 == _currentColor) ? 0 : _currentColor + 1;
                for (int j = _fieldSize - 1; j >= 0; j--)
                {
                    if (_cellList[_fieldSize - 1 - i].Count < _fieldSize)
                    {
                        GameObject cellObject = Instantiate(_cellPrefab, _fieldParent, true);
                        Cell cell = cellObject.GetComponent<Cell>();
                        cell.InitDefault();

                        _cellList[_fieldSize - 1 - i].Add(cell);
                    }

                    _cellList[_fieldSize - 1 - i][_fieldSize - 1 - j].cellSprite.color = _colors[_currentColor];
                    _currentColor = (_colors.Count - 1 == _currentColor) ? 0 : _currentColor + 1;
                    _cellList[_fieldSize - 1 - i][_fieldSize - 1 - j].cellObject.transform.position = new Vector3(
                        startPosition + (i + 0.5f) * _cellSize,
                        startPosition + (j + 0.5f) * (_cellSize), 1);
                    _cellList[_fieldSize - 1 - i][_fieldSize - 1 - j].cellObject.name = $"Cell[{i}][{j}]";
                }
            }

            foreach (Apple apple in _appleList)
            {
                apple.appleObject.transform.position =
                    (Vector2) _cellList[apple.appleId.x][apple.appleId.y].cellObject.transform.position;
            }

            foreach (Good apple in _goodList)
            {
                apple.goodObject.transform.position =
                    (Vector2) _cellList[apple.goodId.x][apple.goodId.y].cellObject.transform.position;
            }

            Snake.Instance.Head.position = _cellList[_cellSnake[0].x][_cellSnake[0].y].cellObject.transform.position;

            for (int i = 1; i < _cellSnake.Count; i++)
            {
                Snake.Instance.Segments[i - 1].segmentTransform.position =
                    _cellList[_cellSnake[i].x][_cellSnake[i].y].cellObject.transform.position;
            }
            //LogCells();
        }
        
        public void LogCells()
        {
            foreach (var cell in _cellList)
            {
                foreach (var oneCell in cell)
                {
                    Debug.Log(_cellList.IndexOf(cell) + " :" + cell.IndexOf(oneCell) + ":");
                }
            }
        }

        public void UpgradeAppleCooldown()
        {
            appleCooldown -= appleCooldown * MainShopManager.newFoodPercent;
        }

        public void UpgradeFoodFinding()
        {
            _foodFinding += 1;
        }

        public void UpgradeSteelStomach()
        {
            _pointForFood += 1;
        }

        public void UpgradeBoostSpeed()
        {
            _speedBoost += 1;
        }

        public void UpgradeBoostMetabolism()
        {
            _metabolismBoost += 1;
        }

        public void UpgradeStrongMuscles(int grade)
        {
            if (grade == 1)
            {
                speedSnakeForTime -= 0.05f;
            }
            else
            {
                speedSnakeForTime -= 0.005f;
            }
        }

        private class Good
        {
            public Vector2Int goodId;
            public GameObject goodObject;
        }

        private class Apple
        {
            public GameObject appleObject;
            public Vector2Int appleId;
            public int point;
            public int fruitID;
        }

        private class Diamond
        {
            public GameObject Object;
            public Vector2Int Id;
        }
        
        public void ExpandSnakeList()
        {
            var lastCell = (_cellSnake.Count > 0) ? _cellSnake[^1] : Vector2Int.zero;
            _cellSnake.Add(lastCell);
        }
    }
}