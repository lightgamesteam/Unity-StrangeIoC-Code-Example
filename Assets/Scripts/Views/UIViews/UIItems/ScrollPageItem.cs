using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace PFS.Assets.Scripts.Views.Components
{
    public class ScrollPageItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Image image;

        [Header("Colors")]
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color defaultColor;

        public void Init(UnityAction buttonCallback)
        {
            button.onClick.AddListener(buttonCallback);
        }

        public void SetSelectedState(bool isSelected)
        {
            image.color = isSelected ? selectedColor : defaultColor;
        }
    }
}