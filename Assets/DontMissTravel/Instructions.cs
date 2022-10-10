using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private TextMeshProUGUI _gateName;

        [Space] [SerializeField] private Image _enemyImage;
        [SerializeField] private Sprite[] _enemySprites; 
        
        [Space] 
        [SerializeField] private Button _closeInstructions;

        public event Action OnInstructionsClosed;

        private void OnEnable()
        {
            StartCoroutine(nameof(AnimateInstructions));
            //StartCoroutine(nameof(GatesAnimation));
            // StartCoroutine(nameof(EnemiesAnimation));
            _closeInstructions.onClick.AddListener(OnInstructionClosed);
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(AnimateInstructions));
            // StopCoroutine(nameof(GatesAnimation));
            // StopCoroutine(nameof(EnemiesAnimation));
            _closeInstructions.onClick.RemoveListener(OnInstructionClosed);
        }
        
        public void SetActive(bool toActivate)
        {
            gameObject.SetActive(toActivate);
        }

        private void OnInstructionClosed()
        {
            OnInstructionsClosed?.Invoke();
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