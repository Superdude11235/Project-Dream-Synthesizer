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
    public string recipeName;
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool canCraft(ItemContainerInterface itemContainer) {

        //loop through the list of materials 
        foreach (ItemAmount itemAmount in Materials) {
            //check if we have enough of each item required for the crafting recipe in the list of selected items

            if (itemContainer.findIfHaveEnough(itemAmount.Item,itemAmount.Amount)) {
                //yes we do have enough to craft at least one 
                return true;
            }
        }
        // no
        return false;
    
    }

    public void craft(ItemContainerInterface itemContainer)
    {
        if (canCraft(itemContainer)) {
            //removed the used items from the inventory
            //need to specifically remove the ones from the list that were selected. 
            foreach (ItemAmount itemAmount in Materials)
            {
                for (int i = 0; i < itemAmount.Amount; i++) {

                    //need to remove the ones that were specifically picked due to the enchantment stuff
                    //remove from inventory and from list of items specifically selected 
                    itemContainer.RemoveItem(itemContainer.findSpecificItemForRemovalCrafting(itemAmount.Item));
                 
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
