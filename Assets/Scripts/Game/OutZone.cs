using System;
using UnityEngine;

namespace PingPong.Game
{
    public class OutZone : MonoBehaviour
    {
        private ulong ownerId;
        private Action<ulong> onBallEnter;

        public void Init(ulong ownerId, Action<ulong> onBallEnter)
        {
            this.ownerId = ownerId;
            this.onBallEnter = onBallEnter;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var ball = other.gameObject.GetComponent<Ball>();
            
            if (ball == null)
                return;

            onBallEnter?.Invoke(ownerId);
        }
    }
}