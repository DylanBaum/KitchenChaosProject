using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;


    public override void Interact(Player player)
    {
        //On interact, test if the counter has a kitchen object as a child, also if the player does
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())//If player is carrying something....
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))//....And it has the kitchen recipie SO script...
                {
                    //...Put it on the table
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                }
                
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


    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            //if it has an object on it and if that object has the recipe SO script, destroy
            //it and spawn the cut SO
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
            
        }
        return false;
    }



    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }

}
