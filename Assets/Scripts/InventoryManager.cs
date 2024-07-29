using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, ItemContainerInterface
{

    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    public List<GameObject> inventorySlotPopulated = new List<GameObject>();
   

    public Transform ItemContent;
    public GameObject InventoryItem;

    //  public List<GameObject> inventorySlotPopulated = new List<GameObject>();

    public InventoryItemController[] InventoryItems;
    private void Awake()
    {
        Instance = this;
    }

    public void Add(Item item) {
        //add item to inventory
        //item stacking, if in inventory already then stack the items aka increase counter by 1 
        int index = Items.IndexOf(item);
        
        if (index != -1)
        {
            Items[index].numItems++; 
        }
        else {
            Items.Add(item);
            int indexAgain = Items.IndexOf(item);
            Items[indexAgain].numItems++;

        }   
    
    }

    public void RemoveItem(Item item)
    {
        //remove an item from inventory
        int index = Items.IndexOf(item);

        //if already in inventory and more than 1 then remove the item 
        if (index != -1 && Items[index].numItems > 1)
        {
            Items[index].numItems--;
        }
        //remove already ensures that the item is in inventory, can have this as remove if there plus only 1 item. 
        else
        {
            Items.Remove(item);
        }
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
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            //check this 
            var itemIcon = obj.transform.Find("Image").GetComponent<Image>();

            Debug.Log(obj.transform.Find("ItemName"));
            itemName.text = item.ItemName;
            //check this 
            itemIcon.sprite = item.icon;
            inventorySlotPopulated.Add(obj);

            // inventorySlotPopulated.Add(obj);
        }
        SetInventoryItems();

    }

    // public void resetInventory()
    //{
    //   foreach (var item in inventorySlotPopulated)
    // {
    //   Destroy(item);
    //}

    //}


    // Start is called before the first frame update
    //  void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void SetInventoryItems() {

        InventoryItems = ItemContent.GetComponentsInChildren < InventoryItemController >();

        for (int i = 0; i < Items.Count; i++) {

            InventoryItems[i].AddItemInventoryController(Items[i]);


        }
    
    }

    public bool containsItem(Item item) {
        int index = Items.IndexOf(item);

        return (index != -1);
    }

    public int itemCount(Item item)
    {
        int index = Items.IndexOf(item);

        return Items[index].numItems;

    }
}
