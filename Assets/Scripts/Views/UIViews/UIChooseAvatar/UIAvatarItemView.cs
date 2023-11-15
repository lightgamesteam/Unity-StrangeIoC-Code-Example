using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Avatar
{
    public class UIAvatarItemView : BaseView
    {
        public enum AvatarItemState { Profile = 0, ChooseScreen }

        [Header("Options")]
        [SerializeField] private string avatarId;

        [Header("UI")]
        [SerializeField] private Button itemButton;
        [SerializeField] private GameObject selectedIcon;
        [SerializeField] private GameObject defaultGlow;
        [SerializeField] private GameObject selectedGlow;

        public string Id { get => avatarId; }

        private AvatarItemState itemState = AvatarItemState.Profile;

        public void Init(AvatarItemState state)
        {
            itemState = state;

            if (itemState == AvatarItemState.Profile)
            {
                itemButton.enabled = false;
                defaultGlow.SetActive(false);
                selectedIcon.SetActive(false);

                RectTransform rectTransform = (transform as RectTransform);
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }
            else
            {
                itemButton.enabled = true;
                defaultGlow.SetActive(true);
                selectedIcon.SetActive(false);

                itemButton.onClick.AddListener(ProcessClick);
            }

            selectedGlow.SetActive(false);
            selectedIcon.SetActive(false);
        }

        public void LoadView()
        {

        }

        public void RemoveView()
        {

        }

        public void SetSelected(bool isSelected)
        {
            if (itemState != AvatarItemState.Profile)
            {
                defaultGlow.SetActive(!isSelected);
                selectedGlow.SetActive(isSelected);
                selectedIcon.SetActive(isSelected);
            }
        }

        private void ProcessClick()
        {
            Dispatcher.Dispatch(EventGlobal.E_AvatarItemClick, this);
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }
    }
}