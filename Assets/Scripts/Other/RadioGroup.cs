using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
public class RadioEvent : UnityEvent<int>
{  
}
public class RadioGroup : MonoBehaviour
{
    [SerializeField] private List<Image> radioButtons = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    [SerializeField] private bool needsColoring;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color switchColor;
    public RadioEvent triggeredEvent;

    public void OnRadioClick(int number)
    {
        foreach(Image a in radioButtons)
        {
            a.enabled = false;

        }
        radioButtons[number-1].enabled = true;
        if(triggeredEvent == null)
        {
            triggeredEvent = new RadioEvent();
        }
        triggeredEvent.Invoke(number);

        if (needsColoring)
        {
            TextColoring(number);
        }
    }

  

    private void Awake()
    {
        int buttonsNum = 1;
        foreach(Image a in radioButtons)
        {
            int tempInt = buttonsNum;
            a.enabled = false;
            a.GetComponent<Button>().onClick.AddListener(()=>{ OnRadioClick(tempInt); });
            buttonsNum++;
        }
        OnRadioClick(radioButtons.Count);
    }

    public RadioEvent GetTrigger()
    {
        return triggeredEvent;
    }

    private void TextColoring(int index)
    {
        foreach(var a in texts)
        {
            a.color = baseColor;
        }
        texts[index - 1].color = switchColor;
    }
}
