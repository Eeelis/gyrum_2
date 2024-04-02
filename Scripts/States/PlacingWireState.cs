using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingWireState : IState
{
    private WirePlacementHandler wirePlacementHandler;

    public PlacingWireState(WirePlacementHandler wirePlacementHandler)
    {
        this.wirePlacementHandler = wirePlacementHandler;
    }

    public void Enter()
    {
        ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
        ConnectionManager.Instance.ShowWires();
    }

    public void UpdateState()
    {
        wirePlacementHandler.UpdateWirePlacement();
    }

    public void Exit()
    {
        wirePlacementHandler.CancelWirePlacement();
    }
}
