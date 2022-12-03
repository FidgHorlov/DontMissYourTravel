using UnityEngine;

namespace DontMissTravel
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T) this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}