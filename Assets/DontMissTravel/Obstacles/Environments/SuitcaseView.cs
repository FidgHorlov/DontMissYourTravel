using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DontMissTravel.Obstacles.Environments
{
    public class SuitcaseView : MonoBehaviour
    {
        [SerializeField] private Image _suitcaseImage;
        [SerializeField] private List<Sprite> _suitcaseList;

        private void Awake()
        {
            SetRandomSuitcase();
        }

        private void SetRandomSuitcase()
        {
            int suitcaseIndex = Random.Range(0, _suitcaseList.Count);
            _suitcaseImage.sprite = _suitcaseList[suitcaseIndex];
        }
    }
}
