using System;
using System.Collections;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using DontMissTravel.Tutorial;
using UnityEngine;
using AudioType = DontMissTravel.Audio.AudioType;
using Random = UnityEngine.Random;

namespace DontMissTravel.HideGame
{
    public class HideGameController : MonoBehaviour
    {
        private enum Side
        {
            Left,
            Right
        }

        private const float MinSpeed = 1f;
        private const float MaxSpeed = 0.15f;
        private const float HowLong = 15f;

        [SerializeField] private HideGameEnemy _enemy;
        [SerializeField] private RectTransform _player;
        [SerializeField] private RectTransform _hideGameField;

        private GameController _gameController;
        private RectTransform _enemyRectTransform;

        private float _timer = 0f;
        private bool _isLeft;

        private Vector2 _enemyPosition1;
        private Vector2 _enemyPosition2;

        private Vector2 _playerPosition1;
        private Vector2 _playerPosition2;
        private Vector2 _initPlayerPosition;
        
        private bool _gameStateIsPause;

        public void Init(GameController gameController)
        {
            _gameController = gameController;
            _enemyRectTransform = _enemy.RectTransform;
            SetPositions();
            gameObject.SetActive(false);
            _gameController.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.HideGame:
                    _gameStateIsPause = false;
                    StartCoroutine(nameof(ChangePosition));
                    break;
                case GameState.Pause:
                    _gameStateIsPause = true;
                    StopCoroutine(nameof(ChangePosition));
                    break;
            }
        }

        public void StartHideGame(EnemyName enemyName)
        {
            _player.position = _initPlayerPosition;
            gameObject.SetActive(true);
            StopCoroutine(nameof(ChangePosition));
            StartCoroutine(nameof(ChangePosition));
            _enemy.SetEnemy(enemyName);
        }

        public void CloseHideGame()
        {
            StopCoroutine(nameof(ChangePosition));
            gameObject.SetActive(false);
        }

        private IEnumerator ChangePosition()
        {
            _timer = Time.deltaTime;
            while (_timer < HowLong)
            {
                _enemyRectTransform.anchoredPosition = _isLeft ? _enemyPosition1 : _enemyPosition2;
                _isLeft = !_isLeft;
                _timer += Time.deltaTime;
                yield return new WaitForSeconds(GetRandomSpeed());
            }
        }

        private float GetRandomSpeed()
        {
            return Random.Range(MinSpeed, MaxSpeed);
        }

        private void Update()
        {
            if (_gameController.IsPhone || _gameStateIsPause)
            {
                return;
            }

            if (IsHorizontalKeyboard())
            {
                CheckPosition(Input.GetAxis("Horizontal"));
            }
        }

        private bool IsHorizontalKeyboard()
        {
            return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
        }

        private void CheckPosition(float clickPositionX)
        {
            Side horizontal = clickPositionX > 0 ? Side.Right : Side.Left;
            _player.anchoredPosition = GetPlayerPosition(horizontal);
            CheckIfCouldEscape();
        }

        private Vector2 GetPlayerPosition(Side horizontal)
        {
            return horizontal == Side.Left ? _playerPosition1 : _playerPosition2;
        }

        private void SetPositions()
        {
            float rectWidth = (_hideGameField.anchorMax.x - _hideGameField.anchorMin.x) * Screen.width;
            float leftCenterX = rectWidth / 4.5f;
            float rightCenterX = leftCenterX * -1f;
            
            Debug.Log($"Rect width = {rectWidth}. Left center: {leftCenterX}. Right: {rightCenterX}");

            _enemyPosition1 = _enemyRectTransform.anchoredPosition;
            _enemyPosition2 = _enemyPosition1;

            _enemyPosition1.x = leftCenterX;
            _enemyPosition2.x = rightCenterX;
            
            Debug.Log($"Enemy position 1: {_enemyPosition1}. Enemy position 2: {_enemyPosition2}");

            _initPlayerPosition = _player.position;
            _playerPosition1 = _initPlayerPosition;
            _playerPosition2 = _playerPosition1;

            _playerPosition1.x = leftCenterX;
            _playerPosition2.x = rightCenterX;
        }

        private void CheckIfCouldEscape()
        {
            float playerX = _player.position.x;
            float enemyX = _enemyRectTransform.position.x;

            if (playerX > 0 && enemyX < 0 || playerX < 0 && enemyX > 0)
            {
                AudioManager.Instance.PlaySfx(AudioType.HideGameNegative);
                return;
            }

            TutorialGame tutorialGame = _gameController as TutorialGame;
            if (tutorialGame != null)
            {
                tutorialGame.CloseHideGame();
            }
            else
            {
                _gameController.OpenHideGame(false);    
            }
            
            AudioManager.Instance.PlaySfx(AudioType.HideGamePositive);
            StopCoroutine(nameof(ChangePosition));
        }
    }
}