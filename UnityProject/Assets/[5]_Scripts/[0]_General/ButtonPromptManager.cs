using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   static ButtonPromptManager instance;
   static List<ButtonPrompt> buttonPrompts = new List<ButtonPrompt>();

   void Awake()
   {
         if (instance==null)
            instance = this;
        else if (instance!=this)
            Destroy(instance);
   }

   public static void ShowButtonPrompt(Transform target,string buttonName,InputDevice inputDevice, out GameObject spawnedSprite)
   {
      GameObject spriteOnCanvas = GameData.GetData<ButtonPromptAsssets>("ButtonPromptAssets").GetButtonByName(buttonName, inputDevice);
      ShowButtonPrompt(spriteOnCanvas, target,out spawnedSprite);
   }

   public static void ShowButtonPrompt(GameObject spriteOnCanvas,Transform target, out GameObject spawnedSprite)
   {  

      spawnedSprite = Instantiate(spriteOnCanvas);
      Coroutine coroutine = instance.StartCoroutine(AdjustPosition(spawnedSprite,target));
      
      ButtonPrompt buttonPrompt = new ButtonPrompt(spawnedSprite,coroutine,target);
      buttonPrompts.Add (buttonPrompt);
   }

   public static void RemoveButtonPrompt(GameObject spawnedSprite)
   {
      for (int i = 0; i<buttonPrompts.Count; i++)
      {
         if (buttonPrompts[i].instance == spawnedSprite)
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


   static IEnumerator AdjustPosition(GameObject spawnedSprite, Transform target)
   {
      yield return new WaitForEndOfFrame();
      spawnedSprite.transform.position  = Camera.main.WorldToScreenPoint(target.position);
      yield return AdjustPosition(spawnedSprite, target);
   }

   
}
