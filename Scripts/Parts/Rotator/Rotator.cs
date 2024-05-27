using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : Part
{
    public Part connectedPart;

    private float targetRotation;
    private int rotationDirection = 1;
    private bool rotateContinuously = false;
    private float rotationSpeed = 2f;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            if (contextMenu.valueMode == 0)
            {
                int newSpeed = Mathf.Clamp(value.GetValueOrDefault(), 0, 10);
                rotationSpeed = newSpeed * 30;
                contextMenu.UpdateContextMenu("RotationSpeed", newSpeed);
            }
            else if (contextMenu.valueMode == 1)
            {
                int newRotation = Mathf.Clamp(value.GetValueOrDefault(), 0, 360);
                targetRotation = newRotation;
                contextMenu.UpdateContextMenu("TargetRotation", newRotation);
            }
        }
        else 
        {
            if (contextMenu.triggerMode == 0)
            {
                SetActive();
            }
            else if (contextMenu.triggerMode == 1)
            {
                rotationDirection *= -1;
                contextMenu.UpdateContextMenu("Direction", rotationDirection);
            }
            else if (contextMenu.triggerMode == 2)
            {
                rotateContinuously = !rotateContinuously;
                contextMenu.UpdateContextMenu("ContinousRotation", rotateContinuously);
            }
            
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        targetRotation = (int)parameters["TargetRotation"];
        rotateContinuously = (bool)parameters["ContinousRotation"];
        rotationSpeed = (int)parameters["RotationSpeed"] * 30;
        rotationDirection = (int)parameters["Direction"];
    }

    private void FixedUpdate()
    {
        if (!isActive || !cachedRigidbody) { return; }
        
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

    public void DisconnectPart()
    {
        transform.DetachChildren();

        connectedPart.connectedRotator = null;
        connectedPart?.Erase();
        connectedPart = null;
    }   

    public override void Erase()
    {
        
        DisconnectPart();

        base.Erase();
    }

}
