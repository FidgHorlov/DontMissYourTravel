using System;
using System.Collections.Generic;
using UnityEngine;

namespace DontMissTravel.Gates
{
    public class GateController : MonoBehaviour
    {
        public Action OnPlayerReachedGate;
        
        [SerializeField] private List<Gate> _gates;
        [SerializeField] private Sprite _openGateSprite;
        [SerializeField] private Sprite _closeGateSprite;

        private Gate _openedGate;

        public int GateCount => _gates.Count; 
        
        private void Awake()
        {
            foreach (Gate gate in _gates)
            {
                gate.SetGateStatus(gateSprite: _closeGateSprite, isOpen: false);
                gate.OnPlayerReached += OnPlayerReached;
            }
        }

        private void OnDestroy()
        {
            foreach (Gate gate in _gates)
            {
                gate.OnPlayerReached -= OnPlayerReached;
            }
        }

        public string OpenGate(int gateNum)
        {
            _openedGate = _gates[gateNum];
            _openedGate.SetGateStatus(gateSprite: _openGateSprite, isOpen: true);
            return _openedGate.name;
        }

        public void CloseGate()
        {
            _openedGate?.SetGateStatus(gateSprite: _closeGateSprite, isOpen: false);
        }

        private void OnPlayerReached()
        {
            OnPlayerReachedGate?.Invoke();
        }
    }
}
