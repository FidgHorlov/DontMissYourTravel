using System;
using System.Collections.Generic;
using System.Globalization;
using DontMissTravel.Data;
using DontMissTravel.Ui;
using UnityEngine;
using UnityEngine.Serialization;

namespace DontMissTravel.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        private const float ZoomOutPosition = -620f;
        private const float DecreaseTimeGate = 15f;
        private const float TutorialGameTime = 90f;

        [SerializeField] private Camera _mainCamera;
        [Space] [SerializeField] private TutorialGame _tutorialGameController;
        [SerializeField] private TutorialScenario _tutorialScenario;
        [Space] [SerializeField] private Camera _playerCamera;
        [SerializeField] private Camera _tutorialCamera;
        [Space] [SerializeField] private TutorialBonus _tutorialBonus;
        [SerializeField] private List<Transform> _bonusPositions;

        private GameSystemManager _gameSystemManager;
        private GameController _gameController;
        private KeepDataManager _keepDataManager;
        private Hud _hud;

        private void Start()
        {
            _mainCamera.transform.position = new Vector3(0f, 0f, ZoomOutPosition);
            _gameSystemManager = Singleton<GameSystemManager>.Instance;
            _hud = Singleton<Hud>.Instance;
            _gameController = Singleton<GameController>.Instance;
            _keepDataManager = Singleton<KeepDataManager>.Instance;

            _hud.SetGateWillOpenTime(TutorialGameTime);
            _hud.ChangeGateState(GateState.WillOpen, TutorialGameTime.ToString(CultureInfo.InvariantCulture));
            _hud.PrepareTutorialInfo();

            _tutorialScenario.OnFullTutorialCompleted += OnTutorialCompleted;
            _tutorialScenario.TutorialStateChanged += OnTutorialStateChanged;
            _gameController.OnTutorialTimeOut += OnGameTimeOut;
            _gameController.SetCustomGameTime(TutorialGameTime);
        }

        private void OnEnable()
        {
            _tutorialGameController.gameObject.SetActive(false);
            _tutorialGameController.ShowObstacle(false);
            _tutorialGameController.ShowEnemy(false);
            _tutorialGameController.InitGame();
        }

        private void OnDestroy()
        {
            _tutorialScenario.OnFullTutorialCompleted -= OnTutorialCompleted;
            _tutorialScenario.TutorialStateChanged -= OnTutorialStateChanged;
            _gameController.OnTutorialTimeOut -= OnGameTimeOut;
        }

        private void OnGameTimeOut()
        {
            _tutorialScenario.ShowTimeIsOut();
            _hud.ChangeGateState(GateState.Closed, null);
            _keepDataManager.SwitchGameState(GameState.Pause);
            _gameController.SetCustomGameTime(TutorialGameTime);
        }

        private void OnTutorialStateChanged(TutorialState previousState)
        {
            Debug.Log($"[{name}]. Tutorial changed -> Previous state: {previousState}");
            switch (previousState)
            {
                case TutorialState.Welcome:
                    _hud.SetGateWillOpenTime(TutorialGameTime);
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.DepartureIn:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Level:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Movement:
                    _keepDataManager.SwitchGameState(GameState.Play);
                    OnFirstPartTutorialCompleted();
                    break;
                case TutorialState.Obstacles:
                    _keepDataManager.SwitchGameState(GameState.Play);
                    OnMovableTutorialCompleteHud();
                    StartObstaclesTutorial();
                    break;
                case TutorialState.Enemy:
                    _keepDataManager.SwitchGameState(GameState.Play);
                    _tutorialGameController.ShowObstacle(false);
                    _tutorialGameController.ShowEnemy(true);
                    _tutorialGameController.OnMetEnemy += OnMetEnemy;
                    break;
                case TutorialState.HideGamePreview:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    _tutorialCamera.gameObject.SetActive(false);
                    _hud.HidePlayerAndPlayground(toHide: true);
                    _gameController.TutorialPrepareHideGame();
                    break;
                case TutorialState.HideGame:
                    _keepDataManager.SwitchGameState(GameState.HideGame);
                    _gameController.TutorialStartHideGame();
                    _gameController.OnTutorialMeetOver += OnMeetOver;
                    break;
                case TutorialState.BonusPreview:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    _tutorialCamera.gameObject.SetActive(true);
                    _hud.HidePlayerAndPlayground(toHide: false);
                    break;
                case TutorialState.Bonus:
                    _keepDataManager.SwitchGameState(GameState.Play);
                    _tutorialBonus.ChangeBonusLifeTime();
                    ShowTutorialBonus();
                    _tutorialBonus.OnBonusApplied += OnBonusApplied;
                    _gameController.OnTutorialReachGate += OnPlayerReachedGate;
                    break;
                case TutorialState.Remark:
                    _tutorialGameController.ForceStopPlayer();
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Gate:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    _gameController.OpenGateDecreaseMaxTime(DecreaseTimeGate);
                    break;
                case TutorialState.TutorialGame:
                    _keepDataManager.SwitchGameState(GameState.Play);
                    break;
                case TutorialState.Complete:
                    _keepDataManager.SwitchGameState(GameState.Pause);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(previousState), previousState, null);
            }
        }

        private void ShowTutorialBonus()
        {
            foreach (Transform bonusPosition in _bonusPositions)
            {
                _gameController.AddAvailablePositions(bonusPosition.position);
            }
            
            _tutorialBonus.SetActiveInTime(true, _gameController.GetRandomAvailablePosition());
        }

        private void OnPlayerReachedGate()
        {
            if (_tutorialScenario.CurrentState != TutorialState.TutorialGame)
            {
                _tutorialScenario.OnTutorialStageNext(_tutorialScenario.CurrentState);
                return;
            }

            _tutorialScenario.OnTutorialStageNext(TutorialState.TutorialGame);
            _gameController.OnTutorialReachGate -= OnPlayerReachedGate;
        }

        private void OnBonusApplied()
        {
            _tutorialScenario.OnTutorialStageNext(TutorialState.Bonus);
            _tutorialBonus.OnBonusApplied -= OnBonusApplied;
        }

        private void OnMetEnemy()
        {
            _tutorialScenario.OnTutorialStageNext(TutorialState.Enemy);
            _tutorialGameController.OnMetEnemy -= OnMetEnemy;
        }

        private void OnMeetOver()
        {
            _tutorialScenario.OnTutorialStageNext(TutorialState.HideGame);
            _gameController.OnTutorialMeetOver -= OnMeetOver;
        }

        private void StartObstaclesTutorial()
        {
            _tutorialGameController.ShowObstacle(true);
        }

        private void OnFirstPartTutorialCompleted()
        {
            _tutorialGameController.gameObject.SetActive(true);
            _playerCamera.gameObject.SetActive(false);
            _tutorialCamera.gameObject.SetActive(true);
            _tutorialGameController.StartMovableTutorial();
            _tutorialGameController.OnMovableTutorialComplete += OnMovableTutorialComplete;
        }

        private void OnMovableTutorialCompleteHud()
        {
            _tutorialGameController.StopMovableTutorialCompleting();
            _tutorialGameController.OnMovableTutorialComplete -= OnMovableTutorialComplete;
        }

        private void OnMovableTutorialComplete()
        {
            _tutorialScenario.OnTutorialStageNext(TutorialState.Movement);
            _tutorialGameController.OnMovableTutorialComplete -= OnMovableTutorialComplete;
        }

        private void OnTutorialCompleted()
        {
            _gameSystemManager.ToggleTutorial(toOpen: false);
        }

#region Editor

        public void OnFirstPartCompleteEditor()
        {
#if UNITY_EDITOR
            OnFirstPartTutorialCompleted();
#endif
        }

#endregion
    }
}