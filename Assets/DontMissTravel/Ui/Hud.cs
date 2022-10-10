using System.Data;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;
using GameState = DontMissTravel.Data.GameState;
using WindowName = DontMissTravel.Data.WindowName;

namespace DontMissTravel.Ui
{
    public class Hud : MonoBehaviour
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
        
#region Properties

        public static Hud Instance { get; set; }

        public GameObject Controller
        {
            get => _controller;
            set => _controller = value;
        }

        public GameObject Header
        {
            get => _sideInfo;
            set => _sideInfo = value;
        }

        public GameObject Player
        {
            get => _player;
            set => _player = value;
        }

        public GameObject Playground
        {
            get => _playground;
            set => _playground = value;
        }

        public GameObject PullButton
        {
            get => _pullButton;
            set => _pullButton = value;
        }

#endregion

        private void Awake()
        {
            Instance = this;
            _pauseButton.onClick.AddListener(OnPauseClick);
            SetAllWindowsInvisible();
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
            ShowHideWindow(WindowName.Pause, true);
            GameController.Instance.SwitchGameState(GameState.Pause);
        }

        private void SetAllWindowsInvisible()
        {
            _loseWindow.SetActiveImmediately(false);
            _winWindow.SetActiveImmediately(false);
            _pauseWindow.SetActiveImmediately(false);
        }

        private GameState _previousGameState;
        
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