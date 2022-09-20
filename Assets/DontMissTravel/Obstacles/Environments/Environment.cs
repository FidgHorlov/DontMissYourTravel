using UnityEngine;
using UnityEngine.UI;
using EnvironmentType = DontMissTravel.Data.EnvironmentType;

namespace DontMissTravel.Obstacles.Environments
{
    public class Environment : MonoBehaviour
    {
        [Header("Works only in Editor")]
        [SerializeField] private Sprite _borderSprite;
        [SerializeField] private Sprite _chairSprite;
        [SerializeField] private Sprite _chairFrontSideSprite;
        [SerializeField] private Sprite _gateSprite;
        [SerializeField] private EnvironmentType _currentType;

        private Transform _currentTransform;
        private Image _currentImage;

        public void ChangeEnvironment()
        {
            _currentImage = GetComponent<Image>();
            _currentTransform = GetComponent<Transform>();

            switch (_currentType)
            {
                case EnvironmentType.Border:
                {
                    _currentImage.sprite = _borderSprite;
                    break;
                }
                case EnvironmentType.ChairLeftSide:
                {
                    _currentImage.sprite = _chairSprite;
                    var scale = _currentTransform.localScale;
                    scale.x = 1f;
                    _currentTransform.localScale = scale;
                    break;
                }
                case EnvironmentType.ChairRightSide:
                {
                    _currentImage.sprite = _chairSprite;
                    var scale = _currentTransform.localScale;
                    scale.x = -1f;
                    _currentTransform.localScale = scale;
                    break;
                }
                case EnvironmentType.ChairFrontSide:
                {
                    _currentImage.sprite = _chairFrontSideSprite;
                    break;
                }
                case EnvironmentType.Gate:
                {
                    _currentImage.sprite = _gateSprite;
                    break;
                }
            }
        } 
    }
}