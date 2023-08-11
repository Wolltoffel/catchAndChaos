using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

[Serializable]
public class Selector
{
    [HideInInspector]public ModelProperty modelProperty;
    public Button left, right;

    public void AddListener(UnityEngine.Events.UnityAction prev, UnityEngine.Events.UnityAction next)
    {
        left.onClick.AddListener(prev);
        right.onClick.AddListener (next);
    }

    public void SetModelProperty(ModelProperty modelProperty)
    {
        this.modelProperty = modelProperty;
    }

    public void HideButtons()
    {
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
}

[Serializable]
public class Section
{
    Characters character;
    [HideInInspector] public bool isConfirmed = false;
    Action checkAction;

    public Transform parent;

    [Header("Buttons")]
    public Button confirm;
    public Selector genderSelector;
    public Selector hairColorSelector;
    public Selector skinColorSelector;

    public void SetCharacter(Characters character)
    {
        this.character = character;
    }

    public void SetModelProperties()
    {
        genderSelector.SetModelProperty(ModelProperty.Gender);
        hairColorSelector.SetModelProperty(ModelProperty.Hair);
        skinColorSelector.SetModelProperty(ModelProperty.Skincolor);
    }

    public void AddListeners(Action action)
    {
        checkAction = action;
        confirm.onClick.AddListener(ConfirmSelection);

        AddListenerToSelector(genderSelector);
        AddListenerToSelector(hairColorSelector);
        AddListenerToSelector(skinColorSelector);
    }
    void AddListenerToSelector(Selector selector)
    { PropertyHandler propertyHandler = new PropertyHandler();
        Characters character = this.character;
        Section section = this;
        selector.AddListener(
            () => section.UpdateAndApplyMaterials(selector, character, selector.modelProperty, Step.Prev),
            () => section.UpdateAndApplyMaterials(selector, character, selector.modelProperty, Step.Next));
    }

    public void SetCharacterSpawnAnchor(Transform parent)
    {
        this.parent = parent;
    }

    void UpdateAndApplyMaterials(Selector selector,Characters character, ModelProperty modelProperty, Step step)
    {   
        PropertyHandler propertyHandler = new PropertyHandler();
        propertyHandler.SetProperty(character,selector.modelProperty,step);
        CharacterInstantiator.InstantiateCharacter(character, out GameObject obj, parent);
    }

    public void ConfirmSelection()
    {
        isConfirmed = true;

        genderSelector.HideButtons();
        hairColorSelector.HideButtons();
        skinColorSelector.HideButtons();

        checkAction();
    }
}

public class MaterialSelectorUI : MonoBehaviour
{
    [SerializeField] Transform cameraPosition;
    [SerializeField]Section child, parent;

    void  Awake()
    {   
        child.SetCharacter(Characters.Child);
        parent.SetCharacter(Characters.Parent);
        child.SetModelProperties();
        parent.SetModelProperties();
        child.AddListeners(CheckConfirm);
        parent.AddListeners(CheckConfirm);
        CharacterInstantiator.InstantiateCharacter(Characters.Child, out GameObject obj, child.parent);
        CharacterInstantiator.InstantiateCharacter(Characters.Parent, out obj, parent.parent);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(cameraPosition);
    }

    void Update()
    {
        //Debug.Log (GameData.GetData<PlayerData>("Child").characterAssets.male.GetActiveHairIndex());

    }

    void CheckConfirm()
    {
        if (child.isConfirmed && parent.isConfirmed)
        {
            ScreenSwitcher.SwitchScreen(Screen.GameScreen);
        }
    }
}
