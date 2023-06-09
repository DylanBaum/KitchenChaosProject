using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;



    //Create a method to be called when the the rays from the Player Script have hit it. 
    public override void Interact(Player player)
    {
        //On interact, test if the counter has a kitchen object as a child, also if the player does
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())//If player is carrying something, set it on the table
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //Player has nothing
            }
        }
        else //There is a kitchen object on counter
        {
            if (player.HasKitchenObject())
            {
                //If player already has an object, it wont pick it up
            }
            else//if it doesnt, it will pick it up
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }
}
