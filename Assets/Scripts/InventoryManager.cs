using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor;
//using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, ItemContainerInterface
{
    //for the inventory management 
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public List<GameObject> inventorySlotPopulated = new List<GameObject>();
    public Transform ItemContent;
    public GameObject InventoryItem;
    public GameObject InventoryMenuUI;
    public static bool EquipIsOpen = false;

    //List of all items
    [SerializeField] private List<ItemBase> AllItemBases = new List<ItemBase>();
    int countControllers;
    public InventoryItemController[] InventoryItems;

    //for managing the selected items -> suggestion: add a green check mark button for adding it to the selected list, and a red x for remove
    public List<Item> selectedItemsForRecipe = new List<Item>();

    //For equipment inventory
    public List<Item> EquipmentItems = new List<Item>();
    public delegate void OnEquipChanged();
    public OnEquipChanged onEquipChangedCallback;

    //Inventory Scene
    private const int INVENTORY_SCENE_INDEX = 2;
    private const int MAIN_LEVEL_SCENE_INDEX = 1;

    //adding a few test items
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
       

        //bow recipe, string stick bubble water
        //Item.WeaponEnchantment weaponEnchantment = Item.GetRandomWeaponEnchantment();
        //Item.ArmorEnchantment armorEnchantment = Item.GetRandomArmorEnchantment();

        //ItemBase ItemBase1 = new ItemBase();
        //ItemBase ItemBase2 = new ItemBase();
        //ItemBase ItemBase3 = new ItemBase();
        ////ItemBase ItemBase4 = new ItemBase();
        //ItemBase1.ItemName = "Bubble Water";
        //ItemBase2.ItemName = "Stick";
        //ItemBase3.ItemName = "String";
        ////ItemBase4.ItemName = "String";
        //ItemBase1.ItemType = ItemBase.ItemTypes.OTHER;
        //ItemBase2.ItemType = ItemBase.ItemTypes.OTHER;
        //ItemBase3.ItemType = ItemBase.ItemTypes.OTHER;
        //// ItemBase4.ItemType = ItemBase.ItemTypes.OTHER;

        //Item itemTest1 = new Item(ItemBase1, weaponEnchantment, armorEnchantment);
        //Item itemTest2 = new Item(ItemBase2, weaponEnchantment, armorEnchantment);
        //Item itemTest3 = new Item(ItemBase3, weaponEnchantment, armorEnchantment);
        //Item itemTest4 = new Item(ItemBase3, weaponEnchantment, armorEnchantment);



        //Items.Add(itemTest1);
        //Items.Add(itemTest2);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);
        //Items.Add(itemTest3);


    }

    

    //private void AssignInventoryVariables(Scene scene, LoadSceneMode mode)
    //{
    //    InventoryMenuUI = GameObject.Find("InventorySystem/Inventory");
    //    ItemContent = GameObject.Find("InventorySystem/Inventory/Scroll View/Viewport/Content").transform;
    //}


    void Update() {
        if (Input.GetKeyDown(KeyCode.I) && SceneManager.GetActiveScene().buildIndex != INVENTORY_SCENE_INDEX)
        {
            if (EquipIsOpen)
            {
                InventoryMenuUI.SetActive(false);
                EquipIsOpen = false;
            }
            else
            {
                InventoryMenuUI.SetActive(true);
                ListItems(false);
                EquipIsOpen = true;
            }
        }

        if ((Input.GetKeyDown(KeyCode.C) && SceneManager.GetActiveScene().buildIndex == INVENTORY_SCENE_INDEX)){
            SaveSystem.SaveInventory(this);
            SceneManager.LoadSceneAsync(MAIN_LEVEL_SCENE_INDEX);
        }

    }


    private void OnEnable()
    {
        Events.Loadprogress += LoadItems;
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

            if (item.ItemName == "Cloud Boots") Events.GetCloudBoots();
            if (item.ItemType != ItemBase.ItemTypes.OTHER)
            {
                EquipmentItems.Add(item);
                
                if (onEquipChangedCallback != null)
                {
                    onEquipChangedCallback.Invoke();
                }
            }
        }
    }

    //only removes 1 at a time. 
    public void RemoveItem(Item item)
    {
        //remove an item from inventory
        int index = IndexOf(item);

        //check if item is equipment
        bool itemIsEquip = false;
        foreach (var i in EquipmentItems)
        {
            if (item.Equals(i))
            {
                itemIsEquip = true;
            }
        }

        //if already in inventory and more than 1 then remove the item 
        if (index != -1 && Items[index].numItems > 1)
        {
            Items[index].numItems--;
        }
        //remove already ensures that the item is in inventory, can have this as remove if there plus only 1 item. 
        else
        {
            Items.Remove(item);
            if (itemIsEquip)
            {
                EquipmentItems.Remove(item);
            }

            //need to remove the associated InventoryItemController 
            // need to remove it from the Transform


        }

        if (itemIsEquip && onEquipChangedCallback != null)
        {
            onEquipChangedCallback.Invoke();
        }
    }

    private void OnDisable()
    {
        Events.Loadprogress -= LoadItems;

    }

    public int IndexOf(Item item)
    {
        for (int i = 0; i < Items.Count; i++) 
        {
                if (item.Equals(Items[i])) return i;
        }
            return -1;
    }

    //this is for finding the item in the selected items list
    //had to create a seperate one since using ItemBase for the crafting recipe
    private int IndexOfForSelection(string itemName)
    {
        for (int i = 0; i < selectedItemsForRecipe.Count; i++)
        {
            if (itemName.Equals(selectedItemsForRecipe[i].ItemName)) return i;
        }
        return -1;
    }


    private int IndexOfBase(Item item) {

        for (int i = 0; i < AllItemBases.Count; i++) {

            if (item.ItemName.Equals(AllItemBases[i].ItemName)) return i;
        
        }
        return -1;
    
    }


    public void ListItems(bool flagForButtons)
    {
       // inventorySlotPopulated.Clear();

        if (InventoryItems.Length != 0) {
            Array.Clear(InventoryItems, 0, countControllers);
        }
        Debug.Log("Child Coutn for the Transform Before Load" + ItemContent.childCount);
        foreach (Transform child in ItemContent) {

            Destroy(child.gameObject);
           
        }

        while (ItemContent.childCount != 0) {

            Instance.ItemContent.GetChild(0).transform.parent = null;
        }

        Debug.Log("Child Coutn for the Transform After Load" + ItemContent.childCount);

        //lists the items in the inventory
        foreach (var item in Items)
        {           

            //populate the inventory slots with name and image 
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("Image").GetComponent<Image>();  
            //get the buttons for adding and removing the items from selected list 
            var removeFromSelectionButton = obj.transform.Find("RemoveFromSelectionButton").GetComponent<Button>();
            var addToSelectionButton = obj.transform.Find("AddToSelectionButton").GetComponent<Button>();

            itemName.text = item.ItemName + "(" +item.numItems+ ")";
            itemIcon.sprite = item.icon;

            //if flag = true set them to active
            if (flagForButtons)
            {
                removeFromSelectionButton.gameObject.SetActive(true);
                addToSelectionButton.gameObject.SetActive(true);

            }


            //else set them to inactive 

            else {
                removeFromSelectionButton.gameObject.SetActive(false);
                addToSelectionButton.gameObject.SetActive(false);
            }

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

        countControllers = 0;
        InventoryItems = ItemContent.GetComponentsInChildren< InventoryItemController >();
        for (int i = 0; i < Items.Count; i++) {
            InventoryItems[i].AddItemInventoryController(Items[i]);
            countControllers++;
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
        Item tempItem = item;
        tempItem.numItems = 1;
        selectedItemsForRecipe.Add(item);

    }

    //removes the item from the selected crafting list 
    public void removeItemForCrafting(Item item) {

        selectedItemsForRecipe.Remove(item);

    }

    //go through the list of selected items, and check to see if enough of the requested item has been selected
    // if enough has been selected -> true
    //else -> false 
    public bool findIfHaveEnough(string itemNeeded, int amountNeeded) {

        int count = 0; 
        foreach (Item item in selectedItemsForRecipe) {

            if (itemNeeded.Equals(item.ItemName)) {

                count++;
            
            }
        }
        return count == amountNeeded;
    
    }

    //find and remove the selected items from the list of selected items upon successful crafting
    //returns item so can call the RemoveItem function in the InventoryItemController.
    //assign an InventoryItemController to every item that is made 
   public Item findSpecificItemForRemovalCrafting(string itemName) { 
    
        //remove from the selected list 

        int index = IndexOfForSelection(itemName);

        Item tempOneToRemove = selectedItemsForRecipe[index];

        removeItemForCrafting(tempOneToRemove);

        return tempOneToRemove;

    }

    public void LoadItems()
    {
        SaveData data = SaveSystem.LoadInventoryData();

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
            if (temp != null) Add(temp);
        }
    }

}

