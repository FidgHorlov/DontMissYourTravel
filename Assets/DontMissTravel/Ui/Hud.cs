using System;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class Hud : MonoBehaviour
    {
        public event Action OnMainMenuClicked;

        private static Hud _instance;

        [SerializeField] private Button _pauseButton;

        [Space] [SerializeField] private GameObject _sideInfo;
        [SerializeField] private GameObject _playground;
        [SerializeField] private GameObject _pullButton;

        [Space] [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _controller;

        [Space] [SerializeField] private PauseWindow _pauseWindow;
        [SerializeField] private Window _loseWindow;
        [SerializeField] private Window _winWindow;

        [Space] [SerializeField] private InfoPanel _infoPanel;

        private GameState _previousGameState;
        private bool _onPause;

#region Properties

        public static Hud Instance => _instance;
        public GameObject Controller => _controller;
        public GameObject Player => _player;
        public GameObject Playground => _playground;
        public GameObject PullButton => _pullButton;

#endregion

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(_instance.gameObject);
            }

            _instance = this;
            _pauseButton.onClick.AddListener(OnPauseClick);
            SetAllWindowsInvisible();
        }

        private void LateUpdate()
        {
            if (_loseWindow.IsActive || _winWindow.IsActive)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    AudioManager.Instance.PlaySfx(AudioType.MenuClick);
                    GameController.Instance.Restart();
                    return;   
                }
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    InvokeMainMenuClick();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return) && _onPause)
                {
                    OnPauseClick();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Escape) && !_onPause)
                {
                    OnPauseClick();
                }
            }
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPauseClick);
        }

        public void ChangeGateState(GateState gateState, string text)
        {
            _infoPanel.ChangeGateState(gateState, text);
        }

        public void SetGateWillOpenTime(float time)
        {
            _infoPanel.SetGateOpenTime(time);
        }

        public void SetGateWillClose(float inTime)
        {
            _infoPanel.SetDepartureTime(inTime);
        }

        public void PrepareTutorialInfo()
        {
            _infoPanel.PrepareTutorialInfo();
        }

        private void OnPauseClick()
        {
            AudioManager.Instance.PlaySfx(AudioType.MenuClick);
            _onPause = !_onPause;
            ShowHideWindow(WindowName.Pause, _onPause);
            if (_onPause)
            {
                _previousGameState = GameController.Instance.GameState;
            }

            GameController.Instance.SwitchGameState(_onPause ? GameState.Pause : _previousGameState);
        }

        private void SetAllWindowsInvisible()
        {
            _loseWindow.SetActiveImmediately(false);
            _winWindow.SetActiveImmediately(false);
            _pauseWindow.SetActiveImmediately(false);
        }

        public void ShowHideWindow(WindowName window, bool toShow)
        {
            Window targetWindow = null;
            switch (window)
            {
                case WindowName.Lose:
                {
                    targetWindow = _loseWindow;
                    break;
                }
                case WindowName.Win:
                {
                    targetWindow = _winWindow;
                    break;
                }
                case WindowName.Pause:
                {
                    targetWindow = _pauseWindow;
                    break;
                }
            }

            if (targetWindow != null)
            {
                targetWindow.SetActive(toActivate: toShow);
            }
        }

        public void SetCurrentLevel(int currentLevel)
        {
            _infoPanel.SetCurrentLevel(currentLevel);
        }

        public void SetDelay(float randDelay)
        {
            _infoPanel.SetBonus(Constants.InformationTexts.DelayCaption, "+" + randDelay.ToString("##"));
        }

        public void SetBoost()
        {
            _infoPanel.SetBonus(Constants.InformationTexts.SpeedBoosted);
        }

        public void SetGreenTicket()
        {
            _infoPanel.SetBonus(Constants.InformationTexts.GreenTicket);
        }

        public void InvokeMainMenuClick()
        {
            OnMainMenuClicked?.Invoke();
        }
    }
}