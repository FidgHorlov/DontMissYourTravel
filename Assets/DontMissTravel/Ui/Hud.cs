using System;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using DontMissTravel.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class Hud : Singleton<Hud>, IHud
    {
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
        private bool _isPaused;
        private GameController _gameController;
        private AudioManager _audioManager;
        private KeepDataManager _keepDataManager;

        public bool IsPaused => _isPaused;

        protected override void Awake()
        {
            base.Awake();
            SetAllWindowsInvisible();
        }

        private void Start()
        {
            _gameController = Singleton<GameController>.Instance;
            _audioManager = Singleton<AudioManager>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;
            _controller.SetActive(Singleton<KeepDataManager>.Instance.IsPhone);   
        }

        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(OnPauseClick);
        }

        private void OnDisable()
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

        public void OnPauseClick()
        {
            _audioManager.PlaySfx(AudioType.MenuClick);
            _isPaused = !_isPaused;
            ShowHideWindow(WindowName.Pause, _isPaused);
            if (_isPaused)
            {
                _previousGameState = _keepDataManager.GameState;
            }

            _keepDataManager.SwitchGameState(_isPaused ? GameState.Pause : _previousGameState);
        }

        // using for the Hide Game
        public void HidePlayerAndPlayground(bool toHide)
        {
            _player.SetActive(!toHide);
            _playground.SetActive(!toHide);
        }

        public void TogglePullButton(bool toActivate)
        {
            _pullButton.SetActive(toActivate);
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
    }
}