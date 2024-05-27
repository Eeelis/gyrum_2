using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class RotatorContextMenu : ContextMenu
{
    [SerializeField] private Slider rotationSpeedSlider;
    [SerializeField] private TMP_Text rotationSpeedText;
    [SerializeField] private TMP_InputField targetRotationInputField;
    [SerializeField] private Toggle continousRotationToggle;

    [SerializeField] private GameObject directionLeft;
    [SerializeField] private GameObject directionRight;
    [SerializeField] private GameObject directionShortest;
    [SerializeField] private GameObject selectionBox;

    public override void Initialize(Part associatedPart)
    {
        parameters["ContinousRotation"] = true;
        parameters["RotationSpeed"] = 3;
        parameters["Direction"] = -1;
        parameters["TargetRotation"] = 0;

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        continousRotationToggle.isOn = !(bool)parameters["ContinousRotation"];
        targetRotationInputField.text = parameters["TargetRotation"].ToString();
        rotationSpeedSlider.value = (int)parameters["RotationSpeed"];
        rotationSpeedText.text = parameters["RotationSpeed"].ToString();

        if ((int)parameters["Direction"] == 1)
        {
            selectionBox.transform.position = directionLeft.transform.position;
        }
        else 
        {
            selectionBox.transform.position = directionRight.transform.position;
        }
    }

    public void UpdateRotationSpeedText()
    {
        rotationSpeedText.text = rotationSpeedSlider.value.ToString();
    }

    public void SetRotationSpeed()
    {
        parameters["RotationSpeed"] = (int)rotationSpeedSlider.value;
        UpdateUI();
        UpdateAssociatedPart();
    }

    public void SetTargetRotation(string arg)
    {
        parameters["TargetRotation"] = Int32.Parse(arg);
        UpdateAssociatedPart();
    }

    public void SetDirectionLeft()
    {
        selectionBox.transform.position = directionLeft.transform.position;
        parameters["Direction"] = 1;
        UpdateAssociatedPart();
    }

    public void SetDirectionRight()
    {
        selectionBox.transform.position = directionRight.transform.position;
        parameters["Direction"] = -1;
        UpdateAssociatedPart();
    }

    public void SetDirectionShortest()
    {
        selectionBox.transform.position = directionShortest.transform.position;
        parameters["Direction"] = 2;
        UpdateAssociatedPart();
    }   

    public void SetContinousRotation()
    {
        parameters["ContinousRotation"] = !continousRotationToggle.isOn;
        UpdateAssociatedPart();
    }
}
