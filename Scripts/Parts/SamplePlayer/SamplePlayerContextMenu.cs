using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SamplePlayerContextMenu : ContextMenu
{
    private string pathToSample;
    [SerializeField] private TMP_Text pathText;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("PathToSample", pathToSample);
        currentContextMenuData.AddParameter("InitialDirection", Vector3.down);

        this.associatedPart =  associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {

    }

    public void ChooseSample()
    {
        SimpleFileBrowser.FileBrowser.SetFilters(false, ".wav");
        SimpleFileBrowser.FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, 0, false, null, null, "Select Sample", "Select" );
    }

    public void OnSuccess(string[] paths)
    {
        pathToSample = paths[0];
        currentContextMenuData.AddParameter("PathToSample", pathToSample);
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
        pathText.text = pathToSample;
    }

    public void OnCancel()
    {
        return;
    }
}
