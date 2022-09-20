using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Ui
{
    public class HideGameObject : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public void SetActive(bool toActivate)
        {
            if (_canvasGroup == null)
            {
                Debug.LogError($"Fill the Canvas Group!");
                return;
            }

            _canvasGroup.DOKill(_canvasGroup);
            _canvasGroup
                .DOFade(toActivate ? 1f : 0f, Constants.FadeAnimationTime)
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = toActivate;
                })
                .SetId(_canvasGroup);
        }

        public void HideInTime()
        {
            StartCoroutine(nameof(HideInTimeCoroutine));
        }

        private IEnumerator HideInTimeCoroutine()
        {
            yield return new WaitForSeconds(Constants.ObjectAliveTime);
            SetActive(false);
        }

        public void SetActiveImmediate(bool toActivate)
        {
            _canvasGroup.interactable = toActivate;
            _canvasGroup.alpha = toActivate ? 1f : 0f;
        }
    }
}