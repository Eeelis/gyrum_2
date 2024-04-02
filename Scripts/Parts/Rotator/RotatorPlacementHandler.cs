using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorPlacementHandler : MonoBehaviour
{
    [SerializeField] private float snappingRange;
    [SerializeField] private GameObject preview;

    private Part targetPart;
    private Vector2 targetPosition;

    public void InitializeRotatorPlacement()
    {
        preview.transform.position = Utilities.GetMousePositionInWorldSpace();
        preview.SetActive(true);
    }

    public void UpdateRotatorPlacement()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StopPlacing();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ChoosingPartToPlaceState);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StopPlacing();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.MakingSelectionState);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StopPlacing();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingWireState);
        }
        if (Input.GetMouseButtonDown(1))
        {
			StopPlacing();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
        }

        targetPart = FindPartToConnectTo();

        if (targetPart == null || targetPart.connectedRotator)
        { 
            preview.transform.position = Utilities.GetMousePositionInWorldSpace();
            return;
        }

        if (targetPart is Platform)
        {
            preview.transform.position = Utilities.GetClosestPointOnLineRenderer(targetPart.GetComponent<LineRenderer>(), Utilities.GetMousePositionInWorldSpace());
        }
        else 
        {
            preview.transform.position = targetPart.transform.position;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Rotator placedRotator = (Rotator)PartPoolManager.Instance.GetPart("Rotator", preview.transform.position);
            placedRotator.Initialize();
            placedRotator.ConnectPart(targetPart);
        }
    }

    private Part FindPartToConnectTo()
    {
        List<Part> potentialParts = new List<Part>();
        
        foreach(Part part in Utilities.GetPartsInRange(snappingRange))
        {
            if (part.settings.canBeConnectedToRotators)
            {
                potentialParts.Add(part);
            }
        }

        return Utilities.FindClosestToMouse(potentialParts);
    }

    private void StopPlacing()
    {
        preview.SetActive(false);
    }
}
