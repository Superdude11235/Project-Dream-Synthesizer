using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

//Uses binary formatter to serialize save data
public static class SaveSystem
{

    static string playerPath = Application.persistentDataPath + "/PlayerSaveData.sav";
    static string inventoryPath = Application.persistentDataPath + "/InventorySaveData.sav";

   
   public static void Save(Player player, InventoryManager inventoryManager)
    {
        SavePlayer(player);
        SaveInventory(inventoryManager);
    } 
    public static void SavePlayer (Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(playerPath, FileMode.Create);

        SaveData data = new SaveData(player);

        formatter.Serialize(stream, data);

        //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(data.GetType());
        //x.Serialize(stream, data);

        stream.Close();
    }

    public static void SaveInventory(InventoryManager inventoryManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(inventoryPath, FileMode.Create);

        SaveData data = new SaveData(inventoryManager);

        formatter.Serialize(stream, data);

        //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(data.GetType());
        //x.Serialize(stream, data);

        stream.Close();
    }

    public static SaveData LoadPlayerData()
    {
        if (File.Exists(playerPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(playerPath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;

        }
        else
        {
            Debug.LogError("Save file not found in " + playerPath);
            return null;
        }
    }

    public static SaveData LoadInventoryData()
    {
        if (File.Exists(inventoryPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(inventoryPath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;

        }
        else
        {
            Debug.LogError("Save file not found in " + inventoryPath);
            return null;
        }
    }

    public static void DeleteData()
    {
        if (File.Exists(playerPath))
        {
            File.Delete(playerPath);
        }
        if (File.Exists(inventoryPath)) File.Delete(inventoryPath);
        if (InventoryManager.Instance != null) InventoryManager.Instance.resetInventory();
    }

    public static bool IsSaveData()
    {
        return File.Exists(playerPath);
    }
}
