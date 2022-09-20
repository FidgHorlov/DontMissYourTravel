using System;
using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Persons
{
    [Serializable]
    public class EnemyHandler
    {
        [SerializeField] private EnemyName _enemyName;
        [SerializeField] private Sprite _enemySprite;
        
        public Sprite EnemySprite => _enemySprite;
        public EnemyName EnemyName => _enemyName;
    }
}
