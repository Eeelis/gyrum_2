using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TTVConverterContextMenu : ContextMenu
{
    [SerializeField] private int initialValue;
    [SerializeField] private TMP_InputField storedValueInputField;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("StoredValue", initialValue);
        storedValueInputField.text = currentContextMenuData.GetParameter<int>("StoredValue").ToString();

        this.associatedPart = associatedPart;

        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);
        storedValueInputField.text = currentContextMenuData.GetParameter<int>(parameterName).ToString();
    }

    public void SetValue(string arg)
    {
        int value = int.Parse(arg);

        currentContextMenuData.AddParameter("StoredValue", value);

        SendContextMenuDataToAssociatedPart();
    }
}
