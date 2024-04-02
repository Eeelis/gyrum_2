using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ComparatorContextMenu : ContextMenu
{
    [SerializeField] private TMP_InputField storedValueInputField;
    [SerializeField] private TMP_Dropdown operations;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("StoredValue", 0);
        currentContextMenuData.AddParameter("Operation", 0);
        UpdateStoredValueInputField();

        this.associatedPart = associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        UpdateStoredValueInputField();
    }

    public void SetStoredValue(string arg)
    {
        int value = Int32.Parse(arg);
        currentContextMenuData.AddParameter("StoredValue", value);

        SendContextMenuDataToAssociatedPart();
    }

    private void UpdateStoredValueInputField()
    {
        storedValueInputField.text = currentContextMenuData.GetParameter<int>("StoredValue").ToString();
    }

    public void SetOperation()
    {
        currentContextMenuData.AddParameter("Operation", operations.value);
        SendContextMenuDataToAssociatedPart();
    }
}
