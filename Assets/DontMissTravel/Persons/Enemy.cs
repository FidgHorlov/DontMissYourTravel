using System;
using System.Collections;
using DontMissTravel.Data;
using UnityEngine;
using Random = UnityEngine.Random;
using Direction = DontMissTravel.Data.Direction;
using EnemyType = DontMissTravel.Data.EnemyType;


namespace DontMissTravel.Persons
{
    public class Enemy : PersonController
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private EnemyName _enemyName;

        private Direction _previousDirection = Direction.Idle;
        private Vector2 _lastCollisionPosition;

        private bool _toChangeDirection;
        private bool _isStop;
        private bool _isMeet;
        private float _detectDistance;

        public EnemyType EnemyType => _enemyType;
        public EnemyName EnemyName => _enemyName;

        protected override void Awake()
        {
            base.Awake();
            KeepDataManager.OnGameStateChanged += OnGameStateChanged;
            bool isRight = Random.Range(0, 2) == 0;
            bool isTop = Random.Range(0, 2) == 0;
            Move(WhereToGo(isRight, isTop), _speed);
        }

        private void OnDestroy()
        {
            KeepDataManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Play:
                    _currentPersonAction = PersonAction.Move;
                    break;
                case GameState.HideGame:
                    _currentPersonAction = PersonAction.Stay;
                    break;
                case GameState.Pause:
                    _currentPersonAction = PersonAction.Stay;
                    break;
            }

            SwitchSprites(_currentPersonAction);
            if (gameState != GameState.Play)
            {
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
                StopCoroutine(nameof(CheckThePosition));
                return;
            }

            Move(_previousDirection, _speed);
        }

        private Direction WhereToGo(bool isRight, bool isTop)
        {
            Direction targetDirection;
            int random = Random.Range(0, 2);

            if (isRight)
            {
                if (isTop)
                {
                    targetDirection = random == 0 ? Direction.Left : Direction.Down;
                }
                else
                {
                    targetDirection = random == 0 ? Direction.Left : Direction.Up;
                }
            }
            else
            {
                if (isTop)
                {
                    targetDirection = random == 0 ? Direction.Right : Direction.Down;
                }
                else
                {
                    targetDirection = random == 0 ? Direction.Right : Direction.Up;
                }
            }

            targetDirection = GetTotalRandomDirection(targetDirection);
            _previousDirection = targetDirection;
            return targetDirection;
        }

        private Direction GetTotalRandomDirection(Direction targetDirection)
        {
            if (_previousDirection == targetDirection)
            {
                targetDirection = (Direction) Random.Range(0, Enum.GetNames(typeof(Direction)).Length);
            }

            return targetDirection;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (KeepDataManager.GameState != GameState.Play)
            {
                StopCoroutine(nameof(CheckThePosition));
                return;
            }
            
            Vector3 contactPoint = other.contacts[0].point;
            Vector3 center = _collider2D.bounds.center;

            bool right = contactPoint.x > center.x;
            bool top = contactPoint.y > center.y;

            Move(WhereToGo(right, top), _speed);

            _lastCollisionPosition = transform.position;
            StartCoroutine(nameof(CheckThePosition));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (KeepDataManager.GameState != GameState.Play)
            {
                return;
            }

            if (!other.CompareTag(Constants.Tags.Gate))
            {
                return;
            }

            _lastCollisionPosition = transform.position;
            _currentPersonAction = PersonAction.Stay;
            StartCoroutine(nameof(CheckThePosition));
        }

        private IEnumerator CheckThePosition()
        {
            yield return new WaitForSeconds(1f);
            if (KeepDataManager.GameState == GameState.HideGame)
            {
                yield break;
            }

            if (_currentPersonAction != PersonAction.Stay)
            {
                yield break;
            }

            if (Vector2.Distance(_lastCollisionPosition, transform.position) < 1.5f)
            {
                bool isRight = Random.Range(0, 2) == 0;
                bool isTop = Random.Range(0, 2) == 0;
                Move(WhereToGo(isRight, isTop), _speed);
            }
        }
    }
}