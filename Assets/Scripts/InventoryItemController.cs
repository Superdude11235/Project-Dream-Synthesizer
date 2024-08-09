using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{

    Item item;
    //public Button removeFromSelectionForCrafting;
   // public Button addToSelectionForCrafting;


    public void RemoveItem() {

        InventoryManager.Instance.RemoveItem(item);
        Destroy(gameObject);

    }

    public void AddItemInventoryController(Item newItem) {

        item = newItem;
    
    }

    // Start is called before the first frame update
  //  void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
