using System;
using System.Collections;
using DontMissTravel.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;

namespace DontMissTravel
{
    public class Instructions : MonoBehaviour
    {
        private readonly string[] GateNames = new[] {"L1", "L2", "L3", "R1", "R2", "R3"};
        private const float TimeForSwitchingSprites = 1.5f;
        public event Action OnInstructionsClosed;

        [SerializeField] private TextMeshProUGUI _gateName;

        [Space] [SerializeField] private Image _enemyImage;
        [SerializeField] private Sprite[] _enemySprites;

        [Space] [SerializeField] private Button _closeInstructions;
        [SerializeField] private Button _clearProgress;

        [Space] [SerializeField] private ConfirmationPopUp _confirmationPopUp;
        
        private GameObject _currentGameObject;
        private LevelGenerator _levelGenerator;

        private GameObject CurrentGameObject => _currentGameObject ??= gameObject;
        private LevelGenerator LevelGenerator => _levelGenerator ??= Singleton<LevelGenerator>.Instance;

        private void OnEnable()
        {
            StartCoroutine(nameof(AnimateInstructions));
            //StartCoroutine(nameof(GatesAnimation));
            // StartCoroutine(nameof(EnemiesAnimation));
            _closeInstructions.onClick.AddListener(OnInstructionClosed);
            _clearProgress.onClick.AddListener(OnClearProgress);
            //_confirmationPopUp.SetActive(false);
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(AnimateInstructions));
            // StopCoroutine(nameof(GatesAnimation));
            // StopCoroutine(nameof(EnemiesAnimation));
            _closeInstructions.onClick.RemoveListener(OnInstructionClosed);
            _clearProgress.onClick.RemoveListener(OnClearProgress);
            _confirmationPopUp.ConfirmedEvent -= ConfirmationEventHandler;
        }

        public void SetActive(bool toActivate)
        {
            CurrentGameObject.SetActive(toActivate);
        }

        private void OnInstructionClosed()
        {
            OnInstructionsClosed?.Invoke();
        }

        private void OnClearProgress()
        {
            _confirmationPopUp.SetActive(true);
            _confirmationPopUp.ConfirmedEvent += ConfirmationEventHandler;
        }

        private void ConfirmationEventHandler(bool isConfirmed)
        {
            _confirmationPopUp.ConfirmedEvent -= ConfirmationEventHandler;
            if (isConfirmed)
            {
                LevelGenerator.ClearProgress();
            }
        }

        private IEnumerator AnimateInstructions()
        {
            int enemySpriteIndex = 0;
            int gateIndex = 0;
            while (true)
            {
                yield return new WaitForSeconds(TimeForSwitchingSprites);
                ChangeEnemySprite(enemySpriteIndex);
                ChangeGateName(gateIndex);

                gateIndex++;
                if (gateIndex > GateNames.Length - 1)
                {
                    gateIndex = 0;
                }

                enemySpriteIndex++;
                if (enemySpriteIndex > _enemySprites.Length - 1)
                {
                    enemySpriteIndex = 0;
                }
            }
        }

        private IEnumerator EnemiesAnimation()
        {
            int enemySpriteIndex = 0;
            while (true)
            {
                yield return new WaitForSeconds(TimeForSwitchingSprites);
                _enemyImage.sprite = _enemySprites[enemySpriteIndex];
                enemySpriteIndex++;
                if (enemySpriteIndex > _enemySprites.Length - 1)
                {
                    enemySpriteIndex = 0;
                }
            }
        }

        private IEnumerator GatesAnimation()
        {
            int gateIndex = 0;
            while (true)
            {
                yield return new WaitForSeconds(TimeForSwitchingSprites);
                _gateName.text = GateNames[gateIndex];
                gateIndex++;
                if (gateIndex > GateNames.Length - 1)
                {
                    gateIndex = 0;
                }
            }
        }

        private void ChangeEnemySprite(int enemySpriteIndex)
        {
            _enemyImage.sprite = _enemySprites[enemySpriteIndex];
        }

        private void ChangeGateName(int gateIndex)
        {
            _gateName.text = GateNames[gateIndex];
        }
    }
}