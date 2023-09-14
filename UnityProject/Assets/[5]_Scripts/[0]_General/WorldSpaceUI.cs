using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt
{  
   public GameObject instance;
   public Coroutine coroutine;
   public Transform target;

   public Prompt(GameObject instance, Coroutine coroutine, Transform target)
   {
        this.instance = instance;
        this.coroutine = coroutine;
        this.target = target;
   }

}

public class WorldSpaceUI : MonoBehaviour
{

   public float hintOffsetX, hintOffsetY;
   static WorldSpaceUI instance;
   static List<Prompt> buttonPrompts = new List<Prompt>();


   void Awake()
   {
         if (instance==null)
            instance = this;
        else if (instance!=this)
            Destroy(instance);
   }

   public static void ShowPrompt (GameObject prompt,Transform target, string promptName, out GameObject canvasHolder,out GameObject spawnedPrompt)
   {
      canvasHolder = SetUpCanvas(promptName);

      spawnedPrompt = Instantiate(prompt);
      //Add GameObject to canvas
      spawnedPrompt.GetComponent<RectTransform>().SetParent(canvasHolder.GetComponent<RectTransform>());
      //Start Coroutine that follows objects
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spawnedPrompt,target,Vector2.zero));
      
      //Create newnPrompt file and save Coroutine to list
      Prompt promptData = new Prompt(canvasHolder,coroutine,target);
      buttonPrompts.Add (promptData);
   } 
 
   public static void ShowButtonPrompt(Transform target,string buttonName, out GameObject canvasHolder, string hintName = "")
   {
      Sprite buttonSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetButtonSpriteByName(buttonName);
      SpawnButtonPrompt(target,buttonName,buttonSprite, out canvasHolder,Vector2.zero, 1.5f);
      if (hintName!="" &&  !GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").WasHintAlreadyShown(hintName))
      {
         Sprite hintSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetHintSpriteByName(hintName);
         GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").AddToShownHints(hintName);
         SpawnButtonPrompt(target,buttonName,hintSprite, out canvasHolder,new Vector2(instance.hintOffsetX,instance.hintOffsetY), 3,canvasHolder.transform);
      }

   }

   static void SpawnButtonPrompt(Transform target,string objectName,Sprite sprite, out GameObject canvasHolder, Vector2 offset, float scale = 1f,Transform canvasParent = null)
   { 
      //Add Canvas if neccessary
      if (canvasParent==null)
         canvasHolder = SetUpCanvas(objectName);
      else
         canvasHolder = canvasParent.gameObject;

      //Add Sprite to Canvas
      GameObject spriteHolder = AddSpriteToCanvas(sprite,canvasHolder.GetComponent<RectTransform>(),objectName);
      spriteHolder.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,8);
      
      //Start Coroutine that follows objects
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spriteHolder,target,offset,scale));
      
      //Create new ButtonPrompt file and save Coroutine to lists
      Prompt buttonPrompt = new Prompt(canvasHolder,coroutine,target);
      buttonPrompts.Add (buttonPrompt);
   }

   static GameObject SetUpCanvas(string objectName)
   {
         GameObject canvasHolder;
         canvasHolder = new GameObject("WorldUI: "+ objectName);
         Canvas canvas =canvasHolder.AddComponent<Canvas>();
         canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
         CanvasScaler canvasScaler  = canvasHolder.AddComponent<CanvasScaler>();
         canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
         canvasScaler.referenceResolution =new Vector2(3840,2160);
         return canvasHolder;
   }

   static GameObject AddSpriteToCanvas(Sprite sprite,RectTransform parent,string objectName)
   {
      GameObject spriteHolder = new GameObject(objectName);
      Image image = spriteHolder.AddComponent<Image>();
      image.sprite = sprite;
      RectTransform spriteHolderRectTransform = spriteHolder.GetComponent<RectTransform>();
      spriteHolderRectTransform .SetParent(parent);
      spriteHolderRectTransform.localScale = new Vector2(0.3f,0.3f);
      return spriteHolder;
   }
   
   public static void RemovePrompt(GameObject spawnedObject)
   {
      for (int i = 0; i<buttonPrompts.Count; i++)
      {
         if (buttonPrompts[i].instance == spawnedObject)
         {
            DestroyPrompt(buttonPrompts[i]);
         } 
      }
   }

   static void DestroyPrompt(Prompt buttonPrompt)
   {
      instance.StopCoroutine(buttonPrompt.coroutine);
      Destroy(buttonPrompt.instance);
      buttonPrompts.Remove(buttonPrompt);
   }


   static IEnumerator AdjustPosition(GameObject spawnedSprite, Transform target,Vector2 offset, float scale = 1)
   {
      spawnedSprite.transform.localScale = new Vector3(scale,scale,scale);
      while (spawnedSprite!=null && target!=null)
      {
         Vector3 worldToScreenPoint =  Camera.main.WorldToScreenPoint(target.position);
         Vector2 newPos = new Vector2(worldToScreenPoint.x+offset.x*Screen.width*0.001f, worldToScreenPoint.y+offset.y*Screen.height*0.001f);
         spawnedSprite.transform.position  = newPos;
         yield return null;
      }
   }

   
}
