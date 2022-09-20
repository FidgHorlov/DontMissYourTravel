using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Gates
{
    public class GateController : MonoBehaviour
    {
        [SerializeField] private List<Gate> _gates;
        [SerializeField] private Sprite _openGateSprite;
        [SerializeField] private Sprite _closeGateSprite;

        public Action OnPlayerReachedGate;
        
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
            _gates[gateNum].SetGateStatus(gateSprite: _openGateSprite, isOpen: true);
            return _gates[gateNum].name;
        }

        private void OnPlayerReached()
        {
            OnPlayerReachedGate?.Invoke();
        }
    }
}
