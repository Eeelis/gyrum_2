using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionMatrixElement : MonoBehaviour
{   
    private Part associatedPart;
    private Part sender;
    private Part receiver;
    [SerializeField] private TMP_Text partNameText;

    public void Initialize(Part sender, Part receiver, Part associatedPart)
    { 
        this.sender = sender;
        this.receiver = receiver;
        this.associatedPart = associatedPart;

        SetPartNameText();
    }

    public void SetPartNameText()
    {
        partNameText.text = associatedPart.partName;
    }

    public void RemoveConnection()
    {
        ConnectionManager.Instance.RemoveConnection(sender, receiver);
        //CommandInvoker.Instance.AddCommand(new DisconnectPartsCommand(sender, receiver));
        //CommandInvoker.Instance.ExecuteCommand();
    }

    public void ShowPart()
    {
        if (associatedPart is Platform)
        {
            Vector2 targetPosition = (associatedPart.GetComponent<LineRenderer>().GetPosition(0) + associatedPart.GetComponent<LineRenderer>().GetPosition(1)) / 2;
            CameraManager.Instance.SetCameraPosition(targetPosition);
        }
        else 
        {
            CameraManager.Instance.SetCameraPosition(associatedPart.transform.position);
        }
        
    }


}
