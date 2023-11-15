using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static PFS.Assets.Scripts.Views.Library.UIMovePanelView;

namespace PFS.Assets.Scripts.Views.Components
{
    public class UIControlContentSizeView : BaseView
    {
        [Header("Panel")]
        [SerializeField] private ScrollRect panelScroll;

        [Header("Ataptive books content params")]
        [SerializeField, Range(0f, 1000f), Tooltip("4:3")] private float contentTopSizeTablet;
        [SerializeField, Range(0f, 1000f), Tooltip("16:9")] private float contentTopSizePhone;

        private float screenAspectRatio1 = 16f / 9f;    //max screen
        private float screenAspectRatio2 = 4f / 3f;     //min screen

        private int baseTopLayoutContent, adaptiveTopLayoutContent;
        private VerticalLayoutGroup verticalLayoutGroup;
        private IEnumerator layoutTopIEnum;

        public void LoadView()
        {
            SetContentLayout();

            Dispatcher.AddListener(EventGlobal.E_MoveLibraryPanel, BackPanelTopLayout);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_MoveLibraryPanel, BackPanelTopLayout);
        }

        private void SetContentLayout()
        {
            verticalLayoutGroup = panelScroll?.content?.GetComponent<VerticalLayoutGroup>();
            if (verticalLayoutGroup == null)
            {
                return;
            }

            baseTopLayoutContent = verticalLayoutGroup.padding.top;

            float topTablet, topPhone;
            float realScreeenResolutionDivision = (float)Screen.width / Screen.height;
            GetCoef(contentTopSizeTablet, contentTopSizePhone, out topTablet, out topPhone);

            adaptiveTopLayoutContent = (int)Math.Truncate((decimal)(topTablet * realScreeenResolutionDivision + topPhone));
        }

        private void GetCoef(float x, float y, out float a, out float b)
        {
            a = (y - x) / (screenAspectRatio1 - screenAspectRatio2);
            b = y - screenAspectRatio1 * a;
        }

        private void BackPanelTopLayout(IEvent e)
        {
            ScreenState res = (ScreenState)e.data;

            if (verticalLayoutGroup == null)
            {
                return;
            }

            if (layoutTopIEnum != null)
            {
                StopCoroutine(layoutTopIEnum);
            }

            if (res == ScreenState.Down)
            {
                layoutTopIEnum = SetLayoutTop(baseTopLayoutContent);
            }
            else if (res == ScreenState.Up)
            {
                layoutTopIEnum = SetLayoutTop(adaptiveTopLayoutContent);
            }

            StartCoroutine(layoutTopIEnum);
        }

        private IEnumerator SetLayoutTop(int top)
        {
            verticalLayoutGroup.padding.top = top;

            yield return new WaitForEndOfFrame();
            verticalLayoutGroup.enabled = false; // need for visualization

            yield return new WaitForEndOfFrame();
            verticalLayoutGroup.enabled = true; // need for visualization
        }
    }
}