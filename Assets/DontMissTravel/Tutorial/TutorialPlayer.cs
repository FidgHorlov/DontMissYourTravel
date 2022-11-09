using System;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;

namespace DontMissTravel.Tutorial
{
    public class TutorialPlayer : Player
    {
        private List<TutorialEnemy> _possibleEnemies;
        public event Action<EnemyName> OnEnemyMeet;

        public void SetPossibleEnemies(List<TutorialEnemy> enemies)
        {
            _possibleEnemies = enemies;
        }
        
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            string otherTag = other.gameObject.tag;

            if (otherTag.Equals(Constants.Tags.Enemy))
            {
                if (IsAlreadyMet)
                {
                    return;
                }

                TutorialEnemy tutorialEnemy = GetEnemy(other.gameObject);
                OnEnemyMeet?.Invoke(tutorialEnemy.EnemyName);
                IsAlreadyMet = true;
                tutorialEnemy.SetActive(false);
                return;
            }

            base.OnCollisionEnter2D(other);
        }

        private TutorialEnemy GetEnemy(GameObject targetGameObject)
        {
            TutorialEnemy enemy = _possibleEnemies.FirstOrDefault(tutorialEnemy => tutorialEnemy.gameObject.Equals(targetGameObject));
            if (ReferenceEquals(enemy, null))
            {
                Debug.LogError($"Something strange");
                return null;
            }

            return enemy;
        }
    }
}