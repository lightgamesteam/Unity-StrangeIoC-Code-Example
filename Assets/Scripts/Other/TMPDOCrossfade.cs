using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
namespace TMPTools {
    public static class TMPDOCrossfade
    {
        private static TextMeshProUGUI CreateTempChildTMP(TextMeshProUGUI image)
        {
            TextMeshProUGUI result = null;

            string tempChildName = GetTempChildName(image);
            Transform foundTransform = image.transform.Find(tempChildName);
            GameObject tempGo = foundTransform != null ? foundTransform.gameObject : null;

            if (tempGo == null)
            {
               
                tempGo = new GameObject("TempCloneChild");
                var rt = image.GetComponent<RectTransform>();
                
                var rtPrime = tempGo.AddComponent<RectTransform>();
                result = tempGo.AddComponent<TextMeshProUGUI>();
                rtPrime.SetParent(rt);
                rtPrime.localScale = Vector3.one;
                rtPrime.anchorMin = Vector2.zero;
                rtPrime.anchorMax = Vector2.one;
                //rtPrime.sizeDelta = Vector2.zero;
                rtPrime.SetLeft(0);
                rtPrime.SetRight(0);
                rtPrime.SetBottom(0);
                rtPrime.SetTop(0);
                
                //rtPrime.anchoredPosition = Vector3.zero;// image.gameObject.GetComponent<RectTransform>().anchoredPosition;
                //rtPrime.position = image.gameObject.GetComponent<RectTransform>().position;
                

                
                //result.preserveAspect = image.preserveAspect;
            }
            else
            {
                result = tempGo.GetComponent<TextMeshProUGUI>();
            }
            result.margin = image.margin;
            result.alignment = image.alignment;
            result.fontSize = image.fontSize;
            result.fontStyle = image.fontStyle;
            result.font = image.font;
            return result;
        }

        private static string GetTempChildName(TextMeshProUGUI target) => string.Format("TempCloneChild_{0}", target.GetInstanceID());

        public static float GetAlpha(this Image image) => image.color.a;
        public static void SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }


        private static void RemoveTempChildImage(TextMeshProUGUI childTMP)
        {
            if (childTMP != null)
            {
                Object.Destroy(childTMP.gameObject);
            }
        }

        public static Tweener DOCrossfadeTMP(this TextMeshProUGUI textMeshPro, string to, float duration, System.Action OnComplete = null)
        {
            TextMeshProUGUI childTMP = CreateTempChildTMP(textMeshPro);
            float progress = 0f;
            var finalAlpha = textMeshPro.color.a;

            childTMP.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0f);
            childTMP.text = to;

            return DOTween.To(
                () => progress,
                (curProgress) =>
                {
                    progress = curProgress;

                    float childAlpha = finalAlpha * progress;
                    float imageAlpha = finalAlpha - childAlpha;
                    textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, imageAlpha);
                    childTMP.color = new Color(childTMP.color.r, childTMP.color.g, childTMP.color.b, childAlpha);
                },
                1f, duration)
                .OnComplete(() =>
                {
                    textMeshPro.text = to;
                    textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1f);

                    RemoveTempChildImage(childTMP);

                    OnComplete?.Invoke();
                })
                .OnKill(() =>
                {
                //Note: If you expect this tween will cancel and wish to halt the
                //  animation, remove this next line. It will look bad when you 
                //  start another CrossFadeImage animation on this
                RemoveTempChildImage(childTMP);
                });
        }
    }
}