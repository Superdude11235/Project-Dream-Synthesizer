using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RecipeSelectionManager : MonoBehaviour
{

   public static RecipeSelectionManager Instance;

    //list of crafting recipes that we have 
    //double check that this populates.. 
    public CraftingRecipe[] allRecipes;
   // public List<CraftingRecipe> recipeList = new List<CraftingRecipe>(allRecipes);

    //for getting the items to list them out in the crafting menu (item selection to craft)
    //these are the slots that show what items you need
    public Transform ListedItemsForRecipeContent;
    public GameObject ListedRecipeItems;

    //for getting the names to list them out in the recipe menu
    public Transform RecipeNameContent;
    public GameObject GameRecipe;

    //note below may not be accurate. 
    //2d list, {which recipe}{which ingredient}-> still correct since the gameobject saved does have a list in it 
    public List<GameObject> recipeListingItemSlotPopulated = new List<GameObject>();

    //names for the recipes to be displayed.. please let me know if there is a better way of doing this. 
    public List<GameObject> recipeNames = new List<GameObject>();

    //this will load it when the scene is loaded, by the looks of it we are having crafting be done in another scene 
    private void Awake()
    {
        Instance = this;
        allRecipes = Resources.LoadAll<CraftingRecipe>("ScriptedCraftingRecipes");
        ListRecipeNames();
         
     }

  //list all the recipes that can be selected 
    public void ListRecipeNames() {

        foreach (Transform recipe in RecipeNameContent)
        { 
            Destroy(recipe.gameObject);
       }

        foreach (var recipe in allRecipes)
        {
           
            GameObject obj = Instantiate(GameRecipe, RecipeNameContent);

            //get the recipe name 
            var recipeName = obj.transform.Find("recipeName").GetComponent<Text>();
            Debug.Log(recipeName.text);
            recipeName.text = recipe.recipeName;

            recipeNames.Add(obj);


        }

    }

    public void ListItemsForRecipe(CraftingRecipe recipeSelected)
    {

        foreach (Transform recipe in ListedItemsForRecipeContent)
        {
            Destroy(recipe.gameObject);
        }


        //lists the items in the inventory
        //for the selected recipe we have do the following 
      
            //got the recipe we need and the slots that are going to be populated with the items 
            GameObject obj = Instantiate(ListedRecipeItems, ListedItemsForRecipeContent);

            //list of items from the recipe
            // var itemsRequired = obj.transform.Find("Materials").GetComponent<List<ItemAmount>>();

        //recipe name  NEEDS TO BE FIXED
              var recipeName = obj.transform.Find("recipeName").GetComponent<Text>();
              recipeName.text = recipeSelected.recipeName;


        int count = 0;
        //for each item required for the recipe do the following 
        foreach (var itemForRecipe in recipeSelected.Materials) {

            //make both show up in all the allocated slots 
            //item name 

            var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
            //check this 
            var itemIcon = obj.transform.Find("Image").GetComponent<Image>();

            itemName.text = itemForRecipe.Item.ItemName;
            itemIcon.sprite = itemForRecipe.Item.icon;
            count++;

            }

        //double check this loop

        foreach (Transform itemListed in ListedItemsForRecipeContent) {
            if (count != 0)
            {
                itemListed.Find("ItemNeededForCraftingRecipe").gameObject.SetActive(true);

            }
            else {
                itemListed.Find("ItemNeededForCraftingRecipe").gameObject.SetActive(false);

            }
            count--;
        
        }


        recipeListingItemSlotPopulated.Add(obj);


    }




}
