using UnityEngine;

namespace CocoDoogy.Core
{
    public class Singleton<T>: MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; } = null;
        
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
