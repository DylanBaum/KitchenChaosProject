using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    //Create a property for the player that lets other classes get it but only this one
    //set it. 
    public static Player Instance { get; private set; }



    //Make the event that fires when the selected counter variable has changed
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    //Set a variable for the movement speed
    [SerializeField] private float moveSpeed = 7f;

    //Create a variable for the game input script, which (obviously) handles the inputs recieved and returns those
    //as a normalized Vector 2 (X and Z). It is "Grabbed" from the hierarchy by dragging it over
    [SerializeField] private GameInput gameInput;

    //Create a variable to mask out which layer I want the raycast to hit with down in the HandleInteractions method
    //Assigned in the inspector
    [SerializeField] private LayerMask countersLayerMask;

    //
    [SerializeField] private Transform kitchenObjectHoldPoint;



    //Create a bool variable to check if were walking
    private bool isWalking;

    //Set a variable for the last direction moved
    private Vector3 lastInteractDir;

    //Create a variable to check for the BaseCounter script on an object, this will be the "selected counter" or the counter we are in front of
    private BaseCounter selectedCounter;

    //
    private KitchenObject kitchenObject;

    //
    private void Awake()
    {
        //On Awake, make sure this is only one Player instance, the set it to this Player class.
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player Instance");
        }
        Instance = this;
    }


    private void Start()
    {
        //Create a listener for the interact event created in the gameinput script
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        //If in front of an object that has the BaseCounter script/class passed to it, call the interact function on it
        //this is set up above as a variable
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }


    //Method created from the event listener. When the event from the game input script is called, this is called and runs 
    //the interact code
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        //If in front of an object that has the BaseCounter script/class passed to it, call the interact function on it
        //this is set up above as a variable
        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
        
    }




    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }



    //Create a method to test if player is walking or not
    public bool IsWalking()
    {
        return isWalking;
    }



    //Create a method to handle interactions and call it in the update method
    private void HandleInteractions()
    {
        //Grab the normalized Vector 2 from the game input script and assign it as our input vector
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        //Create a movement Vector 3 from that Vector 2 since I'm working in a 3D space
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        
        //Create a function to check if we are moving, and if we are, save that vector 3 so that we can use it when interacting
        //(We need to save this to use when raycasting for our interactables, else when we stopped moving, it would have
        //no direction to cast in)
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        //Create a variable for the distance we can interact with an object
        float interactDistance = 2f;

        //Create a raycast that checks if it his something. It returns a bool and an
        //object called raycastHit we can use to get information.
        //If the ray hits something...
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            //...do something here...

            //Specifically, check if what the ray hit has a clear counter script component,
            //and name it clearCounter for later reference
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //if it does, update the selectedCounter variable to what it hit
                if (baseCounter != selectedCounter)
                {
                    selectedCounter = baseCounter;

                    //Call the SetSelectedCounter method below, pass in the counter the ray hit, and
                    //fire off the Selected counter event above
                    SetSelectedCounter(baseCounter);


                }
                //If it doesnt have the clear counter script on it, set the variable to null
            }
            else
            {
                //Pass in null to the Method below so that the selectedCounter variable gets updated to show nothing is selected
                SetSelectedCounter(null);
            }
        }
        //If the ray didnt hit anything, set the variable to null
        else
        {
            //Pass in null to the Method below so that the selectedCounter variable gets updated to show nothing is selected
            SetSelectedCounter(null);
        }
    }



    //Create a method to handle movement and call it in the update method
    private void HandleMovement()
    {
        //Grab the normalized Vector 2 from the game input script and assign it as our input vector
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        //Create a movement Vector 3 from that Vector 2 since I'm working in a 3D space
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);


        //Create variables to use for the capsule cast
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2.0f;
        //Check if there is something solid in front of the player
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        //MOVEMENT WHEN UP AGAINST AN OBJECT
        //Gonna check if we can no longer move (ray is hitting something)
        if (!canMove)
        {
            //Attempt to move only in the X. Create new movement direction variable for just the X, and see if a ray hits something
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove =  moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //If the ray doesnt hit anything, then set my move direction variable to the new X specific one
                //and use it to move only in the X(Done in the applied movement comment below)
                moveDir = moveDirX;
            }
            else
            {//If I cant move only in the X
             //Attempt movement only in the Z the same way I did with the X
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //Can move only in the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //Cannot move in any direction
                }

            }

        }


        //APPLIED MOVEMENT
        //if we can move, do it by changing the transform
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }


        //ROTATE THE WAY YOURE LOOKING
        //check if the walking bool is true
        if (isWalking = moveDir != Vector3.zero)
        {
            //if we are walking(if its not zero), rotate by a certain speed around the desired axis (in this case, the "forward")
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
            //LEARN MORE ABOUT SLERP. it helps smooth the transitions
        }

    }


    //Create a method that sets the member variable as the varaible passed as the parameter and fires off the selected counter event
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        //set the selected counter argument in the event to the selected counter variable passed in as the parameter.
        //(IMPORTANT: These are 2 different selectedCounter variables. One is what the player is looking at,
        //the other is an argument for the event.
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }


    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
