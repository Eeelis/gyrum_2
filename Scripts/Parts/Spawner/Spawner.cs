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
            if (contextMenu.valueMode == 0)
            {
                int newVelocity = Mathf.Clamp(value.GetValueOrDefault(), 0, 10);
                initialVelocity = newVelocity;
                contextMenu.UpdateContextMenu("InitialVelocity", initialVelocity);
            }
            else if (contextMenu.valueMode == 1)
            {  
                int newDirection = value.GetValueOrDefault();

                switch(newDirection)
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
                }
            }
        }
        else 
        {
            SpawnMarble();
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        initialVelocity = (int)parameters["InitialVelocity"] * 5;
        initialDirection = (Vector3)parameters["InitialDirection"];
    }

    private void SpawnMarble()
    {
        if (!isActive) { return; }

        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.one;
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.25f).setEasePunch();

        Marble newMarble = (Marble)PartPoolManager.Instance.GetPart("Marble", transform.position);
        
        newMarble.cachedRigidbody.velocity = transform.TransformDirection(initialDirection) * (initialVelocity);
    }

    public override object GetState()
    {
        return new SaveData()
        {
            initialVelocity = this.initialVelocity,
            intialDirectionX = this.initialDirection.x,
            intialDirectionY = this.initialDirection.y,
            isActive = this.isActive
        };
    }

    public override void LoadState(object state)
    {
        var saveData = (SaveData)state;

        initialVelocity = saveData.initialVelocity;
        initialDirection = new Vector3(saveData.intialDirectionX, saveData.intialDirectionY, 0f);
        isActive = saveData.isActive;
        
        contextMenu.UpdateContextMenu("InitialVelocity", initialVelocity);
        contextMenu.UpdateContextMenu("InitialDirection", initialDirection);
    }

    [System.Serializable]
    private struct SaveData
    {
        public int initialVelocity;
        public float intialDirectionX;
        public float intialDirectionY;
        public bool isActive;
    }
}
