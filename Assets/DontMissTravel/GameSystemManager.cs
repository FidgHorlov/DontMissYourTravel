using System;
using System.Collections;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using DontMissTravel.Tutorial;
using DontMissTravel.Ui;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;

namespace DontMissTravel
{
    public class GameSystemManager : MonoBehaviour
    {
        public enum GameStatus
        {
            Menu,
            Instruction,
            Game,
            Tutorial
        }

        private static GameSystemManager _instance;

        [SerializeField] private Button _startGameButton;
        [SerializeField] private GameObject _initialMenu;
        [SerializeField] private GameObject _mainGame;

        [Space] [SerializeField] private Instructions _instructions;
        [SerializeField] private Button _infoButton;

        [Space] [SerializeField] private Button _tutorialButton;
        [SerializeField] private TutorialController _tutorial;

        [Space] [SerializeField] private Image _mainImage;
        [SerializeField] private Sprite[] _sprites;

        private GameStatus _gameStatus;
        private bool _isInstructionOpened;
        private bool _isTutorialOpened;
        private AudioManager _audioManager;
        private Hud _hud;

        public static GameSystemManager Instance => _instance;

        public GameStatus CurrentGameStatus => _gameStatus;

        private void Awake()
        {
            _instance = FindObjectOfType<GameSystemManager>();
            if (_instance != this)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;

            _tutorial.gameObject.SetActive(false);
            if (KeepDataManager.Instance.WasGameRun)
            {
                InitialMenuSetActive(false);
                return;
            }

            InitialMenuSetActive(true);
            StartCoroutine(nameof(PlayerAnimation));
            _gameStatus = GameStatus.Menu;
        }

        private void Start()
        {
            _audioManager = AudioManager.Instance;
            _audioManager.PlayMusic();
        }

        private void OnEnable()
        {
            _startGameButton.onClick.AddListener(OnStartGameButtonClick);
            _infoButton.onClick.AddListener(OnInfoButtonClick);
            _tutorialButton.onClick.AddListener(OnTutorialButtonClick);
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            _infoButton.onClick.RemoveListener(OnInfoButtonClick);
            _tutorialButton.onClick.RemoveListener(OnTutorialButtonClick);
        }

        private void LateUpdate()
        {
            // if (_gameStatus == GameStatus.Game || _gameStatus == GameStatus.Tutorial)
            // {
            //     return;
            // }

            // if (Input.GetKeyDown(KeyCode.Return))
            // {
            //     if (_gameStatus == GameStatus.Instruction)
            //     {
            //         return;
            //     }
            //
            //     OnStartGameButtonClick();
            // }

            // if (Input.GetKeyDown(KeyCode.I))
            // {
            //     if (_isInstructionOpened)
            //     {
            //         OnInfoClosed();
            //     }
            //     else
            //     {
            //         OnInfoButtonClick();
            //     }
            // }

            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     if (_isTutorialOpened)
            //     {
            //         OnTutorialClose();
            //     }
            //     else
            //     {
            //         OnTutorialButtonClick();
            //     }
            // }
        }

        public void InvokeStartGame()
        {
            if (_gameStatus == GameStatus.Instruction)
            {
                return;
            }

            OnStartGameButtonClick();
        }

        public void InvokeTutorialClick()
        {
            if (_isTutorialOpened)
            {
                OnTutorialClose();
            }
            else
            {
                OnTutorialButtonClick();
            }
        }

        public void InvokeInfoClick()
        {
            if (_isInstructionOpened)
            {
                OnInfoClosed();
            }
            else
            {
                OnInfoButtonClick();
            }
        }

        public void OnTutorialClose()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _isTutorialOpened = false;
            _gameStatus = GameStatus.Menu;
            _initialMenu.SetActive(true);
            _tutorial.gameObject.SetActive(false);
        }
        
        public void InvokeQuitGame()
        {
            AudioManager.Instance.PlaySfx(AudioType.MenuClick);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnInfoButtonClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _isInstructionOpened = true;
            _gameStatus = GameStatus.Instruction;
            _instructions.SetActive(true);
            _initialMenu.SetActive(false);
            _instructions.OnInstructionsClosed += OnInfoClosed;
        }

        private void OnInfoClosed()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _gameStatus = GameStatus.Menu;
            _isInstructionOpened = false;
            _initialMenu.SetActive(true);
            _instructions.SetActive(false);
            _instructions.OnInstructionsClosed -= OnInfoClosed;
        }

        private void OnTutorialButtonClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _tutorial.Initialize(this);
            _gameStatus = GameStatus.Tutorial;
            _tutorial.gameObject.SetActive(true);
            _initialMenu.SetActive(false);
        }

        private IEnumerator PlayerAnimation()
        {
            int index = 0;
            while (true)
            {
                if (index == 0)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                _mainImage.sprite = _sprites[index];
                yield return new WaitForSeconds(0.3f);
            }
        }

        private void OnStartGameButtonClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            KeepDataManager.Instance.WasGameRun = true;
            GameController.Instance.SwitchGameState(GameState.Play);
            _gameStatus = GameStatus.Game;
            StopCoroutine(nameof(PlayerAnimation));
            InitialMenuSetActive(false);
        }

        private void InitialMenuSetActive(bool toActivate)
        {
            _initialMenu.SetActive(toActivate);
            _mainGame.SetActive(!toActivate);
        }


    }
}