using DG.Tweening;
using DontMissTravel.Audio;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;

namespace DontMissTravel.Ui
{
    public class Window : MonoBehaviour
    {
        private const float AnimationDuration = 0.5f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [Space] [SerializeField] private Button _restartGame;
        [SerializeField] private Button _mainMenu;

        private GameSystemManager _gameSystemManager;
        private AudioManager _audioManager;

        protected virtual void Start()
        {
            _gameSystemManager = Singleton<GameSystemManager>.Instance;
            _audioManager = Singleton<AudioManager>.Instance;
        }

        protected virtual void OnEnable()
        {
            _restartGame.onClick.AddListener(OnRestartClick);
            _mainMenu.onClick.AddListener(OnMainMenuClick);
        }

        protected virtual void OnDisable()
        {
            _restartGame.onClick.RemoveListener(OnRestartClick);
            _mainMenu.onClick.RemoveListener(OnMainMenuClick);
        }

        internal void SetActive(bool toActivate)
        {
            float targetVisible = toActivate ? 1f : 0f;
            _canvasGroup.DOKill(_canvasGroup);
            if (toActivate)
            {
                _canvasGroup.gameObject.SetActive(true);
            }

            _canvasGroup
                .DOFade(targetVisible, AnimationDuration)
                .OnComplete(() =>
                {
                    if (!toActivate)
                    {
                        _canvasGroup.gameObject.SetActive(false);
                    }
                })
                .SetId(_canvasGroup);
        }

        public void SetActiveImmediately(bool toActivate)
        {
            _canvasGroup.alpha = toActivate ? 1f : 0f;
            gameObject.SetActive(toActivate);
        }

        private void OnMainMenuClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _gameSystemManager.MainMenu();
        }

        private void OnRestartClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _gameSystemManager.RestartGame();
        }
    }
}