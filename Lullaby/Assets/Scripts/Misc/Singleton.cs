using System;
using UnityEngine;

namespace Lullaby
{
    public class Singleton <T>: MonoBehaviour where T : MonoBehaviour
    {
        protected static T Instance;

        public static T instance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = FindObjectOfType<T>();
                }

                return Instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
