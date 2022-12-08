using System.Collections;
using TMPro;
using UnityEngine;

namespace PingPong
{
    public class WaitOpponentPanel : MonoBehaviour
    {
        private const string WaitingOpponentText = "Waiting opponent ";
        private const int MaxDots = 3;
        
        [SerializeField]
        private TMP_Text text;
        
        private void OnEnable()
        {
            StartCoroutine(Anim());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator Anim()
        {
            text.text = WaitingOpponentText;
            
            var dots = 0;
            var dt = 0f;
            
            while (true)
            {
                dt += Time.deltaTime;

                if (dt > 1)
                {
                    dt -= 1;
                    dots++;

                    if (dots > MaxDots)
                    {
                        dots = 0;
                        text.text = WaitingOpponentText;
                    }
                    else
                    {
                        text.text += ".";
                    }
                }

                yield return null;
            }
        }
    }
}
