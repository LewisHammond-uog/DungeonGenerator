using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Activate a button and wait for it to be pressed
/// Deactivate the button after press
/// </summary>
public class ActivateButtonAndWaitForPress : IEnumerator
{
    private readonly Button button;
    private bool buttonPressed = false;

    public ActivateButtonAndWaitForPress(Button button)
    {
        this.button = button;
        button.interactable = true;
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        buttonPressed = true;   
    }

    public bool MoveNext()
    {
        //Reset button interactable state if button was pressed
        if (buttonPressed)
        {
            button.interactable = false;
        }
        return !buttonPressed;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public object Current { get; }
}
