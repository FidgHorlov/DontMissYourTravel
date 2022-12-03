using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Data;
using UnityEngine;
using EnemyType = DontMissTravel.Data.EnemyType;

namespace DontMissTravel.Persons
{
    public class EnemyCreator : MonoBehaviour
    {
        [SerializeField] private List<Enemy> _enemiesList;
        [SerializeField] private Transform _enemyFolder;

        private GameController _gameController;
        
        private int _policeCount;
        private int _nurseCount;

        private IEnumerator Start()
        {
            _gameController = Singleton<GameController>.Instance;

            int maxEnemies = Singleton<LevelGenerator>.Instance.CurrentLevel;
            if (maxEnemies > Constants.EnemiesParameters.MaximumEnemies)
            {
                maxEnemies = Constants.EnemiesParameters.MaximumEnemies;
            }

            _policeCount = maxEnemies / 2;
            _nurseCount = maxEnemies - _policeCount;

            yield return new WaitForSeconds(1f);

            CreateEnemies(_policeCount, EnemyType.Policeman);
            CreateEnemies(_nurseCount, EnemyType.Nurse);
        }

        private void CreateEnemies(int count, EnemyType enemyType)
        {
            List<Enemy> enemies = new List<Enemy>();
            foreach (Enemy enemy in _enemiesList)
            {
                if (enemy.EnemyType == enemyType)
                {
                    enemies.Add(enemy);   
                }
            }

            int lastIndexInSelected = enemies.Count - 1;
            for (int index = 0; index < count; index++)
            {
                CreateEnemy(index % 2 == 0 ? enemies[0] : enemies[lastIndexInSelected]);
            }
        }
        
        private void CreateEnemy(Enemy enemy)
        {
            Enemy newEnemy = Instantiate(enemy, _enemyFolder, true);
            Vector2 availablePosition = _gameController.GetRandomAvailablePosition();
            newEnemy.transform.position = availablePosition;
            _gameController.RemoveAvailablePositions(availablePosition);
        }


        private void GetRandomEnemy()
        {
            
        }
    }
}