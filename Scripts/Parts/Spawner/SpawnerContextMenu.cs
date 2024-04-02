using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SpawnerContextMenu : ContextMenu
{
    [SerializeField] private Slider initialVelocitySlider;
    [SerializeField] private TMP_Text initialVelocityText;

    [SerializeField] private Sprite unselectedDirectionIndicatorSprite;
    [SerializeField] private Sprite selectedDirectionIndicatorSprite;

    [SerializeField] private Image upButton;
    [SerializeField] private Image downButton;
    [SerializeField] private Image leftButton;
    [SerializeField] private Image rightButton;

    public override void Initialize(Part associatedPart)
    {
        currentContextMenuData.AddParameter("InitialVelocity", 0);
        currentContextMenuData.AddParameter("InitialDirection", Vector3.down);

        initialVelocitySlider.value = currentContextMenuData.GetParameter<int>("InitialVelocity");
        UpdateInitialVelocityText();
        UpdateDirectionButtonSprites();

        this.associatedPart =  associatedPart;
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public override void UpdateContextMenu(string parameterName, object value)
    {
        currentContextMenuData.AddParameter(parameterName, value);

        if (parameterName == "InitialVelocity")
        {
            initialVelocitySlider.value = currentContextMenuData.GetParameter<int>("InitialVelocity");
            UpdateInitialVelocityText();
        }   

        UpdateDirectionButtonSprites();
    }

    public void UpdateInitialVelocityText()
    {
        initialVelocityText.text = initialVelocitySlider.value.ToString();
    }

    public void SetInitialVelocity()
    {
        int initialVelocity = (int)initialVelocitySlider.value;
        currentContextMenuData.AddParameter("InitialVelocity", initialVelocity);

        UpdateInitialVelocityText();
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionUp()
    {
        currentContextMenuData.AddParameter("InitialDirection", Vector3.up);
        UpdateDirectionButtonSprites();
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionDown()
    {
        currentContextMenuData.AddParameter("InitialDirection", Vector3.down);
        UpdateDirectionButtonSprites();
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionLeft()
    {
        currentContextMenuData.AddParameter("InitialDirection", Vector3.left);
        UpdateDirectionButtonSprites();
        SendContextMenuDataToAssociatedPart();
    }

    public void SetDirectionRight()
    {
        currentContextMenuData.AddParameter("InitialDirection", Vector3.right);
        UpdateDirectionButtonSprites();
        SendContextMenuDataToAssociatedPart();
    }

    public void UpdateDirectionButtonSprites()
    {
        Vector3 direction =  currentContextMenuData.GetParameter<Vector3>("InitialDirection");

        if (direction == Vector3.up)
        {
            upButton.GetComponent<Image>().sprite = selectedDirectionIndicatorSprite;
            downButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            leftButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            rightButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
        }
        if (direction == Vector3.down)
        {
            downButton.GetComponent<Image>().sprite = selectedDirectionIndicatorSprite;
            upButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            leftButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            rightButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
        }
        if (direction == Vector3.left)
        {
            leftButton.GetComponent<Image>().sprite = selectedDirectionIndicatorSprite;
            upButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            downButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            rightButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
        }
        if (direction == Vector3.right)
        {
            rightButton.GetComponent<Image>().sprite = selectedDirectionIndicatorSprite;
            upButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            downButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
            leftButton.GetComponent<Image>().sprite = unselectedDirectionIndicatorSprite;
        }
    }
}