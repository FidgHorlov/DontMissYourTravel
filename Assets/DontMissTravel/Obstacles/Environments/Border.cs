using DontMissTravel.Data;
using UnityEngine;

namespace DontMissTravel.Obstacles.Environments 
{
    public class Border : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider2D;

        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Constants.Tags.Obstacle))
            {
                _collider2D.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            string otherTag = other.gameObject.tag;
            if (otherTag.Equals(Constants.Tags.Enemy) || otherTag.Equals(Constants.Tags.Player))
            {
                _collider2D.isTrigger = false;
            }
        }
    }
}