using System;
using System.Collections;
using DG.Tweening;
using DontMissTravel.Audio;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;
using UnityEngine.UI;
using AudioType = DontMissTravel.Audio.AudioType;
using Random = UnityEngine.Random;

namespace DontMissTravel.Obstacles
{
    public class Bonus : MonoBehaviour
    {
        private const float FadeDuration = 0.5f;
        private const float DefaultBonusLifeTime = 5f;

        [SerializeField] private GameObject _visual;
        [SerializeField] private Collider2D _collider;

        [Space] [SerializeField] private Image _bonusImage;
        [SerializeField] private Sprite _timeDelay;
        [SerializeField] private Sprite _speedBoost;
        [SerializeField] private Sprite _invisible;

        private GameController _gameController;
        private AudioManager _audioManager;
        private Transform _currentTransform;
        private BonusType _currentBonusType;

        private Player _player;
        private float _lifeTime;

        public bool IsDelay { get; private set; }

        public void Init(Player player)
        {
            _player = player;
            _currentBonusType = GetBonusType();
            _lifeTime = DefaultBonusLifeTime;
            SetBonusVisual();
            _gameController = Singleton<GameController>.Instance;
            _audioManager = Singleton<AudioManager>.Instance;
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
                StartCoroutine(nameof(HideAfterTime), _lifeTime);
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
        
        protected void ChangeBonusLifeTime(float newTime)
        {
            _lifeTime = newTime;
            StopCoroutine(nameof(HideAfterTime));
        }

        private void SetBonusVisual()
        {
            switch (_currentBonusType)
            {
                case BonusType.TimeDelay:
                    _bonusImage.sprite = _timeDelay;
                    break;
                case BonusType.SpeedBoost:
                    _bonusImage.sprite = _speedBoost;
                    break;
                case BonusType.Invisible:
                    _bonusImage.sprite = _invisible;
                    break;
            }
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

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Constants.Tags.Player))
            {
                return;
            }

            SetBonusPower(_currentBonusType);
            StopCoroutine(nameof(HideAfterTime));
            SetActive(false);
        }

        private void SetBonusPower(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TimeDelay:
                    _gameController.Delay();
                    IsDelay = true;
                    break;
                case BonusType.SpeedBoost:
                    _player.UpgradeSpeed();
                    break;
                case BonusType.Invisible:
                    _player.MakeInvisible();
                    break;
            }

            _audioManager.PlaySfx(AudioType.Attention);
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
            SetBonusPower(BonusType.TimeDelay);
        }

        [ContextMenu("Invisible")]
        private void Invisible()
        {
            SetBonusPower(BonusType.Invisible);
        }

        [ContextMenu("Speed")]
        private void Speed()
        {
            SetBonusPower(BonusType.SpeedBoost);
        }
    }
}