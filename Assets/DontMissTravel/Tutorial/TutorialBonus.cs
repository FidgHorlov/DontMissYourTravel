using System;
using DontMissTravel.Data;
using DontMissTravel.Obstacles;
using UnityEngine;

namespace DontMissTravel.Tutorial
{
    public class TutorialBonus : Bonus
    {
        private const float TutorialBonusLifeTime = 45f;
        public event Action OnBonusApplied;

        public void ChangeBonusLifeTime()
        {
            ChangeBonusLifeTime(TutorialBonusLifeTime);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!other.CompareTag(Constants.Tags.Player))
            {
                return;
            }

            OnBonusApplied?.Invoke();
        }
    }
}