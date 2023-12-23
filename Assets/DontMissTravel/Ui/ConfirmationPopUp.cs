using System;
using UnityEngine;
using UnityEngine.UI;

namespace DontMissTravel.Ui
{
    public class ConfirmationPopUp : MonoBehaviour
    {
        public event Action<bool> ConfirmedEvent;
        
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _cancelButton;
        
        private GameObject _currentGameObject;
        private GameObject CurrentGameObject => _currentGameObject ??= gameObject;

        private void Awake()
        {
            SetActive(false);
        }

        private void OnEnable()
        {
            _okButton.onClick.AddListener(OkPressed);
            _cancelButton.onClick.AddListener(CancelPressed);
        }
        
        private void OnDisable()
        {
            _okButton.onClick.RemoveListener(OkPressed);
            _cancelButton.onClick.RemoveListener(CancelPressed);
            SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            CurrentGameObject.SetActive(isActive);
        }

        private void OkPressed()
        {
            ConfirmedEvent?.Invoke(true);
            SetActive(false);
        }

        private void CancelPressed()
        {
            ConfirmedEvent?.Invoke(false);
            SetActive(false);
        }
    }
}