using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrompt
{  
   public GameObject instance;
   public Coroutine coroutine;
   public Transform target;

   public ButtonPrompt(GameObject instance, Coroutine coroutine, Transform target)
   {
        this.instance = instance;
        this.coroutine = coroutine;
        this.target = target;
   }

}

public class ButtonPromptManager : MonoBehaviour
{
   public float hintOffsetX, hintOffsetY;
   static ButtonPromptManager instance;
   static List<ButtonPrompt> buttonPrompts = new List<ButtonPrompt>();

   void Awake()
   {
         if (instance==null)
            instance = this;
        else if (instance!=this)
            Destroy(instance);
   }

   public static void ShowButtonPrompt(Transform target,string buttonName,InputDevice inputDevice, out GameObject canvasHolder, string hintName = "")
   {
      Sprite buttonSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetButtonSpriteByName(buttonName, inputDevice);
      ShowPrompt(target,buttonName,buttonSprite, out canvasHolder,Vector2.zero, 1.5f);
      if (hintName!="")
      {
         Sprite hintSprite = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetHintSpriteByName(hintName);
         ShowPrompt(target,buttonName,hintSprite, out canvasHolder,new Vector2(instance.hintOffsetX,instance.hintOffsetY), 3);
      }

   }

   static void ShowPrompt(Transform target,string buttonName,Sprite sprite, out GameObject canvasHolder, Vector2 offset, float scale = 1f)
   {
      
      //Set up Canvas
      canvasHolder = new GameObject("ButtonPromptCanvas: "+ buttonName);
      Canvas canvas =canvasHolder.AddComponent<Canvas>();
      canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
      CanvasScaler canvasScaler  = canvasHolder.AddComponent<CanvasScaler>();
      canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      canvasScaler.referenceResolution =new Vector2(3840,2160);

      //Set Up Rect Transform
      RectTransform rectTransform = canvasHolder.GetComponent<RectTransform>();

      //Add Sprite
      GameObject spriteHolder = new GameObject(buttonName);
      Image image = spriteHolder.AddComponent<Image>();
      image.sprite = sprite;
      RectTransform spriteHolderRectTransform = spriteHolder.GetComponent<RectTransform>();
      spriteHolderRectTransform .SetParent(canvasHolder.transform);
      spriteHolderRectTransform.localScale = new Vector2(0.3f,0.3f);

      
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spriteHolder,target,offset,scale));
      
      ButtonPrompt buttonPrompt = new ButtonPrompt(canvasHolder,coroutine,target);
      buttonPrompts.Add (buttonPrompt);
   }

   

   public static void RemoveButtonPrompt(GameObject spawnedObjet)
   {
      for (int i = 0; i<buttonPrompts.Count; i++)
      {
         if (buttonPrompts[i].instance == spawnedObjet)
         {
            DestroyButtonPrompt(buttonPrompts[i]);
         }
            
      }
   }

   static void DestroyButtonPrompt(ButtonPrompt buttonPrompt)
   {
      instance.StopCoroutine(buttonPrompt.coroutine);
      Destroy(buttonPrompt.instance);
      buttonPrompts.Remove(buttonPrompt);
   }


   static IEnumerator AdjustPosition(GameObject spawnedSprite, Transform target,Vector2 offset, float scale = 1)
   {
      spawnedSprite.transform.localScale = new Vector3(scale,scale,scale);
      while (spawnedSprite!=null)
      {
         Vector3 worldToScreenPoint =  Camera.main.WorldToScreenPoint(target.position);
         Vector2 newPos = new Vector2(worldToScreenPoint.x+offset.x, worldToScreenPoint.y+offset.y);
         spawnedSprite.transform.position  = newPos;
         yield return null;
      }
   }

   
}
