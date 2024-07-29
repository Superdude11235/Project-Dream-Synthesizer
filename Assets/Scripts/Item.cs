using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item",menuName ="Item/Create New Item")]
public class Item : ScriptableObject
{

    public string ItemName;
    public string ItemType;
    public string EnchantmentSlot1;
    public string EnchantmentSlot2;
    public int numItems;
    public Sprite icon;

}
