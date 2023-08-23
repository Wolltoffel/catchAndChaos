using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class StaticData : ScriptableObject
{
    public string key;
}

public class GameData : MonoBehaviour
{
    private static GameData instance;

    private static Dictionary<string, UnityEngine.Object> dataBase = new Dictionary<string, UnityEngine.Object>();

    [SerializeField] private StaticData[] data;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);

        //Add files from inspector to static dataBase
        for (int i = 0; i < data.Length; i++)
        {
            SetData(data[i], data[i].key);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static T GetData<T>(string key) where T : UnityEngine.Object
    {
        UnityEngine.Object data;
        try
        {
            dataBase.TryGetValue(key, out data);
        }
        catch (System.Exception)
        {
            throw new System.Exception($"The given key ({key}) was not found.");
        }

        if (data is T)
            return (T)data;

        throw new System.Exception($"The retrun type of the data is not equal to the return type of the key.");
    }

    public static void SetData(UnityEngine.Object newData, string key)
    {
        if (dataBase.ContainsKey(key))
        {
            dataBase.Remove(key);
            Debug.Log($"Key \"{key}\" was Overwritten");
        }
        dataBase.Add(key,newData);
    }
}
