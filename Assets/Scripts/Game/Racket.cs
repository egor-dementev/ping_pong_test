using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NetworkObject))]
    public class Racket : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject netObject;
        [SerializeField, Range(1, 10)]
        private float minMaxPos = 1;
        [SerializeField, Range(1, 10)]
        private float maxSpeed = 3;
        [SerializeField, Range(.1f, 10)]
        private float acceleration = 1.2f;

        [SerializeField]
        private bool isInitialized;
        [SerializeField]
        private float destinationX;
        [SerializeField]
        private float speed;
        [SerializeField]
        private Vector3 myPos;

        public ulong ObjectId => netObject.NetworkObjectId;
        public ulong ClientId;
        // public ulong ClientId { get; private set; }
        
        private Action<Racket> onSpawned;

        [ContextMenu("IsOwner?")]
        private void CheckIsOwner()
        {
            Debug.LogWarning($"{ClientId} is owner [{IsOwner}]-[{netObject.IsOwner}]"
                             + $"\n{netObject.OwnerClientId} is owner");
        }
        
        private void OnValidate()
        {
            netObject = GetComponent<NetworkObject>();
        }
        
        public void Init(ulong clientId, Action<Racket> onSpawned)
        {
            this.ClientId = clientId;
            this.onSpawned = onSpawned;
            netObject.Spawn();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                onSpawned?.Invoke(this);
            
            onSpawned = null;
        }

        public void SetOwnerId(ulong clientId)
        {
            netObject.ChangeOwnership(clientId);
        }
        
        public void SetClientId(ulong clientId)
        {
            name = $"Racket for Player_{clientId}";
            
            ClientId = clientId;

            if (ClientId != NetworkManager.Singleton.LocalClientId)
            {
                // this is opponent racket
                enabled = false;
                return;
            }

            isInitialized = true;
        }

        private void Start()
        {
            StartCoroutine(MoveRoutine());
        }

        private void Update()
        {
            if (!isInitialized)
                return;

            destinationX = Input.mousePosition.x - Screen.width / 2;
        }

        private IEnumerator MoveRoutine()
        {
            while (!isInitialized)
                yield return null;
            
            // Vector3 myPos;
            // var speed = 0f;

            while (true)
            {
                myPos = transform.position;
                var diff = destinationX - myPos.x;

                if (diff > -.1f && diff < .1f)
                {
                    speed = 0;
                    yield return null;
                    continue;
                }

                if (destinationX < myPos.x)
                {
                    speed -= acceleration * Time.deltaTime;
                }
                else
                {
                    speed += acceleration * Time.deltaTime;
                }
                
                speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

                if (destinationX < myPos.x)
                {
                    myPos.x = Mathf.Clamp(myPos.x + speed * Time.deltaTime, destinationX, float.MaxValue);
                }
                else
                {
                    myPos.x = Mathf.Clamp(myPos.x + speed * Time.deltaTime, float.MinValue, destinationX);
                }

                if (Mathf.Abs(myPos.x) > minMaxPos)
                    speed = 0;

                myPos.x = Mathf.Clamp(myPos.x, -minMaxPos, minMaxPos);
                transform.position = myPos;

                yield return null;
            }
        }
    }
}