using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

namespace PFS.Assets.Scripts.Views.BookDetails
{
    public class UIMoreInfoButtonView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [Space(5)]
        [SerializeField] private Image on;
        [SerializeField] private Image off;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float fadeAnimDuration;

        public bool Click { get; private set; } = false;

        public void LoadView()
        {
            button.onClick.AddListener(CkickAction);
        }

        public void RemoveView()
        {

        }

        public void AddListener(UnityAction unityAction) => button.onClick.AddListener(unityAction);

        public void RemoveListener(UnityAction unityAction) => button.onClick.RemoveListener(unityAction);

        public void DisableMoreInfo()
        {
            if (Click)
            {
                button.onClick?.Invoke();
            }
        }

        private void CkickAction()
        {
            Click = !Click;

            Dispatcher.Dispatch(EventGlobal.E_SoundClick);

            InitAnimation(Click);
        }

        #region Animation
        private void InitAnimation(bool switchOn)
        {
            on.DOFade(switchOn ? 1f : 0f, fadeAnimDuration);
            off.DOFade(switchOn ? 0f : 1f, fadeAnimDuration);

            StartCoroutine(WaitBlockButton());
        }

        private IEnumerator WaitBlockButton()
        {
            button.interactable = false;
            yield return new WaitForSeconds(fadeAnimDuration);
            button.interactable = true;
        }
        #endregion
    }
}