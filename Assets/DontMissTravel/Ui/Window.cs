using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DontMissTravel.Ui
{
    public class Window : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [Space]
        [SerializeField] private Button _restartGame;
        [SerializeField] private Button _quitGame;
        
        
        private const float AnimationDuration = 0.5f;

        protected virtual void OnEnable()
        {
            _restartGame.onClick.AddListener(OnRestartClick);
            _quitGame.onClick.AddListener(OnQuitClick);
        }

        protected virtual void OnDisable()
        {
            _restartGame.onClick.RemoveListener(OnRestartClick);
            _quitGame.onClick.RemoveListener(OnQuitClick);
        }

        public virtual void SetActive(bool toActivate)
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

        private void OnRestartClick()
        {
            SceneManager.LoadScene("MainScene");
        }

        private void OnQuitClick()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}