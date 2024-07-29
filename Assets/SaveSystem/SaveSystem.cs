using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/SaveData.sav";
    public static void SavePlayer (Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(player);

        formatter.Serialize(stream, data);

        //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(data.GetType());
        //x.Serialize(stream, data);

        stream.Close();
    }

    public static SaveData LoadData()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;

        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteData()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
