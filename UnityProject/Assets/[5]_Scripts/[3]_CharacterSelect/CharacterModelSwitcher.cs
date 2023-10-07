using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.TextCore;

[System.Serializable]
struct CirclePath
{
    public float path01;
    public float radius;
    public Vector3 circleCenter;

    public CirclePath(float path, float radius, Vector3 tangentCrossPoint)
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
    [SerializeField]float circleSize = 2;

    Vector3 circleCenter;
    int activeModelIndex;
    int amountOfAssets;
    GameObject activeModel;
    Transform characterParent;

    List<IEnumerator> runningCouroutines = new List<IEnumerator>();


    List<GameObject> spawnedCharacterModels = new List<GameObject>();


    public void Slide (Step step)
    {
        if (step == Step.Next)
            StartCoroutine(SlideOperation(-1));
        else
            StartCoroutine(SlideOperation(1));
    }

    IEnumerator SlideOperation(int slideDir) 
    {   
        if (runningCouroutines.Count>0)
            yield break;

        MoveModels(slideDir);

        yield return new WaitUntil(()=>runningCouroutines.Count<=0);
        
        int delNeighbourIndex = slideDir<0?0:spawnedCharacterModels.Count-1;
        DeleteModel(delNeighbourIndex);
        
        int addNeighbourIndex = slideDir<0?spawnedCharacterModels.Count-1:0;
        Vector3 position = spawnedCharacterModels[addNeighbourIndex].transform.parent.position+new Vector3(slideDir*(-xOffset),0,0);
        AddModel(position,(activeModelIndex-3+3*amountOfAssets)%amountOfAssets,slideDir<0?spawnedCharacterModels.Count:0);

        //Update activeModel
        activeModelIndex = (activeModelIndex+(-slideDir))%amountOfAssets;
        activeModel = spawnedCharacterModels[(spawnedCharacterModels.Count/2)];
        AdjustSaturation(activeModel,true);

        UpdateNames();
    }

    void MoveModels(int direction)
    {   
        //CalculatePositions
        Dictionary<int,Tuple<Transform,Vector3,bool>> transformAndTargetPos = new Dictionary<int, Tuple<Transform, Vector3, bool>>();
        int startvalue = direction<0?spawnedCharacterModels.Count-1:0;
        for (int i = startvalue; i >=0 && i<spawnedCharacterModels.Count; i+=direction)
        {   
            if (i+direction<spawnedCharacterModels.Count&&i+direction>=0)
            {   
                Transform currentTransform = spawnedCharacterModels[i].transform.parent;
                Vector3 targetPos = spawnedCharacterModels[i+direction].transform.parent.position;
                bool followCircle = false;
                
               // if (direction<0 && i==(spawnedCharacterModels.Count/2)+1|direction>0 && i==(spawnedCharacterModels.Count/2)-1)
                   // followCircle = true;

                Tuple<Transform,Vector3,bool> newTuple = new Tuple<Transform,Vector3,bool>(currentTransform,targetPos,followCircle);
                transformAndTargetPos.Add(i,newTuple);
                AdjustSaturation(spawnedCharacterModels[i],false);
            }
        }

        //Run Coroutines
        var keys = transformAndTargetPos.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            if (transformAndTargetPos.TryGetValue(keys[i],out Tuple<Transform,Vector3, bool>tuple))
            {
                Transform currentTransform  = tuple.Item1;
                Vector3 targetPos = tuple.Item2;
                bool followCircle = tuple.Item3;
                StartCoroutineWithCleanUp(MoveModel(currentTransform,targetPos,followCircle));
            }

        }
    }

    IEnumerator MoveModel(Transform modelToMove,Vector3 targetPos, bool followCircle=false)
    {
        float moveSpeed = 2;
        float startTime = Time.time;
        Vector3 startPos = modelToMove.position;
        Vector3 currentPos = startPos;

        while (Vector3.Distance(currentPos,targetPos)>0.000001f)
        {
            float t = (Time.time - startTime) /(1/moveSpeed);
            
            if (followCircle)
              currentPos= GetCoordinatesAlongCircle (new CirclePath(t,circleSize,circleCenter));
            else
                currentPos = Vector3.Lerp(currentPos,targetPos,t);

            modelToMove.position = currentPos;
            yield return null;
        }

        modelToMove.position = targetPos;
        yield return null;
    }



    #region  AddModels
   public void AddModels(int amountOfAssets,int[]beforeActiveModelIndex,int[]afterActiveModelIndex)
   {    
        this.amountOfAssets = amountOfAssets;
        circleCenter = anchor.transform.position+new Vector3(0,0,circleSize);

        //Before active models
        List<GameObject> beforeActiveModels;
        SpawnModels(beforeActiveModelIndex,new CirclePath(0.375f,circleSize,circleCenter),out beforeActiveModels,new Vector3(-xOffset,0,0));
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
        SpawnModels(afterActiveModelIndex,new CirclePath(0.125f,circleSize,circleCenter),out afterActiveModels,new Vector3(xOffset,0,0));
        spawnedCharacterModels.AddRange(afterActiveModels);

        //Name Models
        UpdateNames();
   }

   void SpawnModels(int[]activeModelIndex,CirclePath circle, out List<GameObject> spawnedModels,Vector3 offsetInput)
   {
        spawnedModels = new List<GameObject>();
        for (int i = 0; i < activeModelIndex.Length; i++)
        {
            Vector3 position;
            if (i>0)
                position = spawnedModels[i-1].transform.position+offsetInput;
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
        parent.transform.SetParent(anchor);
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

    void AddModel(Vector3 position, int modelIndex,int index)
    {
        SpawnModel(spawnedCharacterModels.Count,modelIndex,position,out GameObject spawnedModel);
        spawnedCharacterModels.Insert(index,spawnedModel);
    }

    void DeleteModel(int modelIndex)
    {
        GameObject modelToDestroy = spawnedCharacterModels[modelIndex];
        spawnedCharacterModels.Remove(modelToDestroy);
        ReferenceEquals(modelToDestroy.transform,null);
        ReferenceEquals(modelToDestroy,null);
        Destroy(modelToDestroy.transform.parent.gameObject);
    }

   Vector2 GetCoordinatesAlongCircle(CirclePath circlePos)
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

    void StartCoroutineWithCleanUp(IEnumerator coroutine)
    {
        StartCoroutine(WaitForCoroutine(coroutine));
    }

    IEnumerator WaitForCoroutine(IEnumerator coroutine)
    {
        runningCouroutines.Add (coroutine);
        yield return StartCoroutine(coroutine);
        runningCouroutines.Remove(coroutine);
    }

   #endregion   
}
