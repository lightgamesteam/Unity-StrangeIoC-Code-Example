using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.SwitchScreen
{
    public class UISwitchScreensAnimationView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Image image;

        [Header("Params")]
        [SerializeField, Range(0f, 10f)] private float animSpeed;

        [SerializeField, Range(0f, 5f)] private float blackScreenDelay;

        private IEnumerator animIEnum;

        public void LoadView()
        {
            PlayAnimation();
        }

        public void RemoveView()
        {

        }

        public void PlayAnimation()
        {
            if (animIEnum != null)
            {
                StopCoroutine(animIEnum);
            }
            animIEnum = Animation();
            StartCoroutine(animIEnum);
        }

        private IEnumerator Animation()
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

            yield return new WaitForSeconds(blackScreenDelay);

            while (image.color.a > 0)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 0f, Time.deltaTime * animSpeed));

                yield return new WaitForEndOfFrame();
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }
    }
}