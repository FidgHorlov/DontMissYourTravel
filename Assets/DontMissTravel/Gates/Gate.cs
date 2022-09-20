using System;
using DontMissTravel.Persons;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Gates
{
    public class Gate : MonoBehaviour
    {
        [SerializeField] private Image _gateImage;
        [SerializeField] private BoxCollider2D _gateCollider;

        public Action OnPlayerReached;
        
        public void SetGateStatus(Sprite gateSprite, bool isOpen)
        {
            _gateImage.sprite = gateSprite;
            _gateCollider.isTrigger = isOpen;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.GetComponent<Player>())
            {
                return;
            }
            
            OnPlayerReached?.Invoke();
            _gateCollider.isTrigger = false;
        }
    }
}
