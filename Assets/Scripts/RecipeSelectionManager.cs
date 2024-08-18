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
    public List<CraftingRecipe> recipeList;

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

    //making controllers for each recipe (this is for selecting the recipe in the first place)
    public RecipeController[] recipeControllers;

    public CraftingRecipe currentRecipe;


    private void Awake()
    {
        Instance = this;
        allRecipes = Resources.LoadAll<CraftingRecipe>("ScriptedCraftingRecipes");
        recipeList = new List<CraftingRecipe>(allRecipes);
        ListRecipeNames();

        //also load the inventory too for actually selecting stuff 
       // itemContainer.ListItems(true);
     }


    //assign a recipe controller to each recipe during the initial selection of which crafting recipe is going to be 
    //used aka "Crafting 1" according to docs 
    public void SetRecipesForSelection()
    {
        recipeControllers = RecipeNameContent.GetComponentsInChildren<RecipeController>();
        //Debug.Log(recipeList.Count);
        for (int i = 0; i < recipeList.Count; i++)
        {
            recipeControllers[i+1].AddRecipeRecipeController(allRecipes[i]);

        }

    }

    //list all the recipes that can be selected 
    public void ListRecipeNames() {

        foreach (Transform recipe in RecipeNameContent)
        {
            Destroy(recipe.gameObject);
        }

    
        foreach (var recipe in recipeList)
        {
            GameObject obj = Instantiate(GameRecipe, RecipeNameContent);

            //get the recipe name 
            var recipeName = obj.transform.Find("recipeName").GetComponent<Text>();
            recipeName.text = recipe.recipeName;

            recipeNames.Add(obj);

           
        }

    SetRecipesForSelection();

    }

    public void ListItemsForRecipe(CraftingRecipe recipeSelected)
    {
        InventoryManager.Instance.ListItems(true);

        foreach (Transform recipe in ListedItemsForRecipeContent)
        {
            Destroy(recipe.gameObject);
            
        }

        //for each item required for the recipe do the following 
        foreach (var itemForRecipe in recipeSelected.Materials) {

            for (int i = 0; i < itemForRecipe.Amount; i++) {

                GameObject obj = Instantiate(ListedRecipeItems, ListedItemsForRecipeContent);

                //make both show up in all the allocated slots 
                //item name 

                var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                //check this 
                var itemIcon = obj.transform.Find("Image").GetComponent<Image>();

                itemName.text = itemForRecipe.ItemB.ItemName;
                itemIcon.sprite = itemForRecipe.ItemB.icon;

                recipeListingItemSlotPopulated.Add(obj);

            }

        }

        currentRecipe = recipeSelected;

    }


    public void activateCraft()
    {
        craft(InventoryManager.Instance);
        InventoryManager.Instance.ListItems(true);

    }

    public bool canCraft(ItemContainerInterface itemContainer)
    {

        //loop through the list of materials 
        foreach (ItemAmount itemAmount in currentRecipe.Materials)
        {
            //check if we have enough of each item required for the crafting recipe in the list of selected items

            if (!itemContainer.findIfHaveEnough(itemAmount.ItemB.ItemName, itemAmount.Amount))
            {
                //no we do not have enough to craft at least one 
                return false;
            }
        }
        // yes
        return true;

    }


    public void craft(ItemContainerInterface itemContainer)
    {
        Item.WeaponEnchantment Ench1;
        Item.ArmorEnchantment Ench2;
        if (canCraft(itemContainer))
        {

            List<int> indexes = new List<int>();
            foreach (ItemAmount itemAmount in currentRecipe.Materials)
            {
                for (int i = 0; i < itemAmount.Amount; i++)
                {
                    Item toBeRemoved = itemContainer.findSpecificItemForRemovalCrafting(itemAmount.ItemB.ItemName);
                    int index = itemContainer.IndexOf(toBeRemoved);
                    indexes.Add(index);
                }
            }

            int count = 0;
            //removed the used items from the inventory
            //need to specifically remove the ones from the list that were selected. 

            //Take enchantment from first item
            Item firstItem = InventoryManager.Instance.InventoryItems[indexes[0]].item;
            Ench1 = firstItem.WeaponEnchantmentSlot;
            Ench2 = firstItem.ArmorEnchantmentSlot;

            foreach (ItemAmount itemAmount in currentRecipe.Materials)
            {
                for (int i = 0; i < itemAmount.Amount; i++)
                {
                    //Destroy(InventoryManager.Instance.InventoryItems[indexes[count]].gameObject);
                    InventoryManager.Instance.InventoryItems[indexes[count]].RemoveItem();
                    InventoryManager.Instance.ItemContent.GetChild(0).transform.parent = null;
                    count++;

                }
            }

            //add the items we made to the inventory :) 
            foreach (ItemBase itemResultBase in currentRecipe.Results)
            {
                Item itemResult = new Item(itemResultBase,Ench1, Ench2);
                itemContainer.Add(itemResult);

            }

        }
    }



}
