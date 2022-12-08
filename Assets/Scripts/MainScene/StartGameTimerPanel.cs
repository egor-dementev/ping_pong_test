using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PingPong.MainScene
{
    public class StartGameTimerPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timer;

        public void Show(float seconds, Action callback)
        {
            gameObject.SetActive(true);
            StartCoroutine(TimerRoutine(seconds, callback));
        }

        private IEnumerator TimerRoutine(float seconds, Action callback)
        {
            while (seconds > 0)
            {
                timer.text = Mathf.CeilToInt(seconds).ToString();
                seconds -= Time.deltaTime;

                yield return null;
            }
            
            callback?.Invoke();
        }
    }
}