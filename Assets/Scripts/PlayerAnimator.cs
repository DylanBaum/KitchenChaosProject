using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //Create a constant string for the animator parameter
    private const string IS_WALKING = "IsWalking";

    //Set a variable for the animator component
    private Animator animator;

    //Get a reference to the player
    [SerializeField] private Player player;

    private void Awake()
    {
        //Get the animator component
        animator = GetComponent<Animator>();

        
    }

    private void Update()
    {
        //Change the animators bool for walking by getting the name of the parameter and checking the bool variable returned in the 
        //method from the player script.
        animator.SetBool(IS_WALKING, player.IsWalking());
    }

}
