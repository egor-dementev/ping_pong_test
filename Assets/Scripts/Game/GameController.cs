using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PingPong.Game
{
    public class GameController : NetworkBehaviour
    {
        [SerializeField]
        private Ball ballPrefab;
        [SerializeField]
        private Racket racketPrefab;
        [SerializeField]
        private Transform hostRacketSpawn;
        [SerializeField]
        private Transform opponentRacketSpawn;

        private System.Random rnd = new System.Random();
        private Dictionary<ulong, Racket> rackets = new Dictionary<ulong, Racket>();
        private Ball currentBall;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
         
            SpawnRackets();
            SpawnNewBall();
        }

        private void SpawnRackets()
        {
            if (!IsServer)
                return;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var racket = SpawnRacket(client.ClientId == NetworkManager.Singleton.LocalClientId);
                rackets[client.ClientId] = racket;
                racket.Init(client.ClientId, OnRacketSpawned);
            }
        }

        private void OnRacketSpawned(Racket racket)
        {
            racket.SetOwnerId(racket.ClientId);
            InitRacketClientRpc(racket.ObjectId, racket.ClientId);
        }

        [ClientRpc]
        private void InitRacketClientRpc(ulong racketId, ulong clientId)
        {
            Debug.LogWarning($"Init racket {racketId} for client {clientId}");
            
            if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(racketId, out var netObject))
                throw new Exception($"Can't find object with id {racketId}");

            var racket = netObject.gameObject.GetComponent<Racket>();
            
            if (racket == null)
                throw new Exception($"Can't get Racket component for object with id {racketId}");
            
            racket.SetClientId(clientId);
        }

        private Racket SpawnRacket(bool isHost)
        {
            var racket = Instantiate(racketPrefab);
            var spawn = isHost
                ? hostRacketSpawn
                : opponentRacketSpawn;
            racket.transform.position = spawn.transform.position;

            return racket;
        }

        private void SpawnNewBall(float startDirection = 1)
        {
            if (!IsServer)
                return;
            
            if (currentBall != null)
                Destroy(currentBall.gameObject);
            
            currentBall = Instantiate(ballPrefab, transform);

            var direction = new Vector2((float) rnd.NextDouble(), startDirection);
            currentBall.InitAndStartMove(direction);
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