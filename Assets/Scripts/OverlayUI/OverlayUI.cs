using System;
using System.Collections.Generic;
using UnityEngine;

namespace PingPong.OverlayUI
{
    public class OverlayUI : MonoBehaviour
    {
        private static OverlayUI _instance;
        protected static OverlayUI Instance => _instance ??= FindObjectOfType<OverlayUI>();

        [SerializeField]
        private OverlayWindow[] windows;

        private Dictionary<Type, OverlayWindow> windowsByType = new Dictionary<Type, OverlayWindow>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            foreach (var window in windows)
            {
                var type = window.GetType();
                
                if (windowsByType.ContainsKey(type))
                    continue;

                windowsByType[type] = window;
                window.Init();
            }
        }

        public static void Show<T>(params object[] args)
        {
            var window = Instance.GetWindow<T>();
            Show(window, args);
        }

        public static void Show(OverlayWindow window, params object[] args)
        {
            window.OnShow(args);
            window.gameObject.SetActive(true);
        }

        public static void Hide<T>()
        {
            var window = Instance.GetWindow<T>();
            Hide(window);
        }

        public static void Hide(OverlayWindow window)
        {
            window.OnHide();
            window.gameObject.SetActive(false);
        }

        private OverlayWindow GetWindow<T>()
        {
            var type = typeof(T);

            if (windowsByType.TryGetValue(type, out var window))
                return window;
            
            throw new Exception($"There is no overlay window of type {type.Name}");
        }
    }
}
