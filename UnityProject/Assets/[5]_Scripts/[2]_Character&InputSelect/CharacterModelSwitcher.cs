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
    [SerializeField] float moveSpeed = 4;
    [SerializeField]float moveSpeedMultiplier=2;


    Vector3 circleCenter;
    int activeModelIndex;
    int amountOfAssets;
    GameObject activeModel;
    Transform characterParent;

    bool activeFastSlide;
    Coroutine fastSlideTimer;

    Characters characters;

    List<IEnumerator> runningCouroutines = new List<IEnumerator>();
    List<GameObject> spawnedCharacterModels = new List<GameObject>();

    Dictionary<int,Tuple<Transform,Vector3,bool,bool>> movementInfo;


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
        {
            FinishCoroutines();
            ActivateFastSlide();
        }
        
        yield return new WaitUntil(()=>runningCouroutines.Count<=0);

        activeModel = spawnedCharacterModels[((spawnedCharacterModels.Count/2)-(slideDir))];
        CharacterInstantiator.SetActiveCharacter(characters,activeModel);

        MoveModels(slideDir, activeFastSlide);

        //Wait until all models are moved
        yield return new WaitUntil(()=>runningCouroutines.Count<=0);
        
        AddAndDeleteModels(slideDir);

        activeModelIndex = (activeModelIndex+(-slideDir))%amountOfAssets;

        UpdateNames();
    }

    void ActivateFastSlide()
    {
         if (fastSlideTimer!=null)
            StopCoroutine(fastSlideTimer);
        fastSlideTimer = StartCoroutine(FastSlideTime());
    }
    IEnumerator FastSlideTime()
    {
        activeFastSlide = true;
        yield return new WaitForSeconds(0.1f);
        activeFastSlide = false;
    }

    void AddAndDeleteModels(int slideDir)
    {
        int delNeighbourIndex = slideDir<0?0:spawnedCharacterModels.Count-1;
        DeleteModel(delNeighbourIndex);
        
        int addNeighbourIndex = slideDir<0?spawnedCharacterModels.Count-1:0;
        Vector3 position = spawnedCharacterModels[addNeighbourIndex].transform.parent.position+new Vector3(slideDir*(-xOffset),0,0);
        AddModel(position,(activeModelIndex-3+3*amountOfAssets)%amountOfAssets,slideDir<0?spawnedCharacterModels.Count:0);
    }

    void MoveModels(int direction, bool fast)
    {   
        movementInfo = new Dictionary<int, Tuple<Transform, Vector3, bool,bool>>();
        int startvalue = direction<0?spawnedCharacterModels.Count-1:0;
        
        for (int i = startvalue; i >=0 && i<spawnedCharacterModels.Count; i+=direction)
        {   
            if (i+direction<spawnedCharacterModels.Count&&i+direction>=0)
            {   
                movementInfo.Add(i,ComputeMovementInfo(i,direction));
                AdjustHighlight(spawnedCharacterModels[i],1);
            }
        }


        if (fast)
            FinishCoroutines();
        else
            RunCoroutines();
       
    }

    Tuple<Transform,Vector3,bool,bool> ComputeMovementInfo(int index, int direction)
    {
        Transform currentTransform = spawnedCharacterModels[index].transform.parent;
        Vector3 targetPos = spawnedCharacterModels[index+direction].transform.parent.position; 
        bool followCircle = direction<0 && index==(spawnedCharacterModels.Count/2)+1|direction>0 
        && index==(spawnedCharacterModels.Count/2)-1;
        bool highlight = spawnedCharacterModels[index]==activeModel;

        return new Tuple<Transform,Vector3,bool,bool>(currentTransform,targetPos,followCircle,highlight);
    }

    void RunCoroutines()
    {
        var keys = movementInfo.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            if (movementInfo.TryGetValue(keys[i],out Tuple<Transform,Vector3, bool,bool>tuple))
            {
                Transform currentTransform  = tuple.Item1;
                Vector3 targetPos = tuple.Item2;
                bool followCircle = tuple.Item3;
                bool highlight = tuple.Item4;
                StartCoroutineWithCleanUp(MoveModel(currentTransform,targetPos,followCircle,highlight,keys[i]));
            }
        }
    }

    IEnumerator MoveModel(Transform currentTransform,Vector3 targetPos, bool followCircle, bool highlight, int removeKey)
    {   
        float startTime = Time.time;
        Vector3 startPos = currentTransform.position;
        Vector3 currentPos = startPos;

        float tempMoveSpeed = Mathf.Clamp(moveSpeed,0.1f,moveSpeed);
        if (activeFastSlide)
            tempMoveSpeed*=moveSpeedMultiplier;

        while (currentTransform!=null&&Vector3.Distance(currentPos,targetPos)>0.000001f&& movementInfo.TryGetValue(removeKey,out var info))
        {   
            float t = (Time.time - startTime) /(1/tempMoveSpeed);
            
            if (followCircle)
                currentPos= GetCoordinatesAlongCircle (new CirclePath(t,circleSize,circleCenter));
            else
                currentPos = Vector3.Lerp(currentPos,targetPos,t);
       
            if (highlight)
                AdjustHighlight(currentTransform.gameObject,Mathf.Lerp(1,0,50*t));

            if (currentTransform!=null)
            {
                currentTransform.position = currentPos;
            }
            
            yield return null;
        }

        if (currentTransform!=null)
        {
            currentTransform.position = targetPos;
            if (highlight)
                AdjustHighlight(currentTransform.gameObject,0);
        }   
        
        movementInfo.Remove(removeKey);
    }

    void FinishCoroutines()
    {
        var keys = movementInfo.Keys.ToList();

         for (int i = 0; i < keys.Count; i++)
        {
            if (movementInfo.TryGetValue(keys[i],out Tuple<Transform,Vector3, bool,bool>tuple))
            {
                Transform currentTransform  = tuple.Item1;
                Vector3 targetPos = tuple.Item2;
                bool highlight = tuple.Item4;

                if (highlight)
                    AdjustHighlight(currentTransform.gameObject,0);
                
                currentTransform.position = targetPos;
                movementInfo.Remove(keys[i]);
            }
        }
    }



    #region  AddModels
   public void AddModels(int amountOfAssets,int[]beforeActiveModelIndex,int[]afterActiveModelIndex, Characters characters)
   {    
        this.amountOfAssets = amountOfAssets;
        this.characters = characters;
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
        AdjustHighlight(activeModel,0);
        spawnedCharacterModels.Add(activeModel);
        CharacterInstantiator.SetActiveCharacter(characters,activeModel);
        
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
        CharacterInstantiator.AddCharacter(characters,out spawnedModel,parent.transform, position,modelIndex,true);
        spawnedModel.transform.SetParent (parent.transform);
        spawnedModel.name = name+"_"+index;
        if (characters == Characters.Child)
            spawnedModel.GetComponent<Animator>().SetInteger("ChildIndex",10);
        else
            spawnedModel.GetComponent<Animator>().SetInteger("MomIndex",10);
        AdjustHighlight(spawnedModel,1);
   }


   #endregion

    #region  HelperFunctions


    void UpdateNames()
   {
        for (int i = 0; i < spawnedCharacterModels.Count; i++)
        {
            spawnedCharacterModels[i].transform.parent.name = $"Model_({i})";
            spawnedCharacterModels[i].transform.name = $"ModelInstance_({i})";
        }
   }
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

   void AdjustHighlight (GameObject parent, float value)
   {

        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
        value = Mathf.Clamp01(value);

        for (int i= 0; i<renderers.Length;i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j= 0; j<materials.Length;j++)
            {
                materials[j].SetFloat("_AccentColorTopOpacity",value);
            }
        }
   }

   float GetHighlightValue(GameObject parent)
   {
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
        Material[] materials = renderers[0].materials;
        
        return materials[0].GetFloat("_AccentColorTopOpacity");
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

    public void HideAll()
    {
        for (int i = 0; i < spawnedCharacterModels.Count; i++)
        {
            if (spawnedCharacterModels[i]!=activeModel)
                spawnedCharacterModels[i].SetActive(false);
        }


        Destroy(this);
    }

   #endregion   
}
