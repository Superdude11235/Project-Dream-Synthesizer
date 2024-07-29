using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount {

    public Item Item;
    [Range(1,999)]
    public int Amount;

}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{

    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool canCraft(ItemContainerInterface itemContainer) {

        //loop through the list of materials 
        foreach (ItemAmount itemAmount in Materials) {
            //check if we have enough of each item required for the crafting recipe in the inventory 
            if (itemContainer.itemCount(itemAmount.Item) < itemAmount.Amount) {
                //no we do not have enough to craft at least one 
                return false;
            
            }
        }
        // yes we have enough to craft at least 1 
        return true;
    
    }

    public void craft(ItemContainerInterface itemContainer)
    {
        if (canCraft(itemContainer)) {
            //removed the used items from the inventory
            foreach (ItemAmount itemAmount in Materials)
            {

                for (int i = 0; i < itemAmount.Amount; i++) {

                    itemContainer.RemoveItem(itemAmount.Item);


                }
            
            
            }

//add the items we made to the inventory :) 
            foreach (ItemAmount itemAmount in Results)
            {

                for (int i = 0; i < itemAmount.Amount; i++)
                {

                    itemContainer.Add(itemAmount.Item);


                }


            }
 


        }
    }




    }
