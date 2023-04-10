using System.Collections.Generic;
using Extensions.Core;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class Snake : Singleton<Snake>
    {
        public class Segment
        {
            public SpriteRenderer segmentSprite;
            public Transform segmentTransform;
        }

        [SerializeField]
        private Transform _head;

        [SerializeField]
        private GameObject _stars;
        public Transform Head => _head;

        [SerializeField]
        private float _segmentWidth;

        [Header("Segment Properties"),SerializeField]
        //private List<GameObject> _segmentsObjects = new();
        private GameObject _segmentPrefab;

        [SerializeField]
        private GameObject _segmentParent;

        public List<Segment> Segments { get; } = new();

        [Header("Sprites"), SerializeField]
        private Sprite _straightSprite;

        [SerializeField]
        private Sprite _swivelSprite;

        [SerializeField]
        private Sprite _endSprite;

        [SerializeField]
        private Material _snakeMaterial;

        private static readonly int Thickness = Shader.PropertyToID("_Thickness");


        public void MoveToTarget(Vector2 target)
        {
            //int  


            if (Segments.Count > 0)
            {
                for (var i = Segments.Count - 1; i >= 0; i--)
                {
                    var nextSegment = (i == 0) ? _head : Segments[i - 1].segmentTransform;
                    Segments[i].segmentTransform.position = nextSegment.position;
                }
            }

            var position = _head.position;
            var tg = target - (Vector2) position;
            position = target;
            _head.position = position;
            UpdateSnakeSprites((Vector2) position + tg);
        }

        public void UpdateSnakeSprites(Vector2 target)
        {
            _head.LookAt2D(target);
            
            for (var i = 0; i < Segments.Count; i++)
            {
                var nextSegment = (i == 0) ? _head : Segments[i - 1].segmentTransform;
                var prevSegment = (i == Segments.Count - 1) ? null : Segments[i + 1].segmentTransform;
                
                var position = nextSegment.position;
                //Vector2 direction = (position - Segments[i].segmentTransform.position).normalized;
                var segmentTransform = Segments[i].segmentTransform;
                
                segmentTransform.LookAt2D(position);
                segmentTransform.gameObject.name = "Segment" + (i + 1);
                
                if (prevSegment != null)
                {
                    Vector2 offset = nextSegment.position - prevSegment.position;
                    Segments[i].segmentSprite.sprite = (offset.x == 0 || offset.y == 0) 
                        ? _straightSprite 
                        : _swivelSprite;
                    
                    if (!(offset.x == 0 || offset.y == 0))
                    {
                        Segments[i].segmentSprite.flipX =
                            !((segmentTransform.position +
                                  segmentTransform.right * _segmentWidth - prevSegment.position).magnitude < 0.01f);
                    }
                }
                else
                {
                    Segments[i].segmentSprite.sprite = _endSprite;
                }
            }
        }

        public void SnakeStartStan()
        {
            _stars.SetActive(true);
        }
        
        public void SnakeStopStan()
        {
            _stars.SetActive(false);
        }

        public void AddSegment()
        {
            var obj = Instantiate(_segmentPrefab);
            var newTransform = obj.transform;

            var segment = new Segment
            {
                segmentSprite = obj.GetComponent<SpriteRenderer>(),
                segmentTransform = newTransform
            };
            
            segment.segmentSprite.sprite = _endSprite;
            newTransform.SetParent(_segmentParent.transform);
            
            var transformSegment = (Segments.Count > 0) ? Segments[^1].segmentTransform : _head;
            
            newTransform.position = transformSegment.position;
            newTransform.up = transformSegment.up;
            
            newTransform.gameObject.SetActive(true);
            
            Segments.Add(segment);
            FieldManager.Instance.ExpandSnakeList();
        }

        public void SetBloomAmount(float value)
        {
           _snakeMaterial.SetFloat(Thickness, value);
        }        
    }
}

