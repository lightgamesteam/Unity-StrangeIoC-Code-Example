using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

namespace PFS.Assets.Scripts.Views.Buttons
{
    public class UILikeButtonView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [Space(5)]
        [SerializeField] private Image outline;
        [SerializeField] private Image heart;
        [SerializeField] private Image fillArea;

        [Header("Params")]
        [SerializeField] private Color likedColor;

        [Header("Animations params")]
        [SerializeField, Range(0f, 3f)] private float scaleAnimDuration;
        [SerializeField, Range(1f, 2f)] private float upScaleAnim;

        private bool click = false;

        public void LoadView()
        {
            button.onClick.AddListener(() =>
            {
                click = true;
                Dispatcher.Dispatch(EventGlobal.E_SoundClick);
            });
        }

        public void RemoveView()
        {

        }

        public void AddListener(UnityAction unityAction) => button.onClick.AddListener(unityAction);

        public void RemoveListener(UnityAction unityAction) => button.onClick.RemoveListener(unityAction);

        public void SetInteractable(bool interactable) => button.interactable = interactable;

        public void SetLike(bool liked)
        {
            StopAllCoroutines();

            if (!click)
            {
                SetLikeFast(liked);
                return;
            }
            click = false;

            InitAnimation(liked);
        }

        public void SetLikeFast(bool liked)
        {
            DOTween.Pause(outline);
            DOTween.Pause(heart);
            DOTween.Pause(fillArea);
            DOTween.Pause(transform);

            if (liked)
            {
                outline.color = likedColor;
                heart.color = likedColor;
                fillArea.color = new Color(fillArea.color.r, fillArea.color.g, fillArea.color.b, 1f);
            }
            else
            {
                outline.color = Color.white;
                heart.color = Color.white;
                fillArea.color = new Color(fillArea.color.r, fillArea.color.g, fillArea.color.b, 0f);
            }

            transform.localScale = Vector3.one;
        }

        #region Animation
        private void InitAnimation(bool liked)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SetLikeAnimation(liked));
            }
        }

        private IEnumerator SetLikeAnimation(bool liked)
        {
            transform.DOScale(new Vector3(upScaleAnim, upScaleAnim, 1f), scaleAnimDuration);
            outline.DOColor(liked ? likedColor : Color.white, scaleAnimDuration);
            heart.DOColor(liked ? likedColor : Color.white, scaleAnimDuration);

            yield return new WaitForSeconds(scaleAnimDuration);

            transform.DOScale(Vector3.one, scaleAnimDuration);
            fillArea.DOFade(liked ? 1f : 0f, scaleAnimDuration);
        }
        #endregion
    }
}