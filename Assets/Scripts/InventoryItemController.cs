using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;
    static int counter;
   // Button removeFromSelectedList;
    

    public void RemoveItem() {
        counter--;
        InventoryManager.Instance.RemoveItem(item);
        Destroy(gameObject);

    }

    public void AddItemInventoryController(Item newItem) {
        item = newItem;
        counter++;
    
    }

    //for crafting section
    //add the item to SelectedItem List 
    public void addItemToSelectedListForCrafting()
    {
        InventoryManager.Instance.selectItemForCraftingAddsOnlyOne(item);
    }

    //remove the item from the SelectedItem List for crafting=

    public void removeFromSelectedListForCrafting() {

        var tempItem = item;
        tempItem.numItems = 1;
        InventoryManager.Instance.removeItemForCrafting(tempItem);

    }



}
