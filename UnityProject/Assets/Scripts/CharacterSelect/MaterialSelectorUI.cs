using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public struct Selector
{
    public Button left, right;

    public void AddListener(UnityEngine.Events.UnityAction prev, UnityEngine.Events.UnityAction next)
    {
        left.onClick.AddListener(prev);
        right.onClick.AddListener (next);
    }
}

[Serializable]
public struct Section
{  
    [Header ("Buttons")]
    public Selector genderSelector;
    public Selector hairColorSelector;
    public Selector skinColorSelector;
}

public class MaterialSelectorUI : MonoBehaviour
{   
    [SerializeField]Section child, parent;

    PropertyHandler propertyHandler = new PropertyHandler();

    void  Awake()
    {
        //Set up Buttons
        child.genderSelector.AddListener(
            () => propertyHandler.SetProperty(Character.Child,ModelProperty.Gender,Step.Prev)
            ,() => propertyHandler.SetProperty(Character.Child,ModelProperty.Gender,Step.Next));
    }

    void UpdateProperties(ModelProperty modelProperty, Character character)
    {
       // propertyHandler.SetProperty(character,modelProperty);
    }
}
