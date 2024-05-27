using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedometerContextMenu : ContextMenu
{
    [SerializeField] private Slider rangeSlider;
    [SerializeField] private TMP_Text rangeText;

    public override void Initialize(Part associatedPart)
    {
        parameters["Range"] = 5;
        
        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        rangeSlider.value = (int)parameters["Range"];
        rangeText.text = rangeSlider.value.ToString();
    }


    public void SetRange()
    {
        parameters["Range"] = (int)rangeSlider.value;
        UpdateUI();
        UpdateAssociatedPart();
    }

}
