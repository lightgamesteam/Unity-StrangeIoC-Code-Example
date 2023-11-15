using UnityEngine;

public class BooksScrollHelper : MonoBehaviour
{
    private ScrollController[] horizontalScrolls;
    [SerializeField] private ScrollController verticalScroll;
    public void OnScrollValueChanged()
    {
        horizontalScrolls = GetComponentsInChildren<ScrollController>();
    }
    private void FixedUpdate()
    {
        if(Input.mouseScrollDelta.x < -0.1f || Input.mouseScrollDelta.x > 0.1f) // Horizontal laptop touchpad delta
        {
            EnablingScrolls(false, true);
        }
        else if (Input.mouseScrollDelta.y < -0.1f || Input.mouseScrollDelta.y > 0.1f) // Vertical laptop touchpad delta
        {
            EnablingScrolls(true, false);
        }
        else if(Input.mouseScrollDelta.x == 0 || Input.mouseScrollDelta.x == 0) // Not scrolling
        {
            EnablingScrolls(true, true);
        }
    }
    private void EnablingScrolls(bool verticalScrollEnabled, bool horizontalScrollsEnabled)
    {
        verticalScroll.enabled = verticalScrollEnabled;

        if (horizontalScrolls != null && horizontalScrolls.Length > 0)
        {
            for (int i = 0; i < horizontalScrolls.Length; i++)
            {
                horizontalScrolls[i].enabled = horizontalScrollsEnabled;
            }
        }
    }
}
