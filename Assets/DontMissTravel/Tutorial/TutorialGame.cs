using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Data;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DontMissTravel.Tutorial
{
    public class TutorialGame : GameController
    {
        public event Action OnMovableTutorialComplete;
        public event Action OnMetEnemy;
        public event Action OnMeetOver;
        public event Action OnPlayerReachGate;
        
        private const float DelayBeforeNextTutorial = 10f;

        [SerializeField] private GameObject _obstacle;
        [SerializeField] private TutorialPlayer _tutorialPlayer;
        [SerializeField] private List<TutorialEnemy> _enemies;
        [SerializeField] private List<Transform> _bonusPositionList;

        private EnemyName _metEnemyName;

        public void StartMovableTutorial()
        {
            StartCoroutine(MovableTutorialCompleteDelay(MovableTutorialCompleteInvoke));
        }

        public void ShowObstacle(bool toShow)
        {
            _obstacle.SetActive(toShow);
        }

        public void ShowEnemy(bool toShow)
        {
            if (toShow)
            {
                ShowRandomEnemy();
                return;
            }

            foreach (TutorialEnemy enemy in _enemies)
            {
                enemy.SetActive(false);
            }
        }

        public void PrepareHideGame()
        {
            _hideGame.gameObject.SetActive(true);
            _hideGame.enabled = false;
        }

        public void StartHideGame()
        {
            _hideGame.enabled = true;
            _hideGame.StartHideGame(_metEnemyName);
        }

        public void CloseHideGame()
        {
            _hideGame.CloseHideGame();
            _tutorialPlayer.OnMeetFinish();
            OnMeetOver?.Invoke();
        }

        public void ForceStopPlayer()
        {
            _tutorialPlayer.ForceStop();
        }

        protected override void OnPlayerReachedGate()
        {
            OnPlayerReachGate?.Invoke();
        }

        private void ShowRandomEnemy()
        {
            int rand = Random.Range(0, _enemies.Count);
            _enemies[rand].SetActive(true);
            _tutorialPlayer.OnEnemyMeet += OnEnemyMeet;
        }

        private void OnEnemyMeet(EnemyName enemyName)
        {
            _metEnemyName = enemyName;
            OnMetEnemy?.Invoke();
            _tutorialPlayer.OnEnemyMeet -= OnEnemyMeet;
            
            TutorialEnemy enem = _enemies.FirstOrDefault(enemy => enemy.EnemyName == _metEnemyName);
            if (enem == null)
            {
                Debug.LogError($"[{name}]. Something wrong.");
                return;
            }
            
            if (enem.gameObject.activeSelf)
            {
                enem.gameObject.SetActive(false);
            }
        }

        private IEnumerator MovableTutorialCompleteDelay(UnityAction onWaitComplete)
        {
            yield return new WaitForSeconds(DelayBeforeNextTutorial);
            onWaitComplete?.Invoke();
        }

        public void StopMovableTutorialCompleting()
        {
            StopCoroutine(nameof(MovableTutorialCompleteDelay));
        }

        private void MovableTutorialCompleteInvoke()
        {
            OnMovableTutorialComplete?.Invoke();
        }

        public void ShowBonus()
        {
            foreach (Transform bonusPosition in _bonusPositionList)
            {
                AddAvailablePositions(bonusPosition.position);
            }
            
            CreateBonus();
        }
    }
}