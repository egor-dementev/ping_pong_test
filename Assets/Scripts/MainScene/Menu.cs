using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PingPong.MainScene
{
    public class Menu : MonoBehaviour
    {
        [SerializeField]
        private Button startAsHostButton;
        [SerializeField]
        private TMP_InputField ipInput;
        [SerializeField]
        private Button joinByIpButton;
        [SerializeField]
        private WaitOpponentPanel waitOpponentPanel;
        [SerializeField]
        private StartGameTimerPanel startGameTimerPanel;

        private void Awake()
        {
            waitOpponentPanel.gameObject.SetActive(false);
            startGameTimerPanel.gameObject.SetActive(false);
            
            ipInput.text = "127.0.0.1";
            
            startAsHostButton.onClick.RemoveAllListeners();
            startAsHostButton.onClick.AddListener(StartAsHost);
            
            joinByIpButton.onClick.RemoveAllListeners();
            joinByIpButton.onClick.AddListener(JoinByIp);
        }

        private void StartAsHost()
        {
            LobbyBeh.OnOpponentConnected += OnOpponentConnected;
            
            NetworkManager.Singleton.StartHost();

            waitOpponentPanel.gameObject.SetActive(true);
        }

        private void OnOpponentConnected()
        {
            waitOpponentPanel.gameObject.SetActive(false);
            SwitchToGameScene(LoadGameScene);
        }

        private void JoinByIp()
        {
            CheckIp();

            var transport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;

            if (transport)
            {
                transport.SetConnectionData(ipInput.text, 7777);
            
                if (!NetworkManager.Singleton.StartClient())
                    throw new Exception("Can't start client!");
                
                SwitchToGameScene(null);
            }
        }

        private void CheckIp()
        {
            var text = ipInput.text;
            var regex = new Regex(@"(\d{1,3}[.]){3}\d{1,3}");
            
            if (!regex.Match(text).Success)
                throw new Exception($"Incorrect ip address\n{text}");
        }

        private void SwitchToGameScene(Action callback)
        {
            startGameTimerPanel.Show(3, () => callback?.Invoke());
        }

        private void LoadGameScene()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
}
