using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlatformContextMenu : ContextMenu
{
    [SerializeField] private Slider bouncinessSlider;
    [SerializeField] private TMP_Text bouncinessText;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("Bounciness", 0);
        bouncinessSlider.value = 0;
        UpdateBouncinessText();

        this.associatedPart = associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        bouncinessSlider.value = currentContextMenuData.GetParameter<int>(parameterName);
        UpdateBouncinessText();
    }

    public void UpdateBouncinessText()
    {
        bouncinessText.text = bouncinessSlider.value.ToString();
    }

    public void SetBounciness()
    {
        int bounciness = (int)bouncinessSlider.value;
        bouncinessText.text = bounciness.ToString();
        currentContextMenuData.AddParameter("Bounciness", bounciness);

        SendContextMenuDataToAssociatedPart();
    }
}
