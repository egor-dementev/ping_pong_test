using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    public class GameController : NetworkBehaviour
    {
        [SerializeField]
        private Ball ballPrefab;

        private Ball currentBall;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            currentBall = Instantiate(ballPrefab, transform);
            currentBall.InitAndStartMove();
        }
        
        private void Update()
        {
            if (IsServer)
            {
                // update server logic here
            }
        }
    }
}