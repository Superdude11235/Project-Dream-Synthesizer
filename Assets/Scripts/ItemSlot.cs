using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    Item item;
    public Image icon;
    public GameObject clicked;
    public Button removeButton;
    public GameObject playerObject;
    private Player player;
    public GameObject menusObject;
    private EquipMenu equipMenu;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        clicked.SetActive(false);
    }

    public void OnRemoveButton()
    {
        if (item == player.WeaponItem || item == player.ArmorItem)
        {
            player.Unequip(item);
        }
        InventoryManager.Instance.RemoveItem(item);
    }

    public void EquipItem()
    {
        if (item == null) return;

        if (item != player.WeaponItem && item != player.ArmorItem)
        {
            player.SetEquipment(item);
            clicked.SetActive(true);
        }
        else
        {
            player.Unequip(item);
            clicked.SetActive(false);
        }
    }

    public void ButtonClicked()
    {
        equipMenu.UpdateAllClicked();
    }

    public void UpdateClicked()
    {
        if (item == null) return;

        if (item.ItemType == ItemBase.ItemTypes.WEAPON && item != player.WeaponItem)
        {
            clicked.SetActive(false);
        }
        else if (item.ItemType == ItemBase.ItemTypes.ARMOR && item != player.ArmorItem)
        {
            clicked.SetActive(false);
        }
    }

    private void Awake()
    {
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();
        menusObject = GameObject.Find("Menus");
        equipMenu = menusObject.GetComponent<EquipMenu>();
    }
}
