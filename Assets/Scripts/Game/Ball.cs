using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkObject))]
    public class Ball : NetworkBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private NetworkObject netObject;
        [SerializeField, Range(1, 10)]
        private float startVelocity = 2;
        [SerializeField, Range(.1f, 10)]
        private float acceleration = .1f;

        private bool isInitialized;
        private Vector2 startDirection;
        private float curVelocity;
        
        private void OnValidate()
        {
            rb = GetComponent<Rigidbody2D>();
            netObject = GetComponent<NetworkObject>();
        }

        public void InitAndStartMove(Vector2 direction)
        {
            startDirection = direction;
            transform.localPosition = Vector3.zero;
            netObject.Spawn();
        }

        public override void OnNetworkSpawn()
        {
            curVelocity = startVelocity;
            rb.velocity = GetVelocity(startDirection);
            isInitialized = true;
        }

        private Vector2 GetVelocity(Vector2 direction)
        {
            return direction.normalized * curVelocity;
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;
            
            if (!isInitialized)
                return;

            rb.velocity = GetVelocity(rb.velocity);
        }

        private void Update()
        {
            curVelocity += acceleration * Time.deltaTime;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            netObject.Despawn();
        }
    }
}