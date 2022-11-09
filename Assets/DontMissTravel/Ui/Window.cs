using System;
using DG.Tweening;
using DontMissTravel.Audio;
using UnityEditor;
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

        private Hud _hud;
        public bool IsActive { get; private set; }

        protected Hud Hud
        {
            get
            {
                if (_hud != null)
                    return _hud;

                _hud = Hud.Instance;
                return _hud;
            }
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

        public virtual void SetActive(bool toActivate)
        {
            IsActive = toActivate;
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
            AudioManager.Instance.PlaySfx(AudioType.MenuClick);
            Hud.InvokeMainMenuClick();
        }

        private void OnRestartClick()
        {
            AudioManager.Instance.PlaySfx(AudioType.MenuClick);
            GameController.Instance.Restart();
        }
    }
}