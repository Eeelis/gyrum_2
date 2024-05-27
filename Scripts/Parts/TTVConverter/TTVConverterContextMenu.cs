using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TTVConverterContextMenu : ContextMenu
{
    [SerializeField] private TMP_InputField storedValueInputField;

    public override void Initialize(Part associatedPart)
    {
        parameters["StoredValue"] = 0;

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        storedValueInputField.text = parameters["StoredValue"].ToString();
    }

    public void SetValue(string arg)
    {
        parameters["StoredValue"] = int.Parse(arg);
        UpdateAssociatedPart();
    }
}
