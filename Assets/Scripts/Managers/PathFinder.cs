using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class PathFinder : Singleton<PathFinder>
    {
        private bool _isReverse;

        private static List<int> _randomDirectionValue = new(10);
        
        public static Vector2Int RandomDirection()
        {
            var repeatValueCount = 10;
            var nextDirRand = 0;
            
            while (repeatValueCount > 5)
            {
                nextDirRand = Random.Range(1, 5);
                
                repeatValueCount = _randomDirectionValue.Count(i => i == nextDirRand);
            }

            _randomDirectionValue.Insert(0,nextDirRand);
            _randomDirectionValue.RemoveAt(_randomDirectionValue.Count - 1);
            
            //Debug.Log(nextDirRand);
            var direction = nextDirRand switch
            {
                1 => Vector2Int.down,
                2 => Vector2Int.right,
                3 => Vector2Int.up,
                4 => Vector2Int.left,
                _ => Vector2Int.zero
            };
            
            return direction;
        }

        public static Vector2Int DirectionToCell(Vector2Int cell)
        {
            var direction = Vector2Int.zero;
            
            if (cell.x > FieldManager.Instance._cellSnake[0].x)
            {
                direction = Vector2Int.right;

            }
            else if (cell.x < FieldManager.Instance._cellSnake[0].x)
            {
                direction = Vector2Int.left;

            }
            else if (cell.y > FieldManager.Instance._cellSnake[0].y)
            {

                direction = Vector2Int.up;
            }
            else if (cell.y < FieldManager.Instance._cellSnake[0].y)
            {
                direction = Vector2Int.down;
            }

            while ((FieldManager.Instance._cellSnake[0] + direction) == FieldManager.Instance._cellSnake[1] ||
                (FieldManager.Instance._cellSnake[0].x + direction.x) > (FieldManager.Instance.FieldSize - 1) ||
                (FieldManager.Instance._cellSnake[0].y + direction.y) > (FieldManager.Instance.FieldSize - 1) ||
                (FieldManager.Instance._cellSnake[0].x + direction.x) < 0 ||
                (FieldManager.Instance._cellSnake[0].y + direction.y) < 0)
            {

                if (direction == Vector2Int.right)
                {
                    direction = Vector2Int.up;
                }
                else if (direction == Vector2Int.left)
                {
                    direction = Vector2Int.down;
                }

                else if (direction == Vector2Int.up)
                {
                    direction = Vector2Int.left;
                }
                else if (direction == Vector2Int.down)
                {
                    direction = Vector2Int.right;
                }
            }

            return direction;
        }

        // public Vector2Int GetNextCell(Vector2Int position, Vector2Int direction)
        // {
        //     Vector2Int nextDirection;
        //     if (direction == Vector2Int.down || direction == Vector2Int.up)
        //     {
        //         Vector2Int nextCell =
        //             new Vector2Int(Mathf.Clamp(position.x + direction.x, 0, FieldManager.Instance.FieldSize - 1),
        //                 Mathf.Clamp(position.y + direction.y, 0, FieldManager.Instance.FieldSize - 1));
        //         if (nextCell != position) return direction;
        //         nextDirection = (_isReverse) ? Vector2Int.right : Vector2Int.left;
        //         nextCell = new Vector2Int(
        //             Mathf.Clamp(position.x + nextDirection.x, 0, FieldManager.Instance.FieldSize - 1),
        //             Mathf.Clamp(position.y + nextDirection.y, 0, FieldManager.Instance.FieldSize - 1));
        //         if (nextCell != position) return nextDirection;
        //         _isReverse = !_isReverse;
        //         nextDirection = (_isReverse) ? Vector2Int.right : Vector2Int.left;
        //         nextCell = new Vector2Int(
        //             Mathf.Clamp(position.x + nextDirection.x, 0, FieldManager.Instance.FieldSize - 1),
        //             Mathf.Clamp(position.y + nextDirection.y, 0, FieldManager.Instance.FieldSize - 1));
        //         return nextDirection;
        //     }
        //     else
        //     {
        //         nextDirection = Vector2Int.down;
        //         Vector2Int nextCell =
        //             new Vector2Int(Mathf.Clamp(position.x + nextDirection.x, 0, FieldManager.Instance.FieldSize - 1),
        //                 Mathf.Clamp(position.y + nextDirection.y, 0, FieldManager.Instance.FieldSize - 1));
        //         if (nextCell != position) return nextDirection;
        //         nextDirection = Vector2Int.up;
        //         nextCell = new Vector2Int(
        //             Mathf.Clamp(position.x + nextDirection.x, 0, FieldManager.Instance.FieldSize - 1),
        //             Mathf.Clamp(position.y + nextDirection.y, 0, FieldManager.Instance.FieldSize - 1));
        //         return nextDirection;
        //     }
        // }
    }
}