using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SamplePlayerContextMenu : ContextMenu
{
    [SerializeField] private TMP_Text pathText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;

    private string pathToSample;

    public override void Initialize(Part associatedPart)
    {
        parameters["PathToSample"] = "";
        parameters["Volume"] = 75f;
        parameters["SampleName"] = "Choose Sample";

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        volumeSlider.value = (float)parameters["Volume"];
        volumeText.text = volumeSlider.value.ToString();
        pathText.text = parameters["SampleName"].ToString();
    }

    public void ChooseSample()
    {
        SimpleFileBrowser.FileBrowser.SetFilters(false, ".wav");
        SimpleFileBrowser.FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, 0, false, null, null, "Select Sample", "Select" );
    }

    public void SetVolume()
    {
        parameters["Volume"] = volumeSlider.value;
        UpdateUI();
        UpdateAssociatedPart();
    }

    public void OnSuccess(string[] paths)
    {
        parameters["PathToSample"] = paths[0];
        UpdateUI();
        UpdateAssociatedPart();
    }

    public void OnCancel()
    {
        return;
    }
}
