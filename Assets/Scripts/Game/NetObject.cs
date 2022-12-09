using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    public abstract class NetObject<T1, T2> : NetObject
    {
        public abstract void Init(T1 t1, T2 t2);
    }

    public abstract class NetObject<T> : NetObject
    {
        public abstract void Init(T t);
    }

    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetObject : NetworkBehaviour
    {
        [SerializeField]
        protected NetworkObject netObject;

        protected virtual void OnValidate()
        {
            netObject = GetComponent<NetworkObject>();
        }

        public override void OnNetworkSpawn()
        {
        }

        public override void OnNetworkDespawn()
        {
            Destroy(gameObject);
        }

        public void Despawn()
        {
            netObject.Despawn();
        }
    }
}