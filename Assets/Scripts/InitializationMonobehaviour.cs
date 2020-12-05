using UnityEngine;

namespace SubScripts.Base
{
    public class InitializationMonobehaviour : MonoBehaviour
    {
        public bool IsAutoInitialized;

        private bool _isInitialized;
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public virtual void Initialization()
        {
            if(_isInitialized)return;

            _isInitialized = true;
        }

        public virtual void Start ()
        {
            if (IsAutoInitialized)
            {
                Initialization();
            }
        }

    }
}
