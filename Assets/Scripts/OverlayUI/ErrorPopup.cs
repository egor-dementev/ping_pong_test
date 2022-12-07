using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PingPong.OverlayUI
{
    public class ErrorPopup : OverlayWindow
    {
        [SerializeField]
        private TMP_Text errorText;
        [SerializeField]
        private Button okButton;
        
        public override void Init()
        {
            base.Init();
            Application.logMessageReceived += OnLogMessage;
            
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(Hide);
        }

        private void OnLogMessage(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
                Show(condition);
        }

        public override void OnShow(params object[] args)
        {
            base.OnShow();

            if (args == null || args.Length != 1)
            {
                Hide();
                return;
            }

            errorText.text = args[0].ToString();
        }
    }
}