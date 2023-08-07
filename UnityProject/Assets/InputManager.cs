using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static Dictionary<string,Coroutine> inputs = new Dictionary<string, Coroutine>();

    private static InputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    //public static void SetUpAxis();

    public static string SetUpMouse(Action function, int callCount, int mouseNumber)
    {
        Func<bool> inputCondition = () => Input.GetMouseButtonDown(mouseNumber);
        string dictionaryKey = "Mouse" + mouseNumber;

        Coroutine coroutine = instance.StartCoroutine(WaitForInputAndCall(function, callCount, inputCondition));
        inputs.Add(dictionaryKey, coroutine);

        return dictionaryKey;
    }

    public static string SetUpButton(Action function, int callCount, KeyCode key)
    {
        Func<bool> inputCondition = () => Input.GetKeyDown(key);
        string dictionaryKey = key.ToString();

        Coroutine coroutine = instance.StartCoroutine(WaitForInputAndCall(function,callCount, inputCondition));
        inputs.Add(dictionaryKey,coroutine);

        return dictionaryKey;
    }

    public static void RemoveAllInputs()
    {
        instance.StopAllCoroutines();
        inputs.Clear();
    }

    public static void RemoveInputAt(KeyCode inputKey)
    {
        RemoveInputAt(inputKey.ToString());
    }
    public static void RemoveInputAt(string key)
    {
        Coroutine coroutine;
        inputs.Remove(key, out coroutine);

        instance.StopCoroutine(coroutine);
    }

    private static IEnumerator WaitForInputAndCall(Action function, int callCount, Func<bool> input)
    {
        while (callCount > 0)
        {
            yield return new WaitUntil(input);

            function();

            callCount--;
        }
    }
}
