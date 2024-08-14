using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipMenu : MonoBehaviour
{
    public static bool EquipIsOpen = false;
    public GameObject equipMenuUI;
    
    void Update()
    {
        // if "e" key is pressed, check whether or not equip menu was already open and open/close it accordingly
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (EquipIsOpen)
            {
                equipMenuUI.SetActive(false);
                EquipIsOpen = false;
                Time.timeScale = 1f;
            }
            else
            {
                equipMenuUI.SetActive(true);
                EquipIsOpen = true;
                Time.timeScale = 0f;
                Equipping.InstanceEquipping.ListItems();
            }
        }
    }
}