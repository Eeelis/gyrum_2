using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ClockContextMenu : ContextMenu
{
    [SerializeField] private int initialRate;
    [SerializeField] private TMP_InputField rateInputField;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("Rate", initialRate);
        UpdateRateInputField();

        this.associatedPart = associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        UpdateRateInputField();
    }

    public void SetRate(string arg)
    {
        int rate = Int32.Parse(arg);
        currentContextMenuData.AddParameter("Rate", rate);

        SendContextMenuDataToAssociatedPart();
    }

    private void UpdateRateInputField()
    {
        rateInputField.text = currentContextMenuData.GetParameter<int>("Rate").ToString();
    }
}
