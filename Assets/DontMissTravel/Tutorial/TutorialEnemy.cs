using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Tutorial
{
    public class TutorialEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyName _enemyName;
        public EnemyName EnemyName => _enemyName;

        public void SetActive(bool toShow)
        {
            gameObject.SetActive(toShow);
        }
    }
}
