using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Equipping : MonoBehaviour
{
    public static Equipping InstanceEquipping;
    public Transform equipContent;
    public GameObject equipItem;

    private void Awake()
    {
        InstanceEquipping = this;
    }

    
    public void ListItems()
    {
        foreach(Transform item in equipContent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < InventoryManager.Instance.indexEquipment.Count; i++)
        {
            Item currentEquipment = InventoryManager.Instance.Items[i];
            GameObject obj = Instantiate(equipItem, equipContent);
            
            //this is where you get the name and icon and assign it to the stuff in the slot
            //var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("Image").GetComponent<Image>();
            
            //itemName.text = currentItem.ItemName + "(" +item.numItems+ ")";
            itemIcon.sprite = currentEquipment.icon;
        }
    }
}