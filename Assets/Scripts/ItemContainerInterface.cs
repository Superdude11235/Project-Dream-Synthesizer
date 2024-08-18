
public interface ItemContainerInterface
{
    //basic inventory stuff
    void Add(Item item);
    void ListItems(bool forSelectionCrafting);
     void RemoveItem(Item item);

    //crafting stuff
    bool containsItem(Item item);
    int itemCount(Item item);

    //find if player selected enough of the required item. 
    bool findIfHaveEnough(string item, int amountNeeded);

    //after successful crafting, remove the item from the selected items list
    Item findSpecificItemForRemovalCrafting(string itemName);

    //select the item to add it to the selected items list for crafting 
    void selectItemForCraftingAddsOnlyOne(Item item);
    //remove the item to add it to the selected items list for crafting 
    void removeItemForCrafting(Item item);

    int IndexOf(Item item);

    }
