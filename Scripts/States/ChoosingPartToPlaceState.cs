using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosingPartToPlaceState : IState
{
    private PartMenuHandler partMenuHandler;

    public ChoosingPartToPlaceState(PartMenuHandler partMenuHandler)
    {
        this.partMenuHandler = partMenuHandler;
    }

    public void Enter()
    {
        partMenuHandler.OpenPartMenu();
    }

    public void UpdateState()
    {
        partMenuHandler.UpdatePartMenu();
        
        if (Input.GetMouseButtonDown(1))
        {
            if (Input.mousePosition.x > Screen.width / 2)
            {
                partMenuHandler.SwitchTab(true);
            }
            else 
            {
                partMenuHandler.SwitchTab(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingPartState);
        }
    }

    public void Exit()
    {
        partMenuHandler.ClosePartMenu();
    }
}
