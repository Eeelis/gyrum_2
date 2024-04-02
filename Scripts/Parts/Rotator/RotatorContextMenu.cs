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
        currentContextMenuData.AddParameter("ContinousRotation", true);
        currentContextMenuData.AddParameter("RotationSpeed", 3);
        currentContextMenuData.AddParameter("Direction", -1);
        currentContextMenuData.AddParameter("TargetRotation", 0);

        continousRotationToggle.isOn = !currentContextMenuData.GetParameter<bool>("ContinousRotation");
        targetRotationInputField.text = currentContextMenuData.GetParameter<int>("TargetRotation").ToString();
        selectionBox.transform.position = directionRight.transform.position;
        rotationSpeedSlider.value = currentContextMenuData.GetParameter<int>("RotationSpeed");

        UpdateRotationSpeedText();

        this.associatedPart =  associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        if (parameterName == "RotationSpeed")
        {
            rotationSpeedSlider.value = currentContextMenuData.GetParameter<int>("RotationSpeed");
            UpdateRotationSpeedText();
        }
        if (parameterName == "TargetRotation")
        {
            targetRotationInputField.text = currentContextMenuData.GetParameter<int>("TargetRotation").ToString();
        }
        if (parameterName == "ContinuousRotation")
        {
            continousRotationToggle.isOn = !currentContextMenuData.GetParameter<bool>("ContinousRotation");
        }
    }

    public void UpdateRotationSpeedText()
    {
        rotationSpeedText.text = rotationSpeedSlider.value.ToString();
    }

    public void SetRotationSpeed()
    {
        int speed = (int)rotationSpeedSlider.value;
        currentContextMenuData.AddParameter("RotationSpeed", speed);

        UpdateRotationSpeedText();
        SendContextMenuDataToAssociatedPart();
    }

    public void SetTargetRotation(string arg)
    {
        int targetRotation = Int32.Parse(arg);
        currentContextMenuData.AddParameter("TargetRotation", targetRotation);
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionLeft()
    {
        selectionBox.transform.position = directionLeft.transform.position;
        currentContextMenuData.AddParameter("Direction", 1);
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionRight()
    {
        selectionBox.transform.position = directionRight.transform.position;
        currentContextMenuData.AddParameter("Direction", -1);
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionShortest()
    {
        selectionBox.transform.position = directionShortest.transform.position;
        currentContextMenuData.AddParameter("Direction", 2);
        SendContextMenuDataToAssociatedPart();
    }   

    public void SetContinousRotation()
    {
        bool continuousDirection = !continousRotationToggle.isOn;
        currentContextMenuData.AddParameter("ContinousRotation", continuousDirection);
        SendContextMenuDataToAssociatedPart();
    }
}
