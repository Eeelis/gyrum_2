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
        parameters["Bounciness"] = 0;
        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        bouncinessSlider.value = (int)parameters["Bounciness"];
        bouncinessText.text = parameters["Bounciness"].ToString();
    }

    public void SetBounciness()
    {
        parameters["Bounciness"] = (int)bouncinessSlider.value;
        UpdateAssociatedPart();
        UpdateUI();
    }
}
