using System;
using UnityEngine;

namespace Demos.MetaVerse
{
    public class Player : MonoBehaviour
    {
        public string Identifier;
        public Vector3 Position;
        public GameObject Avatar;
        public string Username;

        private float NextTimeout = -1;
        
        private void Awake()
        {
            if (!Avatar) return;
            
            SyncPosition();
        }

        void Update()
        {
            if (Time.time <= NextTimeout) return;

            if (!Avatar) return;

            SyncPosition();
            
            NextTimeout = Time.time + 0.5f;
        }

        void SyncPosition()
        {
            if (!Avatar) return;
            
            Position = Avatar.transform.position;
        }
    }
}