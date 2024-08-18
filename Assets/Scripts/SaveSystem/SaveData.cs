using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;
//using static UnityEditor.Progress;

[System.Serializable]
public class SaveData
{
    public float[] position;
    public string[] itemNames;
    public int[] weaponEnchantments;
    public int[] armorEnchantments;
    public int[] numItems;
    public Item weaponItem;
    public Item armorItem;
    public bool has_cloud_boots;


    //Saves player position and inventory contents
    public SaveData(Player player, InventoryManager inventory)
    {
        position = Position(player.transform);
        itemNames = Items(inventory.Items);
        weaponEnchantments = WeaponEnchantments(inventory.Items);
        armorEnchantments = ArmorEnchantments(inventory.Items);
        numItems = NumItems(inventory.Items);
        weaponItem = player.WeaponItem;
        armorItem = player.ArmorItem;
        has_cloud_boots = player.has_cloud_boots;

    }

    public SaveData(Player player)
    {
        position = Position(player.transform);
        weaponItem = player.WeaponItem;
        armorItem = player.ArmorItem;
        has_cloud_boots = player.has_cloud_boots;
    }

    public SaveData(InventoryManager inventory)
    {
        itemNames = Items(inventory.Items);
        weaponEnchantments = WeaponEnchantments(inventory.Items);
        armorEnchantments = ArmorEnchantments(inventory.Items);
        numItems = NumItems(inventory.Items);
    }

    public SaveData()
    {
        return;
    }

    private float[] Position(Transform transform)
    {
        float[] position = new float[3];
        position[0] = transform.position.x;
        position[1] = transform.position.y;
        position[2] = transform.position.z;
        return position;
    }

    private string[] Items(List<Item> itemList)
    {
        string[] items = new string[itemList.Count];

        for (int i = 0; i < itemList.Count; i++)
        {
            items[i] = itemList[i].ItemName;
        }

        return items;
    }

    private int[] WeaponEnchantments(List<Item> itemList)
    {
        int[] enchantments = new int[itemList.Count];
        for(int i = 0; i < itemList.Count; i++)
        {
            enchantments[i] = (int)itemList[i].WeaponEnchantmentSlot;
        }

        return enchantments;
    }

    private int[] ArmorEnchantments(List<Item> itemList)
    {
        int[] enchantments = new int[itemList.Count];
        for (int i = 0; i < itemList.Count; i++)
        {
            enchantments[i] = (int)itemList[i].ArmorEnchantmentSlot;
        }

        return enchantments;
    }


    private int[] NumItems(List<Item> itemList)
    {
        int[] num = new int[itemList.Count];
        for (int i = 0;i < itemList.Count;i++)
        {
            num[i] = itemList[i].numItems;
        }
        return num;
    }
}
