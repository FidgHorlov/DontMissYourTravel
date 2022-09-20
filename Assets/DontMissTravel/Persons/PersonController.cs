using System;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;
using Action = DontMissTravel.Data.Action;
using Direction = DontMissTravel.Data.Direction;

namespace DontMissTravel.Persons
{
    public class PersonController : MonoBehaviour
    {
        [SerializeField] protected Rigidbody2D _rigidbody;
        [SerializeField] protected Collider2D _collider2D;
        [Space]
        [SerializeField] protected Image _targetImage;
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite[] _moveSprites;
        [Space]
        [SerializeField] private float _switchSpriteLimitTimer;

        protected Action CurrentAction;
        protected Sprite[] CurrentSprites;

        private GameController _gameController;
        private Transform _currentTransform;
        private float _timer = 0f;
        private int _spriteIndex = 0;

        private bool _isPersonStay;

        protected virtual void Awake()
        {
            _currentTransform = transform;
            _gameController = GameController.Instance;
            _gameController.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            _gameController.OnGameStateChanged -= OnGameStateChanged;
        }

        private void LateUpdate()
        {
            if (_gameController.GameState != GameState.Play)
            {
                if (_isPersonStay)
                {
                    return;
                }
                
                _isPersonStay = true;
                _targetImage.sprite = _idleSprite;
                Move(Direction.Idle, 0f); // when it pause shouldn't be able walk    
                return;
            }
            
            if (CurrentAction == Action.Stay || CurrentSprites == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer > _switchSpriteLimitTimer)
            {
                _spriteIndex++;
                if (_spriteIndex > CurrentSprites.Length - 1)
                {
                    _spriteIndex = 0;
                }

                _targetImage.sprite = CurrentSprites[_spriteIndex];
                _timer = 0f;
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.Play && _isPersonStay)
            {
                _isPersonStay = false;
            }
        }

        protected void Move(Direction direction, float speed)
        {
            Vector2 velocity = Vector2.zero;
            bool stop = direction == Direction.Idle; //stop if toWhere == Stop
            
            if (!stop)
            {
                if (CurrentAction == Action.Stay)
                {
                    CurrentAction = Action.Move;
                }
                
                Vector3 scale = _currentTransform.localScale;
                switch (direction)
                {
                    case Direction.Left:
                    {
                        velocity = Vector2.left;
                        scale.x = 1f;
                        break;
                    }
                    case Direction.Right:
                    {
                        velocity = Vector2.right;
                        scale.x = -1f;
                        break;
                    }
                    case Direction.Up:
                    {
                        velocity = Vector2.up;
                        break;
                    }
                    case Direction.Down:
                    {
                        velocity = Vector2.down;
                        break;
                    }
                }

                _currentTransform.localScale = scale;
                SwitchSprites(Action.Move);
            }
            else
            {
                velocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
                SwitchSprites(Action.Stay);
                CurrentAction = Action.Stay;
            }

            _rigidbody.velocity = velocity * speed;
        }

        protected virtual void SwitchSprites(Action action)
        {
            switch (action)
            {
                case Action.Move:
                {
                    CurrentSprites = _moveSprites;
                    break;
                }
                case Action.Stay:
                {
                    _targetImage.sprite = _idleSprite;
                    break;
                }
            }
        }
    }
}