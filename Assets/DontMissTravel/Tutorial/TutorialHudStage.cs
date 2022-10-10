using System;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.UI;

namespace DontMissTravel.Tutorial
{
    public class TutorialHudStage : MonoBehaviour
    {
        public event Action<TutorialState> OnContinueTutorial;
        [SerializeField] private TutorialState _tutorialState;
        [SerializeField] private Button _continueButton;
        public int StageIndex { get; set; }
        public TutorialState TutorialState => _tutorialState;

        private void Start()
        {
            _continueButton?.onClick.AddListener(()=> OnContinueTutorial?.Invoke(_tutorialState));
        }

        private void OnDestroy()
        {
            _continueButton?.onClick.RemoveAllListeners();
        }

        public void SetActive(bool toActivate)
        {
            gameObject.SetActive(toActivate);
        }
    }
}