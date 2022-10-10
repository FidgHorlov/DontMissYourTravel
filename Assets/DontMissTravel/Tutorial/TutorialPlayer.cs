using System;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;

namespace DontMissTravel.Tutorial
{
    public class TutorialPlayer : Player
    {
        public event Action<EnemyName> OnEnemyMeet;
        private bool _isAlreadyMeet;

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            string otherTag = other.gameObject.tag;

            if (otherTag.Equals(Constants.Tags.Enemy))
            {
                if (_isAlreadyMeet)
                {
                    return;
                }
                    
                TutorialEnemy tutorialEnemy = other.gameObject.GetComponent<TutorialEnemy>();
                OnEnemyMeet?.Invoke(tutorialEnemy.EnemyName);
                _isAlreadyMeet = true;
                tutorialEnemy.SetActive(false);
                return;
            }

            base.OnCollisionEnter2D(other);
        }
    }
}