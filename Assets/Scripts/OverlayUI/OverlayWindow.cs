using UnityEngine;

namespace PingPong.OverlayUI
{
    public abstract class OverlayWindow : MonoBehaviour
    {
        public virtual void Init()
        {
            gameObject.SetActive(false);
        }
        
        protected void Show()
        {
            OverlayUI.Show(this);
        }
        
        protected void Show(params object[] args)
        {
            OverlayUI.Show(this, args);
        }

        protected void Hide()
        {
            OverlayUI.Hide(this);
        }
        
        public virtual void OnShow(params object[] args)
        {
        }

        public virtual void OnHide()
        {
        }
    }
}