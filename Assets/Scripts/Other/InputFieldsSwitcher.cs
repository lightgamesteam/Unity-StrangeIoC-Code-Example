using System.Collections.Generic;
using UnityEngine;

public class InputFieldsSwitcher : MonoBehaviour
{
    [SerializeField] private List<TMPro.TMP_InputField> inputFields;

    private int currentSelectedField = -1;

    private void OnEnable()
    {
        Init();

        MainContextView.AddListenerStrangeEvent(EventGlobal.E_TabKeyDown, ProcessFieldsSwitching);
    }

    private void OnDisable()
    {
        MainContextView.RemoveListenerStrangeEvent(EventGlobal.E_TabKeyDown, ProcessFieldsSwitching);
    }

    private void Init()
    {
        foreach (var inputField in inputFields)
        {
            inputField.onSelect.AddListener((string data) => UpdateSelectedField(inputField));
        }
    }

    private void ProcessFieldsSwitching()
    {
        if (currentSelectedField >= 0)
        {
            currentSelectedField++;

            if (currentSelectedField >= inputFields.Count)
            {
                currentSelectedField = 0;
            }

            inputFields[currentSelectedField].Select();
        }
    }

    private void UpdateSelectedField(TMPro.TMP_InputField field)
    {
        currentSelectedField = inputFields.IndexOf(field);
    }
}