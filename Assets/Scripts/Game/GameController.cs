using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace PingPong.Game
{
    public class GameController : NetworkBehaviour
    {
        private const int MaxScore = 10;
        
        [Header("Prefabs")]
        [SerializeField]
        private Ball ballPrefab;
        [SerializeField]
        private Racket racketPrefab;
        [Header("Scene objects")]
        [SerializeField]
        private Transform hostRacketSpawn;
        [SerializeField]
        private Transform opponentRacketSpawn;
        [SerializeField]
        private OutZone hostOut;
        [SerializeField]
        private OutZone opponentOut;
        [Header("UI")]
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private GameOverScreen gameOverScreen;

        private Random rnd = new Random();
        private Dictionary<ulong, Racket> rackets = new Dictionary<ulong, Racket>();
        private Dictionary<ulong, int> scores;
        private Ball currentBall;
        private bool isGameOver;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
         
            InitPlayers();
            SpawnNewBall();
        }

        private void InitPlayers()
        {
            if (!IsServer)
                return;
 
            scores = new Dictionary<ulong, int>();

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                // rackets
                SpawnRacketForPlayer(client.ClientId);
               
                // out zones
                var outZone = client.ClientId == NetworkManager.Singleton.LocalClientId
                    ? hostOut
                    : opponentOut;
                InitOutZone(outZone, client.ClientId);
                
                // scores
                scores[client.ClientId] = 0;
            }
            
            UpdateScoresText();
        }

        private void SpawnRacketForPlayer(ulong ownerId)
        {
            var racket = SpawnRacket(ownerId == NetworkManager.Singleton.LocalClientId);
            rackets[ownerId] = racket;
            racket.Init(ownerId, OnRacketSpawned);
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

        private void OnRacketSpawned(Racket racket)
        {
            racket.SetOwnerId(racket.ClientId);
            InitRacketClientRpc(racket.ObjectId, racket.ClientId);
        }

        [ClientRpc]
        private void InitRacketClientRpc(ulong racketId, ulong clientId)
        {
            if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(racketId, out var netObject))
                throw new Exception($"Can't find object with id {racketId}");

            var racket = netObject.gameObject.GetComponent<Racket>();
            
            if (racket == null)
                throw new Exception($"Can't get Racket component for object with id {racketId}");
            
            racket.SetClientId(clientId);
        }

        private void InitOutZone(OutZone outZone, ulong ownerId)
        {
            outZone.Init(ownerId, OnBallGoToOut);
        }

        private void OnBallGoToOut(ulong outOwner)
        {
            if (!IsServer)
                return;
            
            var curScore = scores[outOwner];

            scores[outOwner] = ++curScore;
            
            UpdateScoresText();
            
            if (curScore < MaxScore)
            {
                SpawnNewBall((float) rnd.NextDouble() - .5f);
                return;
            }

            currentBall?.Despawn();

            foreach (var racket in rackets)
                racket.Value.Despawn();

            ShowGameOverScreenClientRpc(outOwner);
        }

        private void UpdateScoresText()
        {
           if (!IsServer)
               return;

           var opponentScore = scores.First(kv => kv.Key != NetworkManager.Singleton.LocalClientId).Value;
           var hostScore = scores[NetworkManager.Singleton.LocalClientId];
           var text = $"Score:\n{hostScore}\nvs\n{opponentScore}";

           UpdateScoresTextClientRpc(text);
        }

        [ClientRpc]
        private void UpdateScoresTextClientRpc(string text)
        {
            scoreText.text = text;
        }

        [ClientRpc]
        private void ShowGameOverScreenClientRpc(ulong loser)
        {
            gameOverScreen.Show(loser != NetworkManager.Singleton.LocalClientId);
        }
        
        private void SpawnNewBall(float startDirection = 1)
        {
            if (!IsServer)
                return;
            
            currentBall?.Despawn();
            
            currentBall = Instantiate(ballPrefab, transform);

            var direction = new Vector2((float) rnd.NextDouble() - .5f, startDirection);
            currentBall.Init(direction);
        }
    }
}