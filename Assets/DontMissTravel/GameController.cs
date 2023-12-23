using System;
using System.Collections.Generic;
using System.Globalization;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using DontMissTravel.Gates;
using DontMissTravel.HideGame;
using DontMissTravel.Obstacles;
using DontMissTravel.Persons;
using DontMissTravel.Ui;
using UnityEngine;
using AudioType = DontMissTravel.Audio.AudioType;
using Random = UnityEngine.Random;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel
{
    public class GameController : Singleton<GameController>
    {
        public event Action OnTutorialTimeOut;
        public event Action OnTutorialMeetOver;
        public event Action OnTutorialReachGate;

        [SerializeField] private HideGameController _hideGame;
        [SerializeField] private GameObject _hideGameCanvas;
        [SerializeField] private GameObject _mainGameCanvas;
        [Space] [SerializeField] private GateController _gateController;
        [SerializeField] private Player _player;
        [Space] [SerializeField] private float _maxGameTime;
        [Space] [SerializeField] private Bonus _bonus;

        private AudioManager _audioManager;

        private List<Vector2> _availableObstaclesPositions;
        private Hud _hud;
        private KeepDataManager _keepDataManager;
        private LevelGenerator _levelGenerator;

        private bool _gateIsOpen = false;
        private float _timer = 0f;
        private float _timeForBonus;
        private float _departureTime;

        private AudioManager AudioManager
        {
            get
            {
                if (_audioManager == null)
                {
                    _audioManager = Singleton<AudioManager>.Instance;
                }

                return _audioManager;
            }
        }

#region Monobehaviour

        private void OnEnable()
        {
            CheckDepartureTime();
            _timeForBonus = Random.Range(0, _maxGameTime);

            _gateController.OnPlayerReachedGate += OnPlayerReachedGate;
            _bonus.Init(player: _player);
            _hideGame.Init();
            HideGameVisibility(false);
        }

        private void OnDisable()
        {
            _gateController.OnPlayerReachedGate -= OnPlayerReachedGate;
        }

        private void Start()
        {
            _timer = 0;
            _hud = Singleton<Hud>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            _levelGenerator = Singleton<LevelGenerator>.Instance;
            AudioManager.PlayAmbient();
            _keepDataManager.OnGameStateChanged += OnGameStateChanged;
            _keepDataManager.IsGameOver = false;
        }

        private void Update()
        {
            if (_keepDataManager.GameState == GameState.Pause)
            {
                return;
            }

            float timeLeft = GetTimeLeft();
            if (timeLeft <= 0 && !_keepDataManager.IsGameOver)
            {
                if (_keepDataManager.GameMode == GameMode.Tutorial)
                {
                    CheckDepartureTime();
                    _hud.ChangeGateState(GateState.WillOpen, timeLeft.ToString(CultureInfo.CurrentCulture));
                    OnTutorialTimeOut?.Invoke();
                    return;
                }

                _hud.ChangeGateState(GateState.Closed, null);
                _hud.ShowHideWindow(WindowName.Lose, true);
                _keepDataManager.IsGameOver = true;
                _keepDataManager.SwitchGameState(GameState.Pause);
                return;
            }

            if (timeLeft < _departureTime || (_gateIsOpen && _bonus.IsDelay))
            {
                _hud.SetGateWillClose(timeLeft);
                if (!_gateIsOpen)
                {
                    OpenGate();
                }
            }
            else
            {
                _hud.SetGateWillOpenTime(timeLeft);
            }

            if (_keepDataManager.GameState.Equals(GameState.HideGame) || _keepDataManager.GameMode.Equals(GameMode.Tutorial))
            {
                return;
            }

            if (timeLeft < _timeForBonus && timeLeft > 0)
            {
                CreateBonus();
            }
        }

        private void OnDestroy()
        {
            _gateController.OnPlayerReachedGate -= OnPlayerReachedGate;
        }

#endregion

#region Public methods

        public void TutorialPrepareHideGame()
        {
            _hideGame.gameObject.SetActive(true);
            _hideGame.enabled = false;
        }

        public void TutorialSetHideGameEnemy(EnemyName enemyName)
        {
            _hideGame.StartHideGame(enemyName);
        }

        public void TutorialStartHideGame()
        {
            _hideGame.enabled = true;
        }

        public void SetCustomGameTime(float maxTime)
        {
            _timer = 0f;
            _maxGameTime = maxTime;
            if (_hud != null)
            {
                _hud.SetGateWillOpenTime(maxTime);
            }
        }

        public void OpenGateDecreaseMaxTime(float newMaxTime)
        {
            _timer = 0f;
            _maxGameTime = newMaxTime;
            _departureTime = newMaxTime;
            _hud.SetGateWillClose(GetTimeLeft());
            OpenGate();
        }

        public void Delay()
        {
            float randDelay = Random.Range(5f, 25f);
            _maxGameTime += randDelay;
            _hud.SetDelay(randDelay);
        }

        public void HideGameSetActive(bool toOpen, EnemyName enemyName)
        {
            _hud.HidePlayerAndPlayground(toHide: toOpen);
            if (toOpen)
            {
                Debug.Log($"Open Hide Game. EnemyName = {enemyName}");
                HideGameVisibility(true);
                _hideGame.MiniGameFinished += MiniGameFinished;
                _hideGame.StartHideGame(enemyName);
            }
            else
            {
                Debug.Log($"Hide Game Closed!");
                HideGameVisibility(false);
                _hideGame.CloseHideGame();
                _player.OnMeetFinish();

                if (_keepDataManager.GameMode == GameMode.Tutorial)
                {
                    OnTutorialMeetOver?.Invoke();
                }
            }

            if (_keepDataManager.GameMode == GameMode.Tutorial && !toOpen)
            {
                // in Tutorial the next Game State is Pause.
                return;
            }

            _keepDataManager.SwitchGameState(toOpen ? GameState.HideGame : GameState.Play);
        }

        public Vector2 GetRandomAvailablePosition()
        {
            int random = Random.Range(0, _availableObstaclesPositions.Count - 1);
            return _availableObstaclesPositions[random];
        }

        public void AddAvailablePositions(Vector2 position)
        {
            _availableObstaclesPositions ??= new List<Vector2>();
            _availableObstaclesPositions.Add(position);
        }

        public void RemoveAvailablePositions(Vector2 position)
        {
            _availableObstaclesPositions.Remove(position);
        }

#endregion

        protected virtual void OnPlayerReachedGate()
        {
            _keepDataManager.SwitchGameState(GameState.Pause);
            switch (_keepDataManager.GameMode)
            {
                case GameMode.Game:
                    _levelGenerator.LevelUp();
                    _keepDataManager.IsGameOver = true;
                    _hud.ShowHideWindow(WindowName.Win, true);
                    break;
                case GameMode.Tutorial:
                    TutorialReachedGate();
                    break;
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Play:
                    AudioManager.PlayAmbient();
                    AudioManager.PlayMusic();
                    break;
                case GameState.HideGame:
                case GameState.Pause:
                    AudioManager.PauseAmbient();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }

        private void CheckDepartureTime()
        {
            _departureTime = _maxGameTime / 2.5f;
        }

        private void CreateBonus()
        {
            _bonus.SetActiveInTime(true, GetRandomAvailablePosition());
            _timeForBonus = -1;
        }

        private float GetTimeLeft()
        {
            _timer += Time.deltaTime;
            return _maxGameTime - _timer;
        }

        [ContextMenu("Open gate")]
        private void OpenGate()
        {
            _audioManager.PlaySfx(AudioType.Gate, defaultPitch: true);
            int gateNumber = Random.Range(0, _gateController.GateCount - 1);
            string gateName = _gateController.OpenGate(gateNumber);
            _gateIsOpen = true;
            _hud.ChangeGateState(GateState.Opened, gateName);
        }

        private void MiniGameFinished()
        {
            Debug.Log($"Mini game finished");
            _hideGame.MiniGameFinished -= MiniGameFinished;
            HideGameSetActive(false, EnemyName.None);
        }

        private void HideGameVisibility(bool isActive)
        {
            _hideGameCanvas.SetActive(isActive);
            _mainGameCanvas.SetActive(!isActive);
        }

        [ContextMenu("Create bonus")]
        private void CreateBonusEditor()
        {
            CreateBonus();
        }

        [ContextMenu("Unlimited time")]
        private void UnlimitedTime()
        {
            _maxGameTime *= 1000f;
        }

        private void OnApplicationQuit()
        {
            if (_levelGenerator != null)
            {
                PlayerPrefs.SetInt("Level", _levelGenerator.CurrentLevel);
            }
        }

        private void TutorialReachedGate()
        {
            OnTutorialReachGate?.Invoke();
        }
    }
}