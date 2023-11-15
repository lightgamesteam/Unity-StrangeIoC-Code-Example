using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuizQuestionBackground : MonoBehaviour
{
    public enum BackgroundImageState
    {
        Default,
        Win,
        Lose,
        Selected
    }

    [Header("Images")]
    [SerializeField] private Image topImage;
    [SerializeField] private Image bottomImage;
    [SerializeField] private Image glowImage;

    [Header("Back Colors")]
    [SerializeField] private Color defaultTopColor;
    [SerializeField] private Color defaultBottomColor;
    [Space]
    [SerializeField] private Color winTopColor;
    [SerializeField] private Color winBottomColor;
    [Space]
    [SerializeField] private Color loseTopColor;
    [SerializeField] private Color loseBottomColor;
    [Space]
    [SerializeField] private Color selectedTopColor;
    [SerializeField] private Color selectedBottomColor;

    [Header("Animation Params")]
    [SerializeField, Range(0.0f, 1.0f)] private float colorChangingDuration;
    [SerializeField, Range(0.0f, 1.0f)] private float glowFadeValue;

    public void SetBackImageState(BackgroundImageState state, bool withGlow = false)
    {
        Color topColor = Color.clear;
        Color bottomColor = Color.clear;

        switch (state)
        {
            case BackgroundImageState.Default:
                topColor = defaultTopColor;
                bottomColor = defaultBottomColor;
                break;
            case BackgroundImageState.Lose:
                topColor = loseTopColor;
                bottomColor = loseBottomColor;
                break;
            case BackgroundImageState.Win:
                topColor = winTopColor;
                bottomColor = winBottomColor;
                break;
            case BackgroundImageState.Selected:
                topColor = selectedTopColor;
                bottomColor = selectedBottomColor;
                break;
            default:
                break;
        }

        topImage.DOColor(topColor, colorChangingDuration);
        bottomImage.DOColor(bottomColor, colorChangingDuration);

        glowImage.color = new Color(topColor.r, topColor.g, topColor.b, 0.0f);

        if (withGlow)
        {
            glowImage.DOFade(glowFadeValue, colorChangingDuration);
        }
    }
}
