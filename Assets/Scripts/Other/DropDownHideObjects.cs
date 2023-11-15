using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownHideObjects : MonoBehaviour, IPointerClickHandler
{
    [Header("Objects")]
    public GameObject[] hideObjects;

    [Header("Params")]
    public bool overrideSortingForTemplate;
    [Helper.ConditionalHide("overrideSortingForTemplate", true)]
    public int sortingOrder;

    private TMP_Dropdown dropdown;
    private Canvas templateCanvas;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideObjects(true);
        if (overrideSortingForTemplate)
        {
            StartCoroutine(WaitTemplateCanvas());
        }
        StartCoroutine(WaitBlocker());
    }

    private void HideObjects(bool hide)
    {
        foreach (GameObject gm in hideObjects)
        {
            gm.SetActive(!hide);
        }
    }

    private IEnumerator WaitTemplateCanvas()
    {
        yield return new WaitForEndOfFrame();
        if (!templateCanvas)
        {
            templateCanvas = dropdown.template.GetComponent<Canvas>();

            dropdown.transform.Find("Dropdown List").GetComponent<Canvas>().sortingOrder = sortingOrder;
        }
        templateCanvas.sortingOrder = sortingOrder;
    }

    private IEnumerator WaitBlocker()
    {
        yield return new WaitForEndOfFrame();
        GameObject blockerGM = GameObject.Find("Blocker");
        if (blockerGM)
        {
            Canvas cvs = blockerGM.GetComponent<Canvas>();
            if (cvs)
            {
                cvs.overrideSorting = overrideSortingForTemplate;
                if (overrideSortingForTemplate)
                {
                    cvs.sortingOrder = sortingOrder - 1;
                }
            }
            yield return new WaitUntil(() => !blockerGM);
            HideObjects(false);
        }
    }
}