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
        currentContextMenuData.AddParameter("Range", 2);
        rangeSlider.value = 2;
        UpdateRangeText();

        this.associatedPart = associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        rangeSlider.value = currentContextMenuData.GetParameter<int>("Range");
        UpdateRangeText();
    }

    public void UpdateRangeText()
    {
        rangeText.text = rangeSlider.value.ToString();
    }

    public void SetRange()
    {
        int range = (int)rangeSlider.value;
        rangeText.text = range.ToString();
        currentContextMenuData.AddParameter("Range", range);

        SendContextMenuDataToAssociatedPart();
    }

}
