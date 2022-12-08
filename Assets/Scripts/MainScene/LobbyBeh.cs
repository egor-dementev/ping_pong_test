using System;
using Unity.Netcode;

namespace PingPong.MainScene
{
    public class LobbyBeh : NetworkBehaviour
    {
        public static event Action OnOpponentConnected;
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer)
                return;

            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            OnOpponentConnected?.Invoke();
        }
    }
}