using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, ItemContainerInterface
{
    //for the inventory management 
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public List<GameObject> inventorySlotPopulated = new List<GameObject>();
    public Transform ItemContent;
    public GameObject InventoryItem;
    public List<int> indexEquipment = new List<int>();

    //List of all items
    [SerializeField] private List<ItemBase> AllItemBases = new List<ItemBase>();
    public InventoryItemController[] InventoryItems;

    //for managing the selected items -> suggestion: add a green check mark button for adding it to the selected list, and a red x for remove
    public List<Item> selectedItemsForRecipe = new List<Item>();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(Item item) {
        //add item to inventory
        //item stacking, if in inventory already then stack the items aka increase counter by 1 
        int index = IndexOf(item);
        
        if (index != -1)
        {
            Items[index].numItems+= item.numItems; 
        }
        else {
            Items.Add(item);
            if (item.ItemType == ItemBase.ItemTypes.WEAPON || item.ItemType == ItemBase.ItemTypes.ARMOR)
            {
                indexEquipment.Add(Items.Count - 1);
            }
            //Unneccessary code; items by default start with an item count of one, and when they don't, there is a reason (like loading items from save data)
            //int indexAgain = Items.IndexOf(item);
            //Items[indexAgain].numItems = 1;
        }   
    
    }

    //only removes 1 at a time. 
    public void RemoveItem(Item item)
    {
        //remove an item from inventory
        int index = IndexOf(item);

        //if already in inventory and more than 1 then remove the item 
        if (index != -1 && Items[index].numItems > 1)
        {
            Items[index].numItems--;
        }
        //remove already ensures that the item is in inventory, can have this as remove if there plus only 1 item. 
        else
        {
            Items.Remove(item);
            InventoryItems[index].RemoveItem();

        }
    }

    private int IndexOf(Item item)
    {
        for (int i = 0; i < Items.Count; i++) 
        {
                if (item.Equals(Items[i])) return i;
        }
            return -1;
    }

    public void ListItems()
    {

       foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
       }

        //lists the items in the inventory
        foreach (var item in Items)
        {
            //populate the inventory slots with name and image 
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("Image").GetComponent<Image>();  

            //Debug.Log(obj.transform.Find("ItemName"));
            itemName.text = item.ItemName + "(" +item.numItems+ ")";

            itemIcon.sprite = item.icon;
            //save the inventory slot so can be interaccted with 
            inventorySlotPopulated.Add(obj);

        }
        SetInventoryItems();

    }

    public void resetInventory()
    {
        Items.Clear();

    }

    //create inventoryItemControllers for every item in the inventory for all the extra stuff. 
    public void SetInventoryItems() {

        InventoryItems = ItemContent.GetComponentsInChildren < InventoryItemController >();

        for (int i = 0; i < Items.Count; i++) {

            InventoryItems[i].AddItemInventoryController(Items[i]);

        }
    
    }

    //check if the inventory has the item 
    public bool containsItem(Item item) {
        int index = Items.IndexOf(item);

        return (index != -1);
    }

    //check if we have enough of the item in the inventory 
    public int itemCount(Item item)
    {
        int index = Items.IndexOf(item);

        return Items[index].numItems;

    }

    //note: below is for selecting the item for crafting and adding it to a separate list. Stacking is not implemented for this list. 
    //add the item to the selected item for crafting list 
    public void selectItemForCraftingAddsOnlyOne(Item item) {
        var tempItem = item;
        tempItem.numItems = 1;
        selectedItemsForRecipe.Add(tempItem);

    }

    //removes the item from the selected crafting list 
    public void removeItemForCrafting(Item item) {

        selectedItemsForRecipe.Remove(item);

    }

    //go through the list of selected items, and check to see if enough of the requested item has been selected
    // if enough has been selected -> true
    //else -> false 
    public bool findIfHaveEnough(Item itemNeeded, int amountNeeded) {

        int count = 0; 
        foreach (Item item in selectedItemsForRecipe) {

            if (itemNeeded.ItemName.Equals(item.ItemName)) {

                count++;
            
            }
        }
        return count == amountNeeded;
    
    }

    //find and remove the selected items from the list of selected items upon successful crafting
    //returns item so can call the RemoveItem function in the InventoryItemController.
    //assign an InventoryItemController to every item that is made 
   public Item findSpecificItemForRemovalCrafting(Item item) { 
    
        //remove from the selected list 
        int index = selectedItemsForRecipe.IndexOf(item);

        Item tempOneToRemove = selectedItemsForRecipe[index];

        removeItemForCrafting(tempOneToRemove);

        return tempOneToRemove;

    }

    public void LoadItems(SaveData data)
    {
        resetInventory();
        for (int i = 0; i < data.itemNames.Length; i++)
        {
            Item temp = null;
            foreach (ItemBase itemBase in AllItemBases)
            {
                if (data.itemNames[i] == itemBase.ItemName)
                {
                    temp = new Item(itemBase, (Item.WeaponEnchantment)data.weaponEnchantments[i], (Item.ArmorEnchantment)data.armorEnchantments[i], data.numItems[i]);
                    break;
                }
            }
            if (temp != null) Items.Add(temp);
        }
    }

}

