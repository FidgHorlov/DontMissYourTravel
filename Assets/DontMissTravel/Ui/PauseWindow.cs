using DontMissTravel.Audio;
using UnityEngine;
using UnityEngine.UI;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class PauseWindow : Window
    {
        [SerializeField] private Button _resumeGameButton;
        [SerializeField] private Button _soundMuteButton;
        [SerializeField] private Button _musicMuteButton;

        [Space] [SerializeField] private GameObject _soundMuteGameObject;
        [SerializeField] private GameObject _musicMuteGameObject;

        private GameController _gameController;
        private AudioManager _audioManager;
        private GameState _previousGameState;

        private GameController GameController
        {
            get
            {
                if (_gameController == null)
                {
                    _gameController = GameController.Instance;
                }

                return _gameController;
            }
        }

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

        public override void SetActive(bool toActivate)
        {
            base.SetActive(toActivate);
            if (toActivate)
            {
                _previousGameState = GameController.GameState;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _resumeGameButton.onClick.AddListener(OnResumeClick);
            _soundMuteButton.onClick.AddListener(OnSoundMuteClick);
            _musicMuteButton.onClick.AddListener(OnMusicMuteClick);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            _resumeGameButton.onClick.RemoveListener(OnResumeClick);
            _soundMuteButton.onClick.RemoveListener(OnSoundMuteClick);
            _musicMuteButton.onClick.RemoveListener(OnMusicMuteClick);
        }

        private void OnResumeClick()
        {
            Hud.ShowHideWindow(WindowName.Pause, false);
            GameController.SwitchGameState(_previousGameState);
        }

        private void OnMusicMuteClick()
        {
            _musicMuteGameObject.SetActive(AudioManager.ToggleMusic());
        }

        private void OnSoundMuteClick()
        {
            _soundMuteGameObject.SetActive(AudioManager.ToggleSound());
        }
    }
}