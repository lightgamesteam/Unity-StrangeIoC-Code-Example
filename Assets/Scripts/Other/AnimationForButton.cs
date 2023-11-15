using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationForButton : MonoBehaviour, IPointerDownHandler
{
    public bool isCustomAnimator = false;
    [Helper.ConditionalHide("isCustomAnimator", true)]
    public Animator animatorGragDrop;
    
    Animator animator;

    private void Awake()
    {
        if (isCustomAnimator)
        {
            animator = animatorGragDrop;
        }
        else
        {
            animator = GetComponentInChildren(typeof(Animator), true) as Animator;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (animator)
        {
            animator.Play("Pressed");
        }
    }
}
