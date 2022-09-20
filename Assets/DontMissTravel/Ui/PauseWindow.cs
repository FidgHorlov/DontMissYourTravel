using UnityEngine;
using UnityEngine.UI;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class PauseWindow : Window
    {
        [SerializeField] private Button _resumeGame;
        [SerializeField] private Button _soundMute;
        [SerializeField] private Button _musicMute;

        private GameController _gameController;
        private GameState _previousGameState;

        private void Awake()
        {
            _gameController = GameController.Instance;
        }

        public override void SetActive(bool toActivate)
        {
            base.SetActive(toActivate);
            if (toActivate)
            {
                _previousGameState = _gameController.GameState;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _resumeGame.onClick.AddListener(OnResumeClick);
            _soundMute.onClick.AddListener(OnSoundMuteClick);
            _musicMute.onClick.AddListener(OnMusicMuteClick);
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            _resumeGame.onClick.RemoveListener(OnResumeClick);
            _soundMute.onClick.RemoveListener(OnSoundMuteClick);
            _musicMute.onClick.RemoveListener(OnMusicMuteClick);
        }

        private void OnResumeClick()
        {
            Hud.Instance.ShowHideWindow(WindowName.Pause, false);
            _gameController.SwitchGameState(_previousGameState);
        }
        
        private void OnMusicMuteClick()
        {
            _gameController.MusicOnOff();
        }

        private void OnSoundMuteClick()
        {
            _gameController.SoundOnOff();           
        }
    }
}