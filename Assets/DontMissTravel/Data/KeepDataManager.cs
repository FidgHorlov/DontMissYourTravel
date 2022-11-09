using UnityEngine;

namespace DontMissTravel.Data
{
    [DefaultExecutionOrder(Constants.ScriptOrder.KeepDataManager)]
    public class KeepDataManager : MonoBehaviour
    {
        public static KeepDataManager Instance { get; private set; }
        public bool WasGameRun { get; set; }

        private void Awake()
        {
            KeepDataManager keepData  = FindObjectOfType<KeepDataManager>();
            if (keepData != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}