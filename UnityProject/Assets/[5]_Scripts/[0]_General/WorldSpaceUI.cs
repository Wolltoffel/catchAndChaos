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
   static List<Prompt> prompts = new List<Prompt>();

   public Transform globalParent;


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
      _AdjustPosition(spawnedPrompt, target, Vector2.zero);
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spawnedPrompt,target,Vector2.zero));
        
      //Create newnPrompt file and save Coroutine to list
      Prompt promptData = new Prompt(canvasHolder,coroutine,target);
      prompts.Add (promptData);
   } 
 
   public static void ShowButtonPrompt(Transform target,string buttonName, out GameObject canvasHolder, string hintName = "")
   {
      GameObject spriteHolder;
      Sprite buttonSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetButtonSpriteByName(buttonName);
      SpawnPrompt(target,buttonName,buttonSprite, out canvasHolder,out spriteHolder,Vector2.zero, 1.5f);
      if (hintName!="" &&  !GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").WasHintAlreadyShown(hintName))
      {
         Sprite hintSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetHintSpriteByName(hintName);
         GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").AddToShownHints(hintName);
         SpawnHint(buttonName+"_hint",hintSprite,canvasHolder,new Vector2(instance.hintOffsetX,instance.hintOffsetY),spriteHolder.transform);
      }

   }

   static void SpawnHint(string objectName,Sprite sprite,GameObject canvasHolder,Vector2 offset,Transform parent,float scale = 1)
   {
      
      //Add Sprite to Canvas
      GameObject spriteHolder = AddSpriteToCanvas(sprite,canvasHolder.GetComponent<RectTransform>(),objectName);
      spriteHolder.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,8);

      //Set Scale
      spriteHolder.GetComponent<Image>().SetNativeSize();
      Vector3 currentScale = spriteHolder.transform.localScale;
      spriteHolder.transform.localScale = currentScale*scale;

      //Set Parent
      spriteHolder.transform.SetParent(parent);

      //Set Offset
      spriteHolder.transform.localPosition = new Vector3(offset.x,offset.y,0);
   }  

   static void SpawnPrompt(Transform target,string objectName,Sprite sprite, out GameObject canvasHolder,out GameObject spriteHolder, Vector2 offset, float scale = 1f,Transform canvas = null)
   { 
      //Add Canvas if neccessary
      if (canvas==null)
         canvasHolder = SetUpCanvas(objectName);
      else
         canvasHolder = canvas.gameObject;

      //Add Sprite to Canvas
      spriteHolder = AddSpriteToCanvas(sprite,canvasHolder.GetComponent<RectTransform>(),objectName);
      spriteHolder.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,8);

      //Set Scale
      spriteHolder.GetComponent<Image>().SetNativeSize();
      Vector3 currentScale = spriteHolder.transform.localScale;
      spriteHolder.transform.localScale = currentScale*scale;

      //Start Coroutine that follows objects
      _AdjustPosition(spriteHolder, target, offset);
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spriteHolder,target,offset));
      
      //Create new ButtonPrompt file and save Coroutine to lists
      Prompt buttonPrompt = new Prompt(canvasHolder,coroutine,target);
      prompts.Add (buttonPrompt);
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
      for (int i = 0; i<prompts.Count; i++)
      {
         if (prompts[i].instance == spawnedObject)
         {
            DestroyPrompt(prompts[i]);
         } 
      }
   }

    public static void RemoveAllButtonPrompts()
    {
        for (int i = prompts.Count - 1; i >= 0 ; i--)
        {
            DestroyPrompt(prompts[i]);
        }
    }

   static void DestroyPrompt(Prompt buttonPrompt)
   {
      instance.StopCoroutine(buttonPrompt.coroutine);
      Destroy(buttonPrompt.instance);
      prompts.Remove(buttonPrompt);
   }


   static IEnumerator AdjustPosition(GameObject spawnedSprite, Transform target,Vector2 offset)
   {
        while (spawnedSprite!=null && target!=null)
        {
            _AdjustPosition(spawnedSprite,target,offset);
            yield return null;
        }
   }

    //Adjusts Position for first Frame
    static private void _AdjustPosition(GameObject spawnedSprite, Transform target, Vector2 offset, CanvasScaler canvasScaler = null)
    {
        if (canvasScaler == null)
            canvasScaler = spawnedSprite.GetComponentInParent<CanvasScaler>();

        float xScreenToScaler = canvasScaler.referenceResolution.x / Screen.width;
        float yScreenToScaler = canvasScaler.referenceResolution.y / Screen.height;

        Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(target.position);
        Vector2 screenMidPoint = new Vector2(Screen.width * xScreenToScaler, Screen.height * yScreenToScaler) /2;
        Vector2 newPos = new Vector2((worldToScreenPoint.x + offset.x) * xScreenToScaler, (worldToScreenPoint.y + offset.y) * yScreenToScaler) - screenMidPoint;
        RectTransform rect = spawnedSprite.GetComponent<RectTransform>();
        rect.anchoredPosition = newPos;
   }

}
