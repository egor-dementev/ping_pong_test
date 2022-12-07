using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
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

        private void Awake()
        {
            startAsHostButton.onClick.RemoveAllListeners();
            startAsHostButton.onClick.AddListener(StartAsHost);
            
            joinByIpButton.onClick.RemoveAllListeners();
            joinByIpButton.onClick.AddListener(JoinByIp);
        }

        private void StartAsHost()
        {
            
        }

        private void JoinByIp()
        {
            CheckIp();
            
            
        }

        private void CheckIp()
        {
            var text = ipInput.text;
            var regex = new Regex(@"(\d{1,3}[.]){3}\d{1,3}");
            
            if (!regex.Match(text).Success)
                throw new Exception($"Incorrect ip address\n{text}");
        }
    }
}
