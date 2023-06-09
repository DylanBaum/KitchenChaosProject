using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    //Player Input Actions was created in the Assets folder and let me create a list of actions acceptable for movement. It was
    //created under the Player Map which is what is enabled in line 18

    //Create a variable for the input actions class
    private PlayerInputActions playerInputActions;

    //Create an event variable
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;


    //Method that is called as the script loads
    private void Awake()
    {
        //on load, get a new player input 
        playerInputActions = new PlayerInputActions();
        //from that, enable the Player set we created 
        playerInputActions.Player.Enable();

        //Call the interact action from the Player action map in PlayerInputActions, and have it throw the interact_performed event
        playerInputActions.Player.Interact.performed += Interact_performed;

        //Create another event to listen for the alternate interact button press
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    //Method created from above, which is firing a signal anytime the player.interact key is pressed
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Fires an event, if its not null(thats what the ? is for, it checks if the left side of it is not null) 
        //I think this is event specific
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        //Get the input vectors, since I just want to move on the x/y, create a new Vector 2
        //and make it empty so I can add values to it from key presses
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        //Normalize speed on the diagonal so it is consistent
        inputVector = inputVector.normalized;

        //Return the normalized vector so we can use it in other scripts (Player)
        return inputVector;
    }



}
