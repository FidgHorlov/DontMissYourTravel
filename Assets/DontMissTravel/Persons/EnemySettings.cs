using System;
using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Persons
{
    [Serializable]
    public class EnemySettings
    {
        public EnemyType Type;
        public EnemyName Name;
        public Sprite IdleSprite;
        public Sprite[] MovementSprites;
    }
}