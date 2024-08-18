using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeController : MonoBehaviour
{
    //public RecipeController Instance;
   public CraftingRecipe recipe;

    private void Awake()
    {
      //  Instance = this;
    }

    //for actually selecting the recipe
    //section for "Crafting 1" aka initial selection of what recipe is selected 
    public void LoadRecipe()
    {
        //load "Crafting 2" aka call the function that displays the next crafting screen 
        RecipeSelectionManager.Instance.ListItemsForRecipe(recipe);
    }

    public void AddRecipeRecipeController(CraftingRecipe newRecipe)
    {
        // counter++;
        recipe = newRecipe;

    }

}
