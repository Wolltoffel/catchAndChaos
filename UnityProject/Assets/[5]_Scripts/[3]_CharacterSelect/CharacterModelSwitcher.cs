using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterModelSwitcher : MonoBehaviour
{

    [SerializeField]Transform anchor;
   int activeModelIndex;
   int [] beforeActiveModelIndex;
   int [] afterActiveModelIndex;

   GameObject activeModel;
   List<GameObject> afterActiveModels = new List<GameObject>();
   List<GameObject> beforeActiveModels = new List<GameObject>();

    public void SlideLeft() 
    {
        //activeModel.transform.position = beforeActiveModels[0].transform.position;

        List<GameObject> allCharacterModels = new List<GameObject>();
        allCharacterModels.AddRange(beforeActiveModels);
        allCharacterModels.Add(activeModel);
        allCharacterModels.AddRange(afterActiveModels);

        for (int i = allCharacterModels.Count-1; i >0; i--)
        {
            if (allCharacterModels[i]==activeModel)
                allCharacterModels[i].transform.position = allCharacterModels[i-1].transform.position;
            else
                allCharacterModels[i].transform.parent.gameObject.transform.position = allCharacterModels[i-1].transform.position;

            AdjustSaturation(allCharacterModels[i],false);
        }

        //Update active model
        activeModel = allCharacterModels[beforeActiveModels.Count];
        AdjustSaturation(activeModel,true);

        //Delete last Model
        GameObject lastModel = allCharacterModels[0];
        beforeActiveModels.Remove(lastModel);
        Destroy(lastModel.transform.parent.gameObject);

        //Add first Model
    }

    
   public void SpawnModels(int activeModelIndex,int[]beforeActiveModelIndex,int[]afterActiveModelIndex)
   {
        CharacterInstantiator.InstantiateCharacter(Characters.Child,out activeModel,anchor.position);
        float circleSize = 2;
        Vector3 circleCenter = anchor.transform.position;
        circleCenter.z+=circleSize;
        
        //After active model
        float circlePath=-0.5f;
        for (int i = 0; i < afterActiveModelIndex.Length; i++)
        {
            Vector2 position2D = GetCoordinatesAlongCircle(circlePath, circleSize,circleCenter);
            Vector3 position = new Vector3(position2D.x,circleCenter.y,position2D.y);
            GameObject parent = new GameObject("AfterActiveModelParent_"+i);
            parent.transform.position = position;
            parent.transform.rotation = Quaternion.Euler(0,180,0);
            GameObject spawnedModel;
            CharacterInstantiator.AddCharacter(Characters.Child,out spawnedModel,parent.transform, position,afterActiveModelIndex[i],true);
            afterActiveModels.Add(spawnedModel);
            afterActiveModels[i].transform.SetParent (parent.transform);
            afterActiveModels[i].name = "AferActiveModel_"+i;
            AdjustSaturation(afterActiveModels[i],false);
            circlePath+=0.25f;
        }

        //Before active model
        circlePath=0.5f;
        for (int i = 0; i < beforeActiveModelIndex.Length; i++)
        {
            Vector2 position2D = GetCoordinatesAlongCircle(circlePath, circleSize,circleCenter);
            Vector3 position = new Vector3(position2D.x,circleCenter.y,position2D.y);
            GameObject parent = new GameObject("BeforeActiveModelParent_"+i);
            parent.transform.position = position;
            parent.transform.rotation = Quaternion.Euler(0,180,0);
            GameObject spawnedModel;
            CharacterInstantiator.AddCharacter(Characters.Child,out spawnedModel,parent.transform, position,beforeActiveModelIndex[i],true);
            beforeActiveModels.Add(spawnedModel);
            beforeActiveModels[i].transform.SetParent (parent.transform);
            beforeActiveModels[i].name = "BeforeActiveModel_"+i;
            AdjustSaturation(beforeActiveModels[i],false);
            circlePath-=0.25f;
        }

   }

   Vector2 GetCoordinatesAlongCircle(float t,float center,Vector3 circlePos)
   {
        t = Mathf.Clamp01(t);

        float angle = t*2*Mathf.PI;

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return new Vector2(x,y)*center+new Vector2(circlePos.x,circlePos.z);
   }

   void AdjustSaturation (GameObject parent, bool saturate)
   {
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        for (int i= 0; i<renderers.Length;i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j= 0; j<materials.Length;j++)
            {
                if (saturate)
                    materials[j].SetFloat("_Saturation",1);
                else
                    materials[j].SetFloat("_Saturation",0);
            }
        }
   }    
}
