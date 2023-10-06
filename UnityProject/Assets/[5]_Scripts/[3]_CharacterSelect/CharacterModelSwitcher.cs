using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

struct Circle
{
    public float path01;
    public float radius;
    public Vector3 circleCenter;

    public Circle(float path, float radius, Vector3 tangentCrossPoint)
    {
        this.path01 = path;
        this.radius = radius;
        this.circleCenter = tangentCrossPoint;
    }
}

public class CharacterModelSwitcher : MonoBehaviour
{

    [SerializeField]Transform anchor;
    [SerializeField] float xOffset = 4f;
   int activeModelIndex;
   int amountOfAssets;
   GameObject activeModel;
   Transform characterParent;

   List<GameObject> spawnedCharacterModels = new List<GameObject>();


    public void Slide (Step step)
    {
        if (step == Step.Next)
            SlideOperation(-1);
        else
            SlideOperation(1);

        UpdateNames();
    }

    void SlideOperation(int slideDir) 
    {
        //Normalize direction
        if (slideDir!=0)
            slideDir = (slideDir*slideDir)/slideDir;
        
        MoveModels(slideDir);

        UpdateActiveModel();
        
        int delNeighbourIndex = slideDir<0?0:spawnedCharacterModels.Count-1;
        DeleteModel(delNeighbourIndex);
        
        int addNeighbourIndex = slideDir<0?spawnedCharacterModels.Count-1:0;
        Vector3 position = spawnedCharacterModels[addNeighbourIndex].transform.position+new Vector3(slideDir*(-xOffset),0,0);
        AddModel(position,(activeModelIndex+3*(-slideDir)+3*amountOfAssets)%amountOfAssets);
        
        activeModelIndex = (activeModelIndex+(-slideDir))%amountOfAssets;
    }

    void MoveModels(int direction)
    {   
        int startvalue = direction<0?spawnedCharacterModels.Count-1:0;
        for (int i = startvalue; i >=0 && i<spawnedCharacterModels.Count; i+=direction)
        {   
            if (i+direction<spawnedCharacterModels.Count&&i+direction>=0){
                spawnedCharacterModels[i].transform.parent.position = spawnedCharacterModels[i+direction].transform.parent.position;
                AdjustSaturation(spawnedCharacterModels[i],false);
            }
        }

        /*for (int i = spawnedCharacterModels.Count-1; i >0; i--)
        {    
            spawnedCharacterModels[i].transform.parent.position = spawnedCharacterModels[i-1].transform.parent.position;
            AdjustSaturation(spawnedCharacterModels[i],false);
        }*/
    }



    #region  AddModels
   public void AddModels(int amountOfAssets,int[]beforeActiveModelIndex,int[]afterActiveModelIndex)
   {    
        this.amountOfAssets = amountOfAssets;

        float circleSize = 2;
        Vector3 circleCenter = anchor.transform.position+new Vector3(0,0,circleSize);

        //Before active models
        List<GameObject> beforeActiveModels;
        SpawnModels(beforeActiveModelIndex,new Circle(0.375f,circleSize,circleCenter),out beforeActiveModels);
        //Add before active in reverse
        for (int i = beforeActiveModels.Count-1; i >=0; i--)
        {
            spawnedCharacterModels.Add(beforeActiveModels[i]);
        }

        //Spawn ActiveModel
        activeModelIndex = (beforeActiveModelIndex[0]+1)%amountOfAssets;
        SpawnModel(beforeActiveModelIndex.Length,activeModelIndex,anchor.transform.position,out activeModel);
        AdjustSaturation(activeModel,true);
        spawnedCharacterModels.Add(activeModel);
        
        //After after active models
        List<GameObject> afterActiveModels;
        SpawnModels(afterActiveModelIndex,new Circle(0.125f,circleSize,circleCenter),out afterActiveModels);
        spawnedCharacterModels.AddRange(afterActiveModels);

        //Name Models
        UpdateNames();
   }

   void SpawnModels(int[]activeModelIndex,Circle circle, out List<GameObject> spawnedModels)
   {
        Vector3 offset = new Vector3(xOffset,0,0);
        spawnedModels = new List<GameObject>();
        for (int i = 0; i < activeModelIndex.Length; i++)
        {
            Vector3 position;
            if (i>0)
                position = spawnedModels[i-1].transform.position+offset;
            else
            {
                Vector2 position2D = GetCoordinatesAlongCircle(circle);
                position = new Vector3(position2D.x,circle.circleCenter.y,position2D.y);
            }
                
            SpawnModel(i,activeModelIndex[i],position, out GameObject spawnedModel );
            spawnedModels.Add(spawnedModel);
        }
   }

   void SpawnModel(int index, int modelIndex,Vector3 position, out GameObject spawnedModel){
        GameObject parent = new GameObject(name+"_Parent_"+index);
        parent.transform.position = position;
        parent.transform.rotation = Quaternion.Euler(0,180,0);
        CharacterInstantiator.AddCharacter(Characters.Child,out spawnedModel,parent.transform, position,modelIndex,true);
        spawnedModel.transform.SetParent (parent.transform);
        spawnedModel.name = name+"_"+index;
        AdjustSaturation(spawnedModel,false);
   }

   void UpdateNames()
   {
        for (int i = 0; i < spawnedCharacterModels.Count; i++)
        {
            spawnedCharacterModels[i].transform.parent.name = $"name+({i})";
        }
   }
   #endregion

    #region  HelperFunctions

    void UpdateActiveModel()
    {
        activeModel = spawnedCharacterModels[(spawnedCharacterModels.Count/2)+1];
        AdjustSaturation(activeModel,true);
    }
    void AddModel(Vector3 position, int modelIndex)
    {
        SpawnModel(spawnedCharacterModels.Count,modelIndex,position,out GameObject spawnedModel);
        spawnedCharacterModels.Add(spawnedModel);
    }

    void DeleteModel(int modelIndex)
    {
        GameObject lastModel = spawnedCharacterModels[modelIndex];
        spawnedCharacterModels.Remove(lastModel);
        Destroy(lastModel.transform.parent.gameObject);
    }

   Vector2 GetCoordinatesAlongCircle(Circle circlePos)
   {
        float t = circlePos.path01;
        Vector3 circleTangentCrossPoint = circlePos.circleCenter;
        float circleRadius = circlePos.radius;

        t = Mathf.Clamp01(t);

        float angle = t*2*Mathf.PI;

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return new Vector2(x,y)*circleRadius+new Vector2(circleTangentCrossPoint.x,circleTangentCrossPoint.z);
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
   #endregion   
}
