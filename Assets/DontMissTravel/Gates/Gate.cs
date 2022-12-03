using System;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;
using UnityEngine.UI;

namespace DontMissTravel.Gates
{
    public class Gate : MonoBehaviour
    {
        [SerializeField] private Image _gateImage;
        [SerializeField] private BoxCollider2D _gateCollider;
        public event Action OnPlayerReached;
        
        public void SetGateStatus(Sprite gateSprite, bool isOpen)
        {
            _gateImage.sprite = gateSprite;
            _gateCollider.isTrigger = isOpen;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            string otherTag = other.tag;

            if (!otherTag.Equals(Constants.Tags.Player))
            {
                return;
            }
            
            OnPlayerReached?.Invoke();
            _gateCollider.isTrigger = false;
        }
    }
}
