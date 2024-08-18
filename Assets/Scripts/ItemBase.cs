using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemBase;

//SO to store base for all items
[CreateAssetMenu(fileName ="New Item",menuName ="Item/Create New Item Base")]
public class ItemBase : ScriptableObject
{
    public enum ItemTypes { WEAPON, ARMOR, OTHER}
    public string ItemName;
    public ItemTypes ItemType;
    public Sprite icon;

}
