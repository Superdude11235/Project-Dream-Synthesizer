using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    bool picked_up = false;

    public ItemBase ItemBase;

    const string PLAYER_TAG = "Player";

    void Pickup() {
        Item.WeaponEnchantment weaponEnchantment = Item.GetRandomWeaponEnchantment();
        Item.ArmorEnchantment armorEnchantment = Item.GetRandomArmorEnchantment(); ;

        Item item = new Item(ItemBase, weaponEnchantment, armorEnchantment);

        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked");
        Pickup();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!picked_up && collision.collider.CompareTag(PLAYER_TAG))
        {
            picked_up=true;
            Debug.Log("Picked up by player");
            Pickup();

        }
    }

}
