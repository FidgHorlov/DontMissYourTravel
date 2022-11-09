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
using UnityEngine.SceneManagement;
using AudioType = DontMissTravel.Audio.AudioType;
using Random = UnityEngine.Random;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel
{
    public class GameController : MonoBehaviour
    {
        public event Action OnTutorialTimeOut;
        public Action<GameState> OnGameStateChanged;
        
        private static GameController _instance;

        [SerializeField] protected HideGameController _hideGame;
        [SerializeField] private GateController _gateController;
        [SerializeField] private Player _player;
        [Space] [SerializeField] private float _maxGameTime;
        [Space] [SerializeField] private Bonus _bonus;

        private AudioManager _audioManager;
        
        private List<Vector2> _availableObstaclesPositions;
        private GameState _gameState;
        private Hud _hud;

        private bool _isPhone;
        private bool _gateIsOpen = false;
        private float _timer = 0f;
        private float _timeForBonus;
        private float _departureTime;

#region Properties

        public static GameController Instance => _instance;
        public bool IsPhone => _isPhone;
        public GameState GameState => _gameState;

        private AudioManager AudioManager
        {
            get
            {
                if (_audioManager == null)
                {
                    _audioManager = AudioManager.Instance;
                }

                return _audioManager;
            }
        }

#endregion

        private void Awake()
        {
            _instance = FindObjectOfType<GameController>();
            if (_instance != this)
            {
                Destroy(_instance);
            }

            _gateController.OnPlayerReachedGate += OnPlayerReachedGate;
            _bonus.Init(player: _player);
            _hideGame.Init(this);

#if UNITY_ANDROID || UNITY_IOS
            _isPhone = true;
#elif UNITY_STANDALONE
            _isPhone = false;
#endif
        }

        private void OnEnable()
        {
            CheckDepartureTime();
            _timeForBonus = Random.Range(0, _maxGameTime);
        }

        private void Start()
        {
            _hud = Hud.Instance;
            _hud.Controller.SetActive(_isPhone);
            AudioManager.PlayAmbient();
        }

        private void Update()
        {
            if (_gameState == GameState.Pause)
            {
                return;
            }

            float timeLeft = GetTimeLeft();
            if (timeLeft <= 0)
            {
                if (_gameState == GameState.Tutorial)
                {
                    CheckDepartureTime();
                    _hud.ChangeGateState(GateState.WillOpen, timeLeft.ToString(CultureInfo.CurrentCulture));
                    OnTutorialTimeOut?.Invoke();
                    return;
                }
                
                _hud.ChangeGateState(GateState.Closed, null);
                _hud.ShowHideWindow(WindowName.Lose, true);
                SwitchGameState(GameState.Pause);
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
            
            switch (_gameState)
            {
                case GameState.Tutorial:
                case GameState.HideGame:
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

        public void SwitchGameState(GameState gameState)
        {
            _gameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
            Debug.Log($"<b>Game state changed</b> -> {gameState.ToString()}");

            switch (gameState)
            {
                case GameState.Play:
                case GameState.Tutorial:
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

        protected virtual void OnPlayerReachedGate()
        {
            _hud.ShowHideWindow(WindowName.Win, true);
            SwitchGameState(GameState.Pause);
            LevelGenerator.Instance.LevelUp();
        }

        private void CheckDepartureTime()
        {
            _departureTime = _maxGameTime / 2.5f;
        }

        protected void CreateBonus()
        {
            _bonus.SetActiveInTime(true, GetRandomAvailablePosition());
            _timeForBonus = -1;
        }

        private float GetTimeLeft()
        {
            _timer += Time.deltaTime;
            return _maxGameTime - _timer;
        }

        private void OpenGate()
        {
            AudioManager.Instance.PlaySfx(AudioType.Gate, defaultPitch: true);
            int gateNumber = Random.Range(0, _gateController.GateCount - 1);
            string gateName = _gateController.OpenGate(gateNumber);
            _gateIsOpen = true;
            _hud.ChangeGateState(GateState.Opened, gateName);
        }

        public void Delay()
        {
            float randDelay = Random.Range(5f, 25f);
            _maxGameTime += randDelay;
            _hud.SetDelay(randDelay);
        }

        public void OpenHideGame(bool toOpen, EnemyName enemyName = EnemyName.Nurse1)
        {
            _hud.Player.SetActive(!toOpen);
            _hud.Playground.SetActive(!toOpen);
            
            if (toOpen)
            {
                _hideGame.StartHideGame(enemyName);
            }
            else
            {
                _hideGame.CloseHideGame();
                _player.OnMeetFinish();
            }

            SwitchGameState(toOpen ? GameState.HideGame : GameState.Play);
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

        [ContextMenu("Create bonus")]
        private void CreateBonusEditor()
        {
            CreateBonus();
        }

        [ContextMenu("Hide game")]
        private void HideGameOpen()
        {
            OpenHideGame(true);
        }

        [ContextMenu("Unlimited time")]
        private void UnlimitedTime()
        {
            _maxGameTime *= 1000f;
        }

#region Game Buttons

        private void PauseExit()
        {
            SwitchGameState(GameState.Play);
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
            PauseExit();
            _timer = 0;
        }

        public void OnApplicationQuit()
        {
            LevelGenerator levelGenerator = LevelGenerator.Instance;
            if (levelGenerator == null)
            {
                return;
            }

            PlayerPrefs.SetInt("Level", LevelGenerator.Instance.CurrentLevel);
        }

        public void NextLevel()
        {
            PauseExit();
            SceneManager.LoadScene(0);
            print(LevelGenerator.Instance.CurrentLevel);
        }

#endregion
    }
}