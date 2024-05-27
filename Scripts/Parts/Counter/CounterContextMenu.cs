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
        parameters["Threshold"] = 2;
        parameters["Count"] = 0;

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        countInputField.text = parameters["Count"].ToString();
        thresholdInputField.text = parameters["Threshold"].ToString();
    }

    public void SetThreshold(string arg)
    {
        parameters["Threshold"] = Int32.Parse(arg);
        UpdateAssociatedPart();
    }

    public void SetCount(string arg)
    {
        parameters["Count"] = Int32.Parse(arg);
        UpdateAssociatedPart();
    }


}
