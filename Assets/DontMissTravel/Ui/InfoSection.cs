using System.Linq;
using DG.Tweening;
using DontMissTravel.Data;
using TMPro;
using UnityEngine;

namespace DontMissTravel.Ui
{
    public class InfoSection : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _staticText;
        [SerializeField] private TextMeshProUGUI _dynamicText;

        public void Init(string staticText = null, string dynamicText = null)
        {
            if (!string.IsNullOrEmpty(staticText))
            {
                _staticText.text = staticText;
            }

            if (!string.IsNullOrEmpty(dynamicText))
            {
                _dynamicText.gameObject.SetActive(true);
                _dynamicText.text = dynamicText;
            }
            else
            {
                _dynamicText.gameObject.SetActive(false);
            }
        }

        public void SetActive(bool isActivate)
        {
            gameObject.SetActive(isActivate);
        }
        
        public void SetDynamicText(string text)
        {
            _dynamicText.text = text;
        }

        public void SetDynamicText(float floatValue)
        {
            _dynamicText.text = floatValue.ToString(Constants.FloatTextFormat);
        }

        public void SetDynamicText(int intValue)
        {
            _dynamicText.text = intValue.ToString();
        }
    }
}