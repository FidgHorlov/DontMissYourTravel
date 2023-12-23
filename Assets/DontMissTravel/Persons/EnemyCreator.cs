using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DontMissTravel.Persons
{
    public class EnemyCreator : MonoBehaviour
    {
        [SerializeField] List<EnemySettings> _enemySettings;
        [SerializeField] private List<Enemy> _enemiesList;

        private GameController _gameController;

        private IEnumerator Start()
        {
            Debug.Log($"Enemies: {_enemiesList.Count}");

            _gameController = Singleton<GameController>.Instance;

            int maxEnemies = Singleton<LevelGenerator>.Instance.CurrentLevel;
            Debug.LogWarning($"Level is: {maxEnemies}");

            if (maxEnemies > Constants.EnemiesParameters.MaximumEnemies)
            {
                maxEnemies = Constants.EnemiesParameters.MaximumEnemies;
                if (maxEnemies > _enemiesList.Count)
                {
                    Debug.LogWarning($"Please, add more enemies dummy into the object pool.");
                }
            }

            yield return ShowEnemies(maxEnemies);
        }

        private IEnumerator ShowEnemies(int maxEnemies)
        {
            for (int index = 0; index < maxEnemies; index++)
            {
                if (index >= _enemiesList.Count)
                {
                    Debug.LogWarning($"Please, add more enemies dummy into the object pool. Index: {index}. Enemies: {maxEnemies}");
                    yield break;
                }

                Enemy enemy = _enemiesList[index];
                enemy.SetEnemy(GetSettings(index));
                Vector2 availablePosition = _gameController.GetRandomAvailablePosition();
                enemy.transform.position = availablePosition;
                _gameController.RemoveAvailablePositions(availablePosition);
                yield return new WaitForEndOfFrame();
                enemy.Show();
            }
        }

        private EnemySettings GetSettings(int index)
        {
            Debug.Log($"index: {index}. Index %2 => {index % 2}.");
            return _enemySettings[index % 2 == 0 ? Random.Range(0, 2) : Random.Range(2, 4)];
        }

#if UNITY_EDITOR
        [ContextMenu("Fetch all enemies")]
        private void FetchEnemies()
        {
            _enemiesList = GetComponentsInChildren<Enemy>().ToList();
        }
#endif
    }
}