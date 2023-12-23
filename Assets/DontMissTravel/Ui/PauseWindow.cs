using UnityEngine;
using UnityEngine.UI;

namespace DontMissTravel.Ui
{
    public class PauseWindow : Window
    {
        [SerializeField] private Button _resumeGameButton;
        [SerializeField] private Button _soundMuteButton;
        [SerializeField] private Button _musicMuteButton;

        [Space] [SerializeField] private GameObject _soundMuteGameObject;
        [SerializeField] private GameObject _musicMuteGameObject;

        private Hud _hud;

        private void Start()
        {
            _hud = Singleton<Hud>.Instance;
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
            _hud.OnPauseClick();
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