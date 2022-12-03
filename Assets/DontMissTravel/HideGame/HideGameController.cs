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

        private float UserCheckMoveDelay = 0.001f;
        private const float MinSpeed = 1f;
        private const float MaxSpeed = 0.15f;
        private const float HowLong = 15f;

        [SerializeField] private HideGameEnemy _enemy;
        [SerializeField] private RectTransform _player;
        [SerializeField] private RectTransform _hideGameField;

        private GameController _gameController;
        private RectTransform _enemyRectTransform;
        private AudioManager _audioManager;
        private KeepDataManager _keepDataManager;

        private float _enemyTimer = 0f;
        private float _userMoveTimer = 0f;
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
            _audioManager = Singleton<AudioManager>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;
        }

        private void OnDestroy()
        {
            _keepDataManager.OnGameStateChanged -= OnGameStateChanged;
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
            _keepDataManager.OnGameStateChanged += OnGameStateChanged;
        }

        public void CloseHideGame()
        {
            StopCoroutine(nameof(ChangePosition));
            gameObject.SetActive(false);
            _keepDataManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private IEnumerator ChangePosition()
        {
            _enemyTimer = Time.deltaTime;
            while (_enemyTimer < HowLong)
            {
                _enemyRectTransform.anchoredPosition = _isLeft ? _enemyPosition1 : _enemyPosition2;
                _isLeft = !_isLeft;
                _enemyTimer += Time.deltaTime;
                yield return new WaitForSeconds(GetRandomSpeed());
            }
        }

        private float GetRandomSpeed()
        {
            return Random.Range(MinSpeed, MaxSpeed);
        }


        private void Update()
        {
            if (_keepDataManager.IsPhone || _gameStateIsPause)
            {
                return;
            }

            _userMoveTimer += Time.deltaTime;
            if (_userMoveTimer < UserCheckMoveDelay)
            {
                return;
            }

            if (IsHorizontalKeyboard())
            {
                CheckPosition(Input.GetAxis("Horizontal"));
            }

            _userMoveTimer = 0;
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
                _audioManager.PlaySfx(AudioType.HideGameNegative);
                return;
            }

            _gameController.OpenHideGame(false);
            _audioManager.PlaySfx(AudioType.HideGamePositive);
            StopCoroutine(nameof(ChangePosition));
        }
    }
}