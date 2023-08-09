using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModelProperty
{
    Hair, Gender, Skincolor
}

public enum Character
{
    Child, Parent
}

public class ModelUpdater : MonoBehaviour
{
    GameObject childBody, parentBody;

    void UpdateModel (Character character,ModelProperty modelProperty)
    {
        GameObject objectToUpdate = null;
        SkinnedMeshRenderer skinnedMeshRenderer;
        string searchKey = "";
        CharacterAssetContainer characterAssetContainer = null;

        //Choose which characters material should be updated
        if (character == Character.Child)
        {
            objectToUpdate = childBody;
            searchKey = "Child";

            ChildData childData = GameData.GetData<PlayerAgent>(searchKey) as ChildData;
            Gender gender = childData.tempGender;

            if (gender == Gender.Male)
                characterAssetContainer = childData.skinContainerBoy;
            else if (gender == Gender.Female)
                characterAssetContainer = childData.skinContainerGirl;   
            
        }
        else if (character == Character.Parent)
        {
            objectToUpdate = parentBody;
            searchKey = "Parent";
            ParentData parentData = GameData.GetData<PlayerAgent>(searchKey) as ParentData;
            characterAssetContainer=parentData.skinContainer;
        }

        //Get skinned Mesh Renderer
        skinnedMeshRenderer = objectToUpdate.GetComponent<SkinnedMeshRenderer>();

        //Get Materials from Renderer
        Material[] materials = skinnedMeshRenderer.materials;

        //Update Materials
        /*
            MaterialIndex to Name Matrix
            0: Hair
            1: Face
            2: Body
            3. Clothing
        */

        //Get Character Asset Container

        switch (modelProperty)
        {
            case ModelProperty.Hair:
                characterAssetContainer.NextActiveHairIndex();
            break;
            case ModelProperty.Skincolor:
                characterAssetContainer.NextActiveSkinIndex();
            break;
        }

    }

}
