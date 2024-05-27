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
        parameters["InitialVelocity"] = 0;
        parameters["InitialDirection"] = Vector3.down;

        base.Initialize(associatedPart);
    }

    public override void UpdateUI()
    {
        initialVelocitySlider.value = (int)parameters["InitialVelocity"];
        initialVelocityText.text = initialVelocitySlider.value.ToString();
        UpdateDirectionButtonSprites();
    }

    public void SetInitialVelocity()
    {
        parameters["InitialVelocity"]  = (int)initialVelocitySlider.value;
        UpdateUI();
        UpdateAssociatedPart();
    }

    public void SetDirectionUp()
    {
        parameters["InitialDirection"] = Vector3.up;
        UpdateDirectionButtonSprites();
        UpdateAssociatedPart();
    }

    public void SetDirectionDown()
    {
        parameters["InitialDirection"] = Vector3.down;
        UpdateDirectionButtonSprites();
        UpdateAssociatedPart();
    }

    public void SetDirectionLeft()
    {
        parameters["InitialDirection"] = Vector3.left;
        UpdateDirectionButtonSprites();
        UpdateAssociatedPart();
    }

    public void SetDirectionRight()
    {
        parameters["InitialDirection"] = Vector3.right;
        UpdateDirectionButtonSprites();
        UpdateAssociatedPart();
    }

    public void UpdateDirectionButtonSprites()
    {
        Vector3 direction = (Vector3)parameters["InitialDirection"];

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