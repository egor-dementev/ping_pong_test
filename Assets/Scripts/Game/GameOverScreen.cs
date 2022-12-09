using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace PingPong.Game
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text resultText;
        [SerializeField]
        private Button exitButton;

        public void Show(bool win)
        {
            resultText.text = win
                ? "You win!"
                : "You lose :(";
            
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(
                () =>
                {
                    NetworkManager.Singleton.Shutdown();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            );

            gameObject.SetActive(true);
        }
    }
}