using System;

namespace PFS.Assets.Scripts.Models.UI
{
    public class PopupModel
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ButtonText { get; private set; }
        public bool IsActiveCloseButton { get; private set; }
        public Action Callback { get; private set; }
        public bool IsCloaseAfterAction { get; private set; }

        public PopupModel(string title, string description, string buttonText, bool isActiveCloseButton, Action callback, bool isCloseAfterAction = true)
        {
            Title = title;
            Description = description;
            ButtonText = buttonText;
            IsActiveCloseButton = isActiveCloseButton;
            Callback = callback;
            IsCloaseAfterAction = isCloseAfterAction;
        }
    }
}