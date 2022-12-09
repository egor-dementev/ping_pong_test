using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : NetObject<Vector2>
    {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField, Range(1, 10)]
        private float startVelocity = 2;
        [SerializeField, Range(.1f, 10)]
        private float acceleration = .1f;

        private bool isInitialized;
        private Vector2 startDirection;
        private float curVelocity;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            rb = GetComponent<Rigidbody2D>();
        }

        public override void Init(Vector2 direction)
        {
            startDirection = direction;
            transform.localPosition = Vector3.zero;
            netObject.Spawn();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
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
    }
}