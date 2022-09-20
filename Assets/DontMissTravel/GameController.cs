using System;
using System.Collections.Generic;
using _Project.Gates;
using DontMissTravel.Data;
using DontMissTravel.HideGame;
using DontMissTravel.Obstacles;
using DontMissTravel.Persons;
using DontMissTravel.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel
{
    public class GameController : MonoBehaviour
    {
        private static GameController _instance;

        [SerializeField] private GateController _gateController;
        [SerializeField] private HideGameController _hideGame;
        [SerializeField] private Player _player;
        [Space] [SerializeField] private float _maxGameTime;
        [SerializeField] private float _departureTime;
        [Space] [SerializeField] private GameObject _musicMute;
        [SerializeField] private GameObject _soundMute;
        [Space] [SerializeField] private Bonus _bonus;

        public Action<GameState> OnGameStateChanged;

        private List<Vector2> _availableObstaclesPositions;
        private GameState _gameState;
        private Hud _hud;

        private bool _sound;
        private bool _music;
        private bool _isPhone;
        private bool _gateIsOpen = false;
        private float _timer = 0f;
        private float _timeForBonus;

#region Properties

        public static GameController Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public bool Sound
        {
            get => _sound;
            set => _sound = value;
        }

        public bool Music
        {
            get => _music;
            set => _music = value;
        }

        public bool IsPhone
        {
            get => _isPhone;
            set => _isPhone = value;
        }

        public GameState GameState => _gameState;

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

        private void Start()
        {
            _hud = Hud.Instance;
            _hud.Controller.SetActive(_isPhone);
            CheckDepartureTime();
            _timeForBonus = Random.Range(0, _maxGameTime);
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

            if (_gameState == GameState.HideGame)
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

        public void SwitchGameState(GameState gameState)
        {
            _gameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
            Debug.Log($"<b>Game state changed</b> -> {gameState.ToString()}");
        }

        private void OnPlayerReachedGate()
        {
            _hud.ShowHideWindow(WindowName.Win, true);
            SwitchGameState(GameState.Pause);
            LevelGenerator.Instance.LevelUp();
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

        private void CheckTime(float time)
        {
            if (time > 0)
            {
                _hud.SetGateWillOpenTime(time);
            }
            else
            {
                _hud.ChangeGateState(GateState.Closed, null);
                _hud.ShowHideWindow(WindowName.Lose, true);
                SwitchGameState(GameState.Pause);
            }
        }

        private float GetTimeLeft()
        {
            _timer += Time.deltaTime;
            return _maxGameTime - _timer;
        }

        private void OpenGate()
        {
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

        public void PauseExit()
        {
            SwitchGameState(GameState.Play);
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
            PauseExit();
            _timer = 0;
        }

        public void SoundOnOff()
        {
            _sound = !_sound;
            _soundMute.SetActive(_sound);
        }

        public void MusicOnOff()
        {
            _music = !_music;
            _musicMute.SetActive(_music);
        }

        public void OnApplicationQuit()
        {
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