using System;
using System.Globalization;
using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        private const float ZoomOutPosition = -620f;
        private const float DecreaseTimeGate = 15f;
        private const float TutorialGameTime = 90f;

        [SerializeField] private Camera _mainCamera;
        [Space] [SerializeField] private TutorialGame _tutorialGameController;
        [SerializeField] private TutorialHud _tutorialHud;
        [Space] [SerializeField] private Camera _playerCamera;
        [SerializeField] private Camera _tutorialCamera;
        [SerializeField] private TutorialBonus _tutorialBonus;

        private GameSystemManager _gameSystemManager;

        private void Start()
        {
            _mainCamera.transform.position = new Vector3(0f, 0f, ZoomOutPosition);
        }

        private void OnEnable()
        {
            _tutorialHud.OnFullTutorialCompleted += OnTutorialCompleted;
            _tutorialHud.TutorialStateChanged += OnTutorialStateChanged;
            _tutorialGameController.OnTutorialTimeOut += OnGameTimeOut;

            _tutorialGameController.gameObject.SetActive(false);
            _tutorialGameController.ShowObstacle(false);
            _tutorialGameController.ShowEnemy(false);
            
            _tutorialGameController.SetCustomGameTime(TutorialGameTime);
            _tutorialHud.SetGateWillOpenTime(TutorialGameTime);
            _tutorialHud.ChangeGateState(GateState.WillOpen, TutorialGameTime.ToString(CultureInfo.InvariantCulture));
            _tutorialHud.PrepareTutorialInfo();
            _tutorialGameController.InitGame();
        }

        private void OnDisable()
        {
            _tutorialHud.OnFullTutorialCompleted -= OnTutorialCompleted;
            _tutorialHud.TutorialStateChanged -= OnTutorialStateChanged;
            _tutorialGameController.OnTutorialTimeOut -= OnGameTimeOut;
        }

        public void Initialize(GameSystemManager gameSystemManager)
        {
            _gameSystemManager = gameSystemManager;
        }
        
        private void OnGameTimeOut()
        {
            _tutorialHud.ShowTimeIsOut();
            _tutorialHud.ChangeGateState(GateState.Closed, null);
            _tutorialGameController.SwitchGameState(GameState.Pause);
            _tutorialGameController.SetCustomGameTime(TutorialGameTime);
        }

        private void OnTutorialStateChanged(TutorialState previousState)
        {
            Debug.Log($"[{name}]. Tutorial changed -> Previous state: {previousState}");
            switch (previousState)
            {
                case TutorialState.Welcome:
                    _tutorialHud.SetGateWillOpenTime(TutorialGameTime);
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.DepartureIn:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Level:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Movement:
                    _tutorialGameController.SwitchGameState(GameState.Tutorial);
                    OnFirstPartTutorialCompleted();
                    break;
                case TutorialState.Obstacles:
                    _tutorialGameController.SwitchGameState(GameState.Tutorial);
                    OnMovableTutorialCompleteHud();
                    StartObstaclesTutorial();
                    break;
                case TutorialState.Enemy:
                    _tutorialGameController.SwitchGameState(GameState.Tutorial);
                    _tutorialGameController.ShowObstacle(false);
                    _tutorialGameController.ShowEnemy(true);
                    _tutorialGameController.OnMetEnemy += OnMetEnemy;
                    break;
                case TutorialState.HideGamePreview:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    _tutorialCamera.gameObject.SetActive(false);
                    _tutorialHud.Playground.SetActive(false);
                    _tutorialHud.Player.SetActive(false);
                    _tutorialGameController.PrepareHideGame();
                    break;
                case TutorialState.HideGame:
                    _tutorialGameController.SwitchGameState(GameState.HideGame);
                    _tutorialGameController.StartHideGame();
                    _tutorialGameController.OnMeetOver += OnMeetOver;
                    break;
                case TutorialState.BonusPreview:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    _tutorialCamera.gameObject.SetActive(true);
                    _tutorialHud.Playground.SetActive(true);
                    _tutorialHud.Player.SetActive(true);
                    break;
                case TutorialState.Bonus:
                    _tutorialGameController.SwitchGameState(GameState.Tutorial);
                    _tutorialGameController.ShowBonus();
                    _tutorialBonus.ChangeBonusLifeTime();
                    _tutorialBonus.OnBonusApplied += OnBonusApplied;
                    _tutorialGameController.OnPlayerReachGate += OnPlayerReachedGate;
                    break;
                case TutorialState.Remark:
                    _tutorialGameController.ForceStopPlayer();
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    break;
                case TutorialState.Gate:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    _tutorialGameController.OpenGateDecreaseMaxTime(DecreaseTimeGate);
                    break;
                case TutorialState.TutorialGame:
                    _tutorialGameController.SwitchGameState(GameState.Tutorial);
                    break;
                case TutorialState.Complete:
                    _tutorialGameController.SwitchGameState(GameState.Pause);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(previousState), previousState, null);
            }
        }

        private void OnPlayerReachedGate()
        {
            if (_tutorialHud.CurrentState != TutorialState.TutorialGame)
            {
                _tutorialHud.OnTutorialStageNext(_tutorialHud.CurrentState);
                return;
            }
            
            _tutorialHud.OnTutorialStageNext(TutorialState.TutorialGame);
            _tutorialGameController.OnPlayerReachGate -= OnPlayerReachedGate;
        }

        private void OnBonusApplied()
        {
            _tutorialHud.OnTutorialStageNext(TutorialState.Bonus);
            _tutorialBonus.OnBonusApplied -= OnBonusApplied;
        }

        private void OnMetEnemy()
        {
            _tutorialHud.OnTutorialStageNext(TutorialState.Enemy);
            _tutorialGameController.OnMetEnemy -= OnMetEnemy;
        }

        private void OnMeetOver()
        {
            _tutorialHud.OnTutorialStageNext(TutorialState.HideGame);
            _tutorialGameController.OnMeetOver -= OnMeetOver;
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
            _tutorialHud.OnTutorialStageNext(TutorialState.Movement);
            _tutorialGameController.OnMovableTutorialComplete -= OnMovableTutorialComplete;
        }

        private void OnTutorialCompleted()
        {
            _gameSystemManager.OnTutorialClose();
        }

        public void OnFirstPartCompleteEditor()
        {
#region Editor

#if UNITY_EDITOR
            OnFirstPartTutorialCompleted();
#endif

#endregion
        }
    }
}