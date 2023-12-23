using System;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;
using Direction = DontMissTravel.Data.Direction;

namespace DontMissTravel.Persons
{
    public class PersonController : MonoBehaviour
    {
        [SerializeField] protected Rigidbody2D _rigidbody;
        [SerializeField] protected Collider2D _collider2D;
        [Space] [SerializeField] protected Image _targetImage;
        [SerializeField] protected Sprite _idleSprite;
        [SerializeField] protected Sprite[] _moveSprites;
        [SerializeField] private Sprite[] _grabSprites;
        [Space] [SerializeField] private float _switchSpriteLimitTimer;

        [SerializeField] protected PersonAction _currentPersonAction;

        private Sprite[] _currentSprites;
        private Transform _currentTransform;
        private float _timer = 0f;
        private int _spriteIndex = 0;

        private bool _isPersonStay;
        private KeepDataManager _keepDataManager;

        protected KeepDataManager KeepDataManager
        {
            get
            {
                if (_keepDataManager == null)
                {
                    _keepDataManager = Singleton<KeepDataManager>.Instance;
                }

                return _keepDataManager;
            }
        }

        protected virtual void Awake()
        {
            _currentTransform = transform;
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            KeepDataManager.OnGameStateChanged += OnGameStateChanged;
        }

        protected virtual void OnDisable()
        {
            KeepDataManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void LateUpdate()
        {
            if (KeepDataManager.GameState != GameState.Play)
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

            if (_currentPersonAction != PersonAction.Move && _currentPersonAction != PersonAction.Grab)
            {
                return;
            }

            if (_currentSprites == null)
            {
                if (_currentPersonAction == PersonAction.Move)
                {
                    SwitchSprites(PersonAction.Move);
                }

                return;
            }

            _timer += Time.deltaTime;
            if (_timer > _switchSpriteLimitTimer)
            {
                _spriteIndex++;
                if (_spriteIndex > _currentSprites.Length - 1)
                {
                    _spriteIndex = 0;
                }

                _targetImage.sprite = _currentSprites[_spriteIndex];
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
            bool stop = direction == Direction.Idle;
            PersonAction tempPersonAction = PersonAction.None;

            if (!stop)
            {
                Vector3 scale = _currentTransform == null ? Vector3.one : _currentTransform.localScale;
                switch (direction)
                {
                    case Direction.Left:
                    {
                        velocity = Vector2.left;
                        if (_currentPersonAction != PersonAction.Grab)
                        {
                            scale.x = 1f;
                        }

                        break;
                    }
                    case Direction.Right:
                    {
                        velocity = Vector2.right;
                        if (_currentPersonAction != PersonAction.Grab)
                        {
                            scale.x = -1f;
                        }

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

                if (_currentPersonAction == PersonAction.Grab)
                {
                    SwitchSprites(PersonAction.Grab);
                    _rigidbody.velocity = velocity * speed;
                    return;
                }

                if (_currentTransform != null)
                {
                    _currentTransform.localScale = scale;
                }
                
                tempPersonAction = PersonAction.Move;
            }
            else
            {
                Player player = this as Player;
                if (player != null)
                {
                    if (_currentPersonAction == PersonAction.Grab && player.IsPulling)
                    {
                        SwitchSprites(PersonAction.Grab);
                        _rigidbody.velocity = velocity * speed;
                        return;
                    }
                }

                velocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
                tempPersonAction = PersonAction.Stay;
            }

            if (_currentPersonAction != tempPersonAction)
            {
                _currentPersonAction = tempPersonAction;
                SwitchSprites(_currentPersonAction);
            }

            _rigidbody.velocity = velocity * speed;
        }

        protected void SwitchSprites(PersonAction personAction)
        {
            if (_targetImage == null)
            {
                Debug.Log($"Image is null.");
                Debug.Log($"{gameObject}");
                Debug.Log($"{name}");
                return;
            }
            
            switch (personAction)
            {
                case PersonAction.Move:
                {
                    _currentSprites = _moveSprites;
                    break;
                }
                case PersonAction.Stay:
                {
                    _targetImage.sprite = _idleSprite;
                    break;
                }
                case PersonAction.Meet when this is Player:
                {
                    _targetImage.sprite = _idleSprite;
                    break;
                }
                case PersonAction.Grab when this is Player:
                {
                    _currentSprites = _grabSprites;
                    break;
                }
                default:
                {
                    Debug.LogError($"[{name}] Something wrong here! Action: {personAction}");
                    break;
                }
            }
        }
    }
}