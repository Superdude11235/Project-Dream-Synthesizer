using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    bool picked_up = false;

    public Item Item;

    void Pickup() {

        InventoryManager.Instance.Add(Item);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked");
        Pickup();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!picked_up)
        {
            picked_up=true;
            Debug.Log("Picked up by player");
            Pickup();

        }
    }
}
