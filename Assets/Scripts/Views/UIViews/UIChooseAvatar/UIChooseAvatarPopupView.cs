using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Views.Avatar
{
    public class UIChooseAvatarPopupView : BaseView
    {
        [Inject]
        public PoolModel Pool { get; set; }

        [Inject]
        public Analytics Analytics { get; set; }

        [Inject]
        public ChildModel ChildModel { get; set; }


        [Header("Localization")]
        [SerializeField] private TMPro.TextMeshProUGUI titleText;

        [Header("UI")]
        [SerializeField] private Button closeButton;
        [SerializeField] private GridLayoutGroup avatarItemsContainer;
        [SerializeField] private GameObject loader;

        private List<UIAvatarItemView> avatarItems;
        private UIAvatarItemView selectedItem;

        public void LoadView()
        {
            loader.SetActive(false);

            CreateAvatars();
            InitSelectedAvatar();
            SetLocalization();

            closeButton.onClick.AddListener(Close);

            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_AvatarItemClick, ProcessAvatarChanging);
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_AvatarItemClick, ProcessAvatarChanging);
        }

        private void SetLocalization()
        {
            titleText.text = LocalizationManager.GetLocalizationText(LocalizationKeys.ChooseAvatarKey);
        }

        private void CreateAvatars()
        {
            avatarItems = new List<UIAvatarItemView>();

            foreach (var avatar in Pool.Avatars)
            {
                var avatarItem = Instantiate(avatar, avatarItemsContainer.transform);
                avatarItem.Init(UIAvatarItemView.AvatarItemState.ChooseScreen);

                avatarItems.Add(avatarItem);
            }
        }

        private void Close()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UIChooseAvatarPopup, showSwitchAnim = false });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }

        private void ProcessAvatarChanging(IEvent e)
        {
            if (e != null && e.data != null)
            {
                UIAvatarItemView newSelectedItem = e.data as UIAvatarItemView;

                if (newSelectedItem && newSelectedItem != selectedItem)
                {
                    loader.SetActive(true);

                    ChildModel child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
                    child.AvatarId = newSelectedItem.Id;

                    ChildEditRequestModel childRequest = new ChildEditRequestModel();
                    childRequest.InitData(child);

                    childRequest.requestTrueAction = () =>
                    {
                        Debug.Log("Avatar Changing Success");

                        Analytics.LogEvent(EventName.ActionChooseAvatar,
                            new Dictionary<Property, object>()
                            {
                                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                                { Property.AvatarTag, newSelectedItem.Id}
                            });

                        loader.SetActive(false);

                        selectedItem.SetSelected(false);
                        newSelectedItem.SetSelected(true);

                        selectedItem = newSelectedItem;

                        Dispatcher.Dispatch(EventGlobal.E_UserAvatarUpdated);
                    };

                    childRequest.requestFalseAction = () =>
                    {
                        Debug.Log("Avatar Changing Fail");

                        loader.SetActive(false);
                    };

                    Dispatcher.Dispatch(EventGlobal.E_EditChild, childRequest);
                }
            }
        }

        private void InitSelectedAvatar()
        {
            string currentAvatarId = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId).AvatarId;

            if (!string.IsNullOrEmpty(currentAvatarId))
            {
                foreach (var avatarItem in avatarItems)
                {
                    if (avatarItem.Id == currentAvatarId)
                    {
                        selectedItem = avatarItem;
                        break;
                    }
                }
            }

            if (selectedItem == null)
            {
                selectedItem = avatarItems[0];
            }

            selectedItem.SetSelected(true);
        }
    }
}