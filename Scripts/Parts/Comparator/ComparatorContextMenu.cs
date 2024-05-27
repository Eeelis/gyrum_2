using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ComparatorContextMenu : ContextMenu
{
    [SerializeField] private TMP_InputField comparisonValueInputField;
    [SerializeField] private TMP_Dropdown operationDropdown;
    [SerializeField] private TMP_Dropdown modeDropdown;

    public override void Initialize(Part associatedPart)
    {
        parameters["ComparisonValue"] = 0;
        parameters["Operation"] = 0;
        parameters["Mode"] = 0; 

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        comparisonValueInputField.text = parameters["ComparisonValue"].ToString();
    }

    public void SetComparisonValue(string arg)
    {
        parameters["ComparisonValue"] = Int32.Parse(arg);
        UpdateAssociatedPart();
    }

    public void SetOperation()
    {
        parameters["Operation"] = operationDropdown.value;
        UpdateAssociatedPart();
    }

    public void SetMode()
    {
        parameters["Mode"] = modeDropdown.value;
        UpdateAssociatedPart();
    }
}
