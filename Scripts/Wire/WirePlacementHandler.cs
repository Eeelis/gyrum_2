using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WirePlacementHandler : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [SerializeField] private Wire wirePrefab;
    [SerializeField] private Wire wirePreview;
    [SerializeField] private GameObject wireConnectionPoint;

    private Part firstPart;
    private Part secondPart;
    private Part sender;
    private Part receiver;

    private GameObject firstPartConnectionPoint;
    private GameObject secondPartConnectionPoint;
    
    private Vector2 firstClickPos;
    private Vector2 previewStartPos;
    private Vector2 previewEndPos;

    private void Start()
    {
        wirePreview.gameObject.SetActive(false);
        wirePreview.Initialize();
    }

    public void UpdateWirePlacement()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ConnectionManager.Instance.HideWires();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingPartState);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ConnectionManager.Instance.HideWires();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ChoosingPartToPlaceState);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.MakingSelectionState);
        }
        if (Input.GetMouseButtonDown(1))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
            CancelWirePlacement();
        }

        if (Input.GetMouseButtonDown(0))
        {
            firstPart = FindSenderOrReceiver();
            firstClickPos = Utilities.GetMousePositionInWorldSpace();

            if (firstPart)
            {
                if (firstPart is Platform)
                {
                    firstPartConnectionPoint = Instantiate(wireConnectionPoint, Utilities.GetMousePositionInWorldSpace(), Quaternion.identity);
                    firstPartConnectionPoint.transform.SetParent(firstPart.transform);
                }
                else 
                {
                    firstPartConnectionPoint = Instantiate(wireConnectionPoint, firstPart.transform.position, Quaternion.identity);
                    firstPartConnectionPoint.transform.SetParent(firstPart.transform);
                }

                
            }
        }

        if (!firstPart) { return; }

        // Only show the preview if there is a little bit of distance between its start and end points to prevent flickering
        if (Vector2.Distance(firstClickPos, Utilities.GetMousePositionInWorldSpace()) > 0.1f && !wirePreview.gameObject.activeSelf)
        {
            EnableWirePreview();
        }
        else if (Vector2.Distance(firstClickPos, Utilities.GetMousePositionInWorldSpace()) < 0.1f)
        {
            wirePreview.gameObject.SetActive(false);
        }

        UpdateWirePreview();

        CameraManager.Instance.PushCameraWithMouse();

        if (Input.GetMouseButtonUp(0))
        {
            secondPart = FindSenderOrReceiver();

            if (firstPart && secondPart)
            {
                wirePreview.gameObject.SetActive(false);
                ConnectParts();
            }
            else
            {
                CancelWirePlacement();
            }
        }
    }

    public void Reset()
    {
        sender = null;
        receiver = null;
        firstPart = null;
        secondPart = null;
        firstPartConnectionPoint = null;
        secondPartConnectionPoint = null;
    }

    public void CancelWirePlacement()
    {
        wirePreview.gameObject.SetActive(false);

        Reset();
    }

    public void ConnectParts()
    {
        OrderSenderAndReceiver();

        if (!sender || !receiver || sender.receivers.Contains(receiver)|| receiver.senders.Contains(sender))
        {
            Reset();
            return;
        }

        if (firstPart.settings.animateWireConnetion)
        {
            LeanTween.scale(firstPart.gameObject, firstPart.transform.localScale * 1.23f, 0.5f).setEaseInBack().setEasePunch();
        }
        if (secondPart.settings.animateWireConnetion)
        {
            LeanTween.scale(secondPart.gameObject, firstPart.transform.localScale * 1.2f, 0.5f).setEaseInBack().setEasePunch();
        }

        Wire newWire = InstantiateWire();

        ConnectionManager.Instance.ConnectParts(sender, receiver, newWire);

        sender = null;
        receiver = null;
        firstPart = null;
        secondPart = null;
    }

    private void EnableWirePreview()
    {
        previewStartPos = firstPartConnectionPoint.transform.position;
        
        previewEndPos = Utilities.GetMousePositionInWorldSpace();

        wirePreview.StartPos = previewStartPos;
        wirePreview.EndPos = previewEndPos;

        wirePreview.PreCalculatePositionsAndSetActive();
    }

    private void UpdateWirePreview()
    {
        previewEndPos = Utilities.GetMousePositionInWorldSpace();

        wirePreview.EndPos = previewEndPos;
        wirePreview.StartPos = firstPartConnectionPoint.transform.position;
    }

    private Wire InstantiateWire()
    {
        Wire newWire = Instantiate(wirePrefab, sender.transform);

        if (secondPart is Platform)
        {
            secondPartConnectionPoint = Instantiate(wireConnectionPoint, Utilities.GetMousePositionInWorldSpace(), Quaternion.identity);
            secondPartConnectionPoint.transform.SetParent(secondPart.transform);
        }
        else 
        {
            secondPartConnectionPoint = Instantiate(wireConnectionPoint, secondPart.transform.position, Quaternion.identity);
            secondPart.transform.SetParent(secondPart.transform);
        }

        newWire.startPointConnectionPoint = firstPartConnectionPoint;
        newWire.endPointConnectionPoint = secondPartConnectionPoint;

        Vector3[] positions = wirePreview.GetPositions();
        positions[positions.Length - 1] = previewEndPos;
        positions[0] = previewStartPos;

        newWire.SetPositions(positions);
        newWire.PreCalculatePositionsAndSetActive();

        return newWire;
    }

    private Part FindSenderOrReceiver()
    {
        Part partUnderPointer = Utilities.GetPartUnderPointer();

        if (partUnderPointer != null && partUnderPointer != firstPart && (
            partUnderPointer.settings.canSendTriggers || partUnderPointer.settings.canReceiveTriggers ||
            partUnderPointer.settings.canSendValues || partUnderPointer.settings.canReceiveValues))
        {
            return partUnderPointer;
        }

        return null;
    }

    private void OrderSenderAndReceiver()
    {
        if ((firstPart.settings.canSendTriggers && secondPart.settings.canReceiveTriggers) || (firstPart.settings.canSendValues && secondPart.settings.canReceiveValues))
        {
            sender = firstPart;
            receiver = secondPart;
        }
        else if ((firstPart.settings.canReceiveTriggers && secondPart.settings.canSendTriggers) || (firstPart.settings.canReceiveValues && secondPart.settings.canSendValues))
        {
            sender = secondPart;
            receiver = firstPart;
        }
        else
        {
            sender = null;
            receiver = null;
        }
    }
}
