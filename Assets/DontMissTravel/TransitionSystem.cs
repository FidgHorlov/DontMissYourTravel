using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace DontMissTravel
{
    public class TransitionSystem : Singleton<TransitionSystem>
    {
        [SerializeField] private bool _fadeIn;
        [Space] [SerializeField] private Transform _circleTransform;
        [SerializeField] private Transform _targetTransform;
        [Space] [SerializeField] private Image _circleImage;
        [SerializeField] private Image _targetImage;
        [SerializeField] private float _size;
        [SerializeField] private float _duration;

        private Vector3 _defaultSize;

        public void ZoomIn()
        {
            _circleTransform.DOKill(_circleTransform);
            _defaultSize = _circleImage.rectTransform.sizeDelta;
            
            Sequence dotween = DOTween.Sequence();
            dotween
                .Append(_circleImage.rectTransform.DOScale(_targetImage.rectTransform.sizeDelta, _duration))
                .Append(_circleImage.rectTransform.DOMove(_targetTransform.position, _duration));
            dotween.Play();
        }

        public void ZoomOut()
        {
            _circleTransform.DOKill(_circleTransform);
            
            Sequence dotween = DOTween.Sequence();
            dotween
                .Append(_circleImage.rectTransform.DOScale(_defaultSize, _duration))
                .Append(_circleImage.rectTransform.DOMove(Vector3.zero, _duration));
            dotween.Play();
        }

        private bool _isZoomed;
        
        private void Update()
        {
            if (_fadeIn)
            {
                ZoomIn();
                _fadeIn = false;
            }
            
        }
    }
}