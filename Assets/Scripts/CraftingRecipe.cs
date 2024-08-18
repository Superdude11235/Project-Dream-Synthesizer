using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount {

    public ItemBase ItemB;
    public int Amount;

}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public int recipeID;
    public List<ItemAmount> Materials;
    public List<ItemBase> Results;

}
