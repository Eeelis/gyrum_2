using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneState : IState
{
    public void Enter()
    {
        ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingWireState);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ChoosingPartToPlaceState);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.MakingSelectionState);
        }
        if (Input.GetMouseButtonDown(1))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
        }
    }

    public void Exit()
    {
        
    }
}
