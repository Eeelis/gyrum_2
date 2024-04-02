using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CounterContextMenu : ContextMenu
{
    [SerializeField] private int initialThreshold;
    [SerializeField] private TMP_InputField thresholdInputField;
    [SerializeField] private TMP_InputField countInputField;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("Threshold", initialThreshold);
        currentContextMenuData.AddParameter("Count", 0);
        UpdateThresholdInputField();
        UpdateCountInputField();

        this.associatedPart = associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        UpdateThresholdInputField();
        UpdateCountInputField();
    }

    public void SetThreshold(string arg)
    {
        int threshold = Int32.Parse(arg);
        currentContextMenuData.AddParameter("Threshold", threshold);

        SendContextMenuDataToAssociatedPart();
    }

    public void SetCount(string arg)
    {
        int count = Int32.Parse(arg);
        currentContextMenuData.AddParameter("Count", count);

        SendContextMenuDataToAssociatedPart();
    }

    private void UpdateThresholdInputField()
    {
        thresholdInputField.text = currentContextMenuData.GetParameter<int>("Threshold").ToString();
    }

    private void UpdateCountInputField()
    {
        countInputField.text = currentContextMenuData.GetParameter<int>("Count").ToString();
    }
}
