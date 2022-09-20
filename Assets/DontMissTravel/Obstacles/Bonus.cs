using System;
using System.Collections;
using DG.Tweening;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DontMissTravel.Obstacles
{
    public class Bonus : MonoBehaviour
    {
        private enum BonusType
        {
            TimeDelay,
            SpeedBoost,
            Invisible
        }
        
        private const float FadeDuration = 0.5f;
        private const float BonusLifeTime = 5f;

        [SerializeField] private GameObject _visual;
        [SerializeField] private Image _bonusImage;
        [SerializeField] private Collider2D _collider;

        private Transform _currentTransform;
        private Player _player;

        public bool IsDelay { get; private set; }

        public void Init(Player player)
        {
            _player = player;
        }

        private void Awake()
        {
            _currentTransform = transform;
            SetActive(false);
        }

        public void SetActiveInTime(bool toShow, Vector2 position = default)
        {
            if (toShow)
            {
                _currentTransform.position = position;
                StartCoroutine(nameof(HideAfterTime), BonusLifeTime);
                SetActive(true);
            }

            _bonusImage
                .DOFade(toShow ? 1f : 0f, FadeDuration)
                .OnComplete(() =>
                {
                    if (!toShow)
                    {
                        SetActive(false);
                    }
                })
                .SetId(_bonusImage);
        }

        private IEnumerator HideAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);
            _bonusImage.DOKill();
            _bonusImage
                .DOFade(0f, FadeDuration)
                .OnComplete(() => SetActive(false))
                .SetId(_bonusImage);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.Tags.Player))
            {
                return;
            }

            SetBonus(GetBonusType());
            StopCoroutine(nameof(HideAfterTime));
            SetActive(false);
        }

        private void SetBonus(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TimeDelay:
                    GameController.Instance.Delay();
                    IsDelay = true;
                    break;
                case BonusType.SpeedBoost:
                    _player.UpgradeSpeed();
                    break;
                case BonusType.Invisible:
                    _player.MakeInvisible();
                    break;
            }
        }

        private BonusType GetBonusType()
        {
            BonusType bonusType;
            int randValue = Random.Range(0, 30);
            if (randValue < 10)
            {
                bonusType = BonusType.Invisible;
            }
            else if (randValue is > 10 and < 20)
            {
                bonusType = BonusType.SpeedBoost;
            }
            else
            {
                bonusType = BonusType.TimeDelay;
            }

            return bonusType;
        }

        private void SetActive(bool toShow)
        {
            _visual.SetActive(toShow);
            _collider.enabled = toShow;
        }

        [ContextMenu("Delay")]
        private void Delay()
        {
            SetBonus(BonusType.TimeDelay);
        }
        
        [ContextMenu("Invisible")]
        private void Invisible()
        {
            SetBonus(BonusType.Invisible);
        }
        
        [ContextMenu("Speed")]
        private void Speed()
        {
            SetBonus(BonusType.SpeedBoost);
        }
    }
}