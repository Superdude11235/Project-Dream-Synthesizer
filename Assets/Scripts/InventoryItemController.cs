using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemController : MonoBehaviour
{

    Item item;

    public void RemoveItem() {

        InventoryManager.Instance.RemoveItem(item);

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
