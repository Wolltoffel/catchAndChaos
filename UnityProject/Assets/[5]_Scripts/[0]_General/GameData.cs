using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class AData : ScriptableObject
{
    public string key;
}

public class GameData : MonoBehaviour
{
    private static GameData instance;

    private static Dictionary<string, AData> dataBase = new Dictionary<string, AData>();

    [SerializeField] private AData[] data;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance);

        //Add files from inspector to static dataBase
        for (int i = 0; i < data.Length; i++)
        {
            dataBase.Add(data[i].key, data[i]);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static T GetData<T>(string key) where T : AData
    {
        AData data;
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

        throw new System.Exception($"The retrun type of the data is not equal to the return type of the key. {System.Environment.NewLine}"
        + $"(Key return type - {data.GetType()}) {System.Environment.NewLine} "
        + $"(Requested return type - {typeof(T)})");
    }

    public static void SetData(AData newData, string key)
    {
        var type  = newData.GetType();
        
        AData oldData = GetData<AData>(key);
        oldData = newData;
    }

}
