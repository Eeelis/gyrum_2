using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Part
{
    private int initialVelocity;
    private Vector3 initialDirection = Vector3.down;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int direction = value.GetValueOrDefault();

            switch(direction)
            {
                case(1):
                {
                    contextMenu.UpdateContextMenu("InitialDirection", Vector3.left);
                    initialDirection = Vector3.left;
                    break;
                }
                case(2):
                {
                    contextMenu.UpdateContextMenu("InitialDirection", Vector3.right);
                    initialDirection = Vector3.right;
                    break;
                }
                case(3):
                {
                    contextMenu.UpdateContextMenu("InitialDirection", Vector3.up);
                    initialDirection = Vector3.up;
                    break;
                }
                case(4):
                {
                    contextMenu.UpdateContextMenu("InitialDirection", Vector3.down);
                    initialDirection = Vector3.down;
                    break;
                }
                default:
                {
                    contextMenu.UpdateContextMenu("InitialDirection", Vector3.down);
                    initialDirection = Vector3.down;
                    break;
                }
            }

            return;
        }

        SpawnMarble();
    }

    private void SpawnMarble()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.one;
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.25f).setEasePunch();

        Marble newMarble = (Marble)PartPoolManager.Instance.GetPart("Marble", transform.position);
        
        newMarble.cachedRigidbody.velocity = transform.TransformDirection(initialDirection) * (initialVelocity);
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        initialVelocity = contextMenuData.GetParameter<int>("InitialVelocity") * 5;
        initialDirection = contextMenuData.GetParameter<Vector3>("InitialDirection");
    }
}
