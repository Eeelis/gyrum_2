using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ClockContextMenu : ContextMenu
{
    [SerializeField] private TMP_InputField rateInputField;

    public override void Initialize(Part associatedPart)
    {
        parameters["Rate"] = 0;

        base.Initialize(associatedPart);
    }
    
    public override void UpdateUI()
    {
        rateInputField.text = parameters["Rate"].ToString();
    }

    public void SetRate(string arg)
    {
        parameters["Rate"] = Int32.Parse(arg);
        UpdateAssociatedPart();
    }
}
