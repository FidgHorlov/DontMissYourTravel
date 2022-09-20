using System;
using System.Collections.Generic;
using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Ui
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _attentionGameObject;
        [Space] [SerializeField] private InfoSection _gateWillOpen;
        [SerializeField] private InfoSection _gateOpen;
        [SerializeField] private InfoSection _gateClosed;
        [Space] 
        [SerializeField] private InfoSection _departureIn;
        [Space] 
        [SerializeField] private HideGameObject _remarkGroup;
        [SerializeField] private InfoSection _remarkInfo;
        [SerializeField] private InfoSection _currentLevel;

        private List<InfoSection> _nonStaticInfoSections;
        
        private void Awake()
        {
            _nonStaticInfoSections = new List<InfoSection>
            {
                _gateWillOpen,
                _gateOpen,
                _gateClosed,
            };

            _attentionGameObject.SetActive(false);
            _remarkInfo.SetActive(false);
            _remarkGroup.SetActiveImmediate(false);
        }

        public void SetGateOpenTime(float openIn)
        {
            _gateWillOpen.SetDynamicText(openIn);
        }

        public void SetDepartureTime(float departureIn)
        {
            _departureIn.SetDynamicText(departureIn);
        }

        public void SetBonus(string staticText, string dynamicText = null)
        {
            _remarkGroup.SetActive(true);
            _remarkInfo.SetActive(true);
            _remarkInfo.Init(staticText, dynamicText);
            _remarkGroup.HideInTime();
        }

        public void ChangeGateState(GateState gateState, string text)
        {
            switch (gateState)
            {
                case GateState.WillOpen:
                    HideAllExcept(_gateWillOpen);
                    _gateWillOpen.Init(dynamicText: text);
                    break;
                case GateState.Opened:
                    HideAllExcept(_gateOpen);
                    _gateOpen.Init(dynamicText: text);
                    _departureIn.SetActive(true);
                    break;
                case GateState.Closed:
                    HideAllExcept(_gateClosed);
                    _departureIn.SetActive(false);
                    break;
            }

            _attentionGameObject.SetActive(true);
        }

        public void SetCurrentLevel(int currentLevel)
        {
            _currentLevel.SetDynamicText(currentLevel);
        }

        private void HideAllExcept(InfoSection infoSection)
        {
            
            foreach (InfoSection section in _nonStaticInfoSections)
            {
                if (section != infoSection)
                {
                    section.SetActive(false);
                    continue;
                }
                
                section.SetActive(true);
            }
        }
    }
}