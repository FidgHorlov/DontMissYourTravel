using System;
using System.Collections.Generic;
using System.Linq;
using DontMissTravel.Data;
using DontMissTravel.Persons;
using UnityEngine;
using UnityEngine.UI;

namespace DontMissTravel.HideGame
{
    public class HideGameEnemy : MonoBehaviour
    {
        [SerializeField] private Image _enemyImage;
        [SerializeField] private RectTransform _rectTransform;
        [Space] 
        [SerializeField] List<EnemyHandler> _enemyHandlers;

        public RectTransform RectTransform => _rectTransform;

        public void SetEnemy(EnemyName enemyName)
        {
            EnemyHandler enemyHandler = _enemyHandlers.FirstOrDefault(handler => handler.EnemyName == enemyName);
            _enemyImage.sprite = enemyHandler?.EnemySprite;
        }
    }
}