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
    public class TutorialGame : MonoBehaviour
    {
        public event Action OnMovableTutorialComplete;
        public event Action OnMetEnemy;
        
        private const float DelayBeforeNextTutorial = 10f;

        [SerializeField] private GameObject _obstacle;
        [SerializeField] private TutorialPlayer _tutorialPlayer;
        [SerializeField] private List<TutorialEnemy> _enemies;
        
        private GameController _gameController;

        private EnemyName _metEnemyName;

        private void Start()
        {
            _gameController = Singleton<GameController>.Instance;
        }

        public void InitGame()
        {
            _tutorialPlayer.SetDefaultPosition();
            _tutorialPlayer.SetPossibleEnemies(_enemies);
        }

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

        public void ForceStopPlayer()
        {
            _tutorialPlayer.ForceStop();
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
            _gameController.TutorialSetHideGameEnemy(enemyName);

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
    }
}