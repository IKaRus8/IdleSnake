using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void SavingData<T>(string name, T value)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + $"/{name}.dat");
        SaveData<T> data = new SaveData<T>();
        data.saveData = value;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log($"{name} data saved!");
    }
    
    public static T LoadindData<T>(string name, T type)
    {
        if (File.Exists(Application.persistentDataPath + $"/{name}.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/{name}.dat", FileMode.Open);
            SaveData<T> data = (SaveData<T>)bf.Deserialize(file);
            file.Close();
            var value = data.saveData;
            Debug.Log($"{name} data loaded!");
            return value;
        }
        else
        {
            return type;
        }
    }
}
[Serializable]
struct SaveData<T>
{
    public T saveData;
}

