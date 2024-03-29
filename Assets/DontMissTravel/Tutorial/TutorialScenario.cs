using System;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using UnityEngine;
using AudioType = DontMissTravel.Audio.AudioType;

namespace DontMissTravel.Tutorial
{
    public class TutorialScenario : MonoBehaviour
    {
        public event Action OnFullTutorialCompleted;
        public event Action<TutorialState> TutorialStateChanged;
        
        [Space]
        [SerializeField] private TutorialHudStage _timeIsOut;
        [SerializeField] private List<TutorialHudStage> _tutorialHudStages;

        private AudioManager _audioManager;
        private TutorialState _currentState;
        
        public TutorialState CurrentState => _currentState;

        private void OnEnable()
        {
            _currentState = 0;
            HideAllStagesExcept(_tutorialHudStages[0]);
            foreach (TutorialHudStage hudStage in _tutorialHudStages)
            {
                hudStage.OnContinueTutorial += OnTutorialStageNext;
            }

            _timeIsOut.OnContinueTutorial += OnTimeIsOut;
        }

        private void OnDisable()
        {
            foreach (TutorialHudStage hudStage in _tutorialHudStages)
            {
                hudStage.OnContinueTutorial -= OnTutorialStageNext;
            }
            
            _timeIsOut.OnContinueTutorial -= OnTimeIsOut;
        }

        private void Start()
        {
            _audioManager = Singleton<AudioManager>.Instance;
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!IsPossibleSkip())
                {
                    return;
                }
                
                OnTutorialStageNext(_currentState);
            }
        }
        
        private void OnTimeIsOut(TutorialState tutorialState)
        {
            OnFullTutorialCompleted?.Invoke();
        }

        private bool IsPossibleSkip()
        {
            switch (_currentState)
            {
                case TutorialState.Enemy:
                case TutorialState.HideGame:
                case TutorialState.Bonus:
                case TutorialState.TutorialGame:
                    return false;
                default:
                    return true;
            }
        }

        public void OnTutorialStageNext(TutorialState currentTutorialState)
        {
            TutorialHudStage tutorial = _tutorialHudStages.FirstOrDefault(tut => tut.TutorialState == currentTutorialState);
            if (tutorial != null)
            {
                tutorial.SetActive(false);
                tutorial.OnContinueTutorial -= OnTutorialStageNext;
            }

            TutorialState newTutorialState = currentTutorialState;
            newTutorialState++;
            SwitchTutorial(newTutorialState);
        }

        public void ShowTimeIsOut()
        {
            HideAllStagesExcept(_timeIsOut);
        }

        private void HideAllStagesExcept(TutorialHudStage tutorialHudStage)
        {
            foreach (TutorialHudStage hudStage in _tutorialHudStages)
            {
                hudStage.SetActive(hudStage.Equals(tutorialHudStage));
            }

            _timeIsOut.SetActive(tutorialHudStage == _timeIsOut);
        }

        private void SwitchTutorial(TutorialState nextTutorial)
        {
            TutorialHudStage tutorialState = _tutorialHudStages.FirstOrDefault(tutorial => tutorial.TutorialState == nextTutorial);
            if (tutorialState != null)
            {
                tutorialState.SetActive(true);
            }
            else
            {
                OnFullTutorialCompleted?.Invoke();
                return;
            }

            TutorialStateChanged?.Invoke(nextTutorial);
            Debug.Log($"Current tutorial state: {nextTutorial}");
            _currentState = nextTutorial;
            _audioManager.PlaySfx(AudioType.MenuClick);
        }
    }
}