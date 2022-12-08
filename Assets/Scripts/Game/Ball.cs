using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkObject))]
    public class Ball : NetworkBehaviour
    {
        private const float StartVelocity = 2;
        
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private NetworkObject netObject;

        private bool isInitialized;
        
        private void OnValidate()
        {
            rb = GetComponent<Rigidbody2D>();
            netObject = GetComponent<NetworkObject>();
        }

        public void InitAndStartMove()
        {
            transform.localPosition = Vector3.zero;
            netObject.Spawn();
        }

        public override void OnNetworkSpawn()
        {
            rb.velocity = Vector2.one * StartVelocity;
            isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;
            
            if (!isInitialized)
                return;
        }
    }
}