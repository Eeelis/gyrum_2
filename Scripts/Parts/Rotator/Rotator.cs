using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : Part
{
    public Part connectedPart;

    private float targetRotation;
    private int rotationDirection = 1;
    private bool isRotating = true;
    private bool rotateContinuously = false;
    private float rotationSpeed = 2f;

    private void FixedUpdate()
    {
        if (!isRotating) { return; }
        if (!cachedRigidbody) { return; }
        
        if (rotateContinuously)
        {
            float newRotation = Mathf.MoveTowards(cachedRigidbody.rotation, cachedRigidbody.rotation + 180, rotationSpeed * rotationDirection * Time.fixedDeltaTime);
            cachedRigidbody.MoveRotation(newRotation);
        }
        else if (cachedRigidbody.rotation != targetRotation)
        {
            float newRotation = Mathf.MoveTowardsAngle(cachedRigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            cachedRigidbody.MoveRotation(newRotation);
        }
    }

    public void ConnectPart(Part part)
    {
        part.connectedRotator = this;
        connectedPart = part;

        part.transform.SetParent(transform, true);
        part.DisableRigidbody();
    }

    public override void Erase()
    {
        transform.DetachChildren();
        
        if (connectedPart)
        {
            connectedPart.connectedRotator = null;
            connectedPart.Erase();
            connectedPart = null;
        }

        base.Erase();
    }

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int newRotation = Mathf.Clamp(value.GetValueOrDefault(), 0, 360);
            targetRotation = newRotation;
            contextMenu.UpdateContextMenu("TargetRotation", newRotation);
        }
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        targetRotation = contextMenuData.GetParameter<int>("TargetRotation");
        rotateContinuously = contextMenuData.GetParameter<bool>("ContinousRotation");
        rotationSpeed = contextMenuData.GetParameter<int>("RotationSpeed") * 30f;
        rotationDirection = contextMenuData.GetParameter<int>("Direction");
    }
}
