using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

//Class to store individual items

[System.Serializable]
public class Item
{
    
    public enum WeaponEnchantment { DAMAGE_UP, CRIT_CHANCE, HP_DRAIN, NONE};
    public enum ArmorEnchantment { HP_UP, COUNTER, SPEED_UP, NONE};


    public string ItemName;
    public ItemBase.ItemTypes ItemType;
    public WeaponEnchantment WeaponEnchantmentSlot;
    public ArmorEnchantment ArmorEnchantmentSlot;
    public int numItems;
    [NonSerialized] public Sprite icon;

    public Item(ItemBase itemBase, WeaponEnchantment WeaponEnchantmentSlot, ArmorEnchantment ArmorEnchantmentSlot)
    {
        ItemName = itemBase.ItemName;
        ItemType = itemBase.ItemType;
        this.WeaponEnchantmentSlot = WeaponEnchantmentSlot;
        this.ArmorEnchantmentSlot = ArmorEnchantmentSlot;
        icon = itemBase.icon;
        this.numItems = 1;
    }

    public Item(ItemBase itemBase, WeaponEnchantment WeaponEnchantmentSlot, ArmorEnchantment ArmorEnchantmentSlot, int numItems)
    {
        ItemName = itemBase.ItemName;
        ItemType = itemBase.ItemType;
        this.WeaponEnchantmentSlot = WeaponEnchantmentSlot;
        this.ArmorEnchantmentSlot = ArmorEnchantmentSlot;
        icon = itemBase.icon;
        this.numItems = numItems;
    }

    public static WeaponEnchantment GetRandomWeaponEnchantment()
    {
        int randomNum = UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeaponEnchantment)).Length - 1);
        return (WeaponEnchantment)randomNum;
    }

    public static ArmorEnchantment GetRandomArmorEnchantment()
    {
        int randomNum = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ArmorEnchantment)).Length - 1);
        return (ArmorEnchantment)randomNum;
    }

    public bool Equals(Item item)
    {
        return (item.ItemName == this.ItemName && item.WeaponEnchantmentSlot == this.WeaponEnchantmentSlot && item.ArmorEnchantmentSlot == this.ArmorEnchantmentSlot);
    }

    public static bool Equals(Item item1, Item item2)
    {
        return (item1.ItemName == item2.ItemName && item1.WeaponEnchantmentSlot == item2.WeaponEnchantmentSlot && item1.ArmorEnchantmentSlot == item2.ArmorEnchantmentSlot);
    }
}
