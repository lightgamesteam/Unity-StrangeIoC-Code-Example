using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.SplashScreen
{
    public class UISplashScreenView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Image logo;
        [SerializeField] private Image bg;
        [SerializeField] private Image loader;

        [Header("Animation params")]
        [SerializeField] private RectTransform startLogoAnimPoint;
        [SerializeField, Range(0f, 3f)] private float logoStartAnimDuration;
        [SerializeField, Range(0f, 3f)] private float logoDelayAfterFade;
        [SerializeField, Range(0f, 3f)] private float logoEndAnimDuration;

        private SplashParams data;
        private bool countryCodeUpdated = false;

        public void LoadView()
        {
            if (otherData != null)
            {
                if (otherData is SplashParams)
                {
                    data = (SplashParams)otherData;
                }
            }

            Dispatcher.Dispatch(EventGlobal.E_GetUserCountryCode, new BasicRequestModel(
            () =>
            {
                countryCodeUpdated = true;
            },
            () =>
            {
                Debug.LogError("Get user country code error!");
            }));

            StartCoroutine(AnimationCoroutine());
        }

        public void RemoveView()
        {

        }

        private IEnumerator AnimationCoroutine()
        {
            loader.color = new Color(loader.color.r, loader.color.g, loader.color.b, 0f);

            float logoPositionY = logo.transform.localPosition.y;
            logo.transform.localPosition = new Vector3(logo.transform.localPosition.x, startLogoAnimPoint.localPosition.y, logo.transform.localPosition.z);
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, 0f);

            logo.transform.DOMoveY(logoPositionY, logoStartAnimDuration);
            logo.DOFade(1f, logoStartAnimDuration);

            yield return new WaitForSeconds(logoStartAnimDuration + logoDelayAfterFade);

            if (data.isLogIn)
            {
                loader.DOFade(1f, logoStartAnimDuration);

                yield return new WaitUntil(() => countryCodeUpdated);

                if (PlayerPrefsModel.CountryCode == Conditions.CountryCodes.Norway.ToDescription())
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.Norwegian.ToDescription());
                }
                else if (PlayerPrefsModel.CountryCode == Conditions.CountryCodes.China.ToDescription())
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.Chinese.ToDescription());
                }
                else if (PlayerPrefsModel.CountryCode == Conditions.CountryCodes.Thailand.ToDescription())
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.Thai.ToDescription());
                }
                else if (PlayerPrefsModel.CountryCode == Conditions.CountryCodes.Denmark.ToDescription())
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.Danish.ToDescription());
                }
                else if (PlayerPrefsModel.CountryCode == Conditions.CountryCodes.NyNorsk.ToDescription())
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.NyNorsk.ToDescription());
                }
                else
                {
                    LocalizationManager.Instance.SetLanguage(Conditions.Languages.English.ToDescription());
                }

                Dispatcher.Dispatch(EventGlobal.E_Reinitlocalization);

                data.action?.Invoke(null);
            }

            logo.transform.DOScale(logo.transform.localScale / 2, logoEndAnimDuration);
            logo.DOFade(0f, logoEndAnimDuration);
            bg.DOFade(0f, logoEndAnimDuration);

            yield return new WaitForSeconds(logoEndAnimDuration);

            if (!data.isLogIn)
            {
                data.action?.Invoke(null);
            }

            Dispatcher.Dispatch(EventGlobal.E_HideScreen, UIScreens.UISplashScreen);
        }

        public struct SplashParams
        {
            public bool isLogIn;
            public UnityAction<object> action;
        }
    }
}