using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SuperSimpleWindowBase : MonoBehaviour
    {
        public bool IsHiding { get; private set; }

        public virtual float HideShowTime => .25f;

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] RectTransform content;

        protected void Awake()
        {
            //gameObject.SetActive(false);
            OnAwake();
        }

        public virtual void OnAwake() { }
        public virtual void OnShow() { }
        public virtual void OnHide() { }

        private Tween hideShowTween;

        public void Hide()
        {
            canvasGroup.interactable = false;
            IsHiding = true;

            OnHide();

            hideShowTween?.Kill();
            hideShowTween = canvasGroup.DOFade(0, HideShowTime)
                .OnComplete(HideFinalize).SetLink(gameObject);
        }

        private void HideFinalize()
        {
            IsHiding = false;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            IsHiding = false;

            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            gameObject.SetActive(true);

            OnShow();

            hideShowTween?.Kill();
            hideShowTween = canvasGroup.DOFade(1, HideShowTime)
                .OnComplete(ShowFinalize).SetLink(gameObject);
        }

        private void ShowFinalize()
        {
            canvasGroup.interactable = true;
        }
    }
}