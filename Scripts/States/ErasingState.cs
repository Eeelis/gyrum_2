using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingState : IState
{
    private Vector2 prevPos;
    private SelectionHandler selectionHandler;
    private Vector2 startingPos;

    public ErasingState(SelectionHandler selectionHandler)
    {
        this.selectionHandler = selectionHandler;
    }

    public void Enter()
    {
        ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Erasing);
        selectionHandler.DeselectAllParts();
        prevPos = Utilities.GetMousePositionInWorldSpace();

        Part targetPart = Utilities.GetPartUnderPointer();

        if (targetPart)
        {
            ErasePart(targetPart);
        }
    }

    public void UpdateState()
    {
        // When RMB is first pressed, erase the topmost part under the mouse
        if (Input.GetMouseButtonDown(1))
        {
            prevPos = Utilities.GetMousePositionInWorldSpace();

            Part targetPart = Utilities.GetPartUnderPointer();

            if (targetPart)
            {
                ErasePart(targetPart);
            }
        }

        // If the mouse is not dragged, we only need to look for parts that can move into the mouse in order to erase them
        if (Input.GetMouseButton(1))
        {
            if (Time.frameCount % 5 == 0)
            {
                prevPos = Utilities.GetMousePositionInWorldSpace();

                foreach(Part part in Utilities.GetPartsBetweenPoints(prevPos, Utilities.GetMousePositionInWorldSpace()))
                {
                    if (part.cachedRigidbody && part.cachedRigidbody.velocity != Vector2.zero)
                    {
                        ErasePart(part);
                    }
                }
            }

            // If the mouse is dragged, look for parts between the current mouse position, and the mouse position 5 frames ago.
            // This ensures that parts are not skipped even if the mouse moves really fast
            if (Vector2.Distance(prevPos, Utilities.GetMousePositionInWorldSpace()) > 0.05f)
            {
                foreach(Part part in Utilities.GetPartsBetweenPoints(prevPos, Utilities.GetMousePositionInWorldSpace()))
                {
                    ErasePart(part);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            ProgramManager.Instance.ChangeStateToPreviousState();
        }

    }

    public void Exit()
    {
        ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
    }

    private void ErasePart(Part targetPart)
    {
        targetPart.Erase();
/*         if (targetPart is Rotator)
        {
            CommandInvoker.Instance.AddCommand(new EraseRotatorCommand(targetPart));
            CommandInvoker.Instance.ExecuteCommand();
        }
        else 
        {
            CommandInvoker.Instance.AddCommand(new ErasePartCommand(targetPart));
            CommandInvoker.Instance.ExecuteCommand(); 
        } */
    }
}
