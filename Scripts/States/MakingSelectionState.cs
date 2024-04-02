using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingSelectionState : IState
{
    private SelectionHandler selectionHandler;

    public MakingSelectionState(SelectionHandler selectionHandler)
    {
        this.selectionHandler = selectionHandler;
    }

    public void Enter()
    {

    }

    public void UpdateState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionHandler.InitializeBoxSelection(true);
        }
        if (Input.GetMouseButton(0))
        {
            selectionHandler.DrawBoxSelection();
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectionHandler.EndBoxSelection();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ProgramManager.Instance.ChangeStateToPreviousState();
        }
        if (Input.GetMouseButtonDown(1))
        {
            selectionHandler.EndBoxSelection();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
        }
    }

    public void Exit()
    {
        selectionHandler.EndBoxSelection();
    }
}
