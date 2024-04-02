using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingPartState : IState
{
    private Part partToPlace;
    private PartPlacementHandler partPlacementHandler;

    public PlacingPartState(Part partToPlace, PartPlacementHandler partPlacementHandler)
    {
        this.partToPlace = partToPlace;
        this.partPlacementHandler = partPlacementHandler;
    }

    public void Enter()
    {
        ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
        partPlacementHandler.Initialize(partToPlace);
    }

    public void UpdateState()
    {
        partPlacementHandler.GetInput();
    }

    public void Exit()
    {
        partPlacementHandler.CancelPlacement();
    }
}
