using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollSelection : MonoBehaviour
{
    [SerializeField] int maxElementsInARow;
    [SerializeField] int paddingSide;
    [SerializeField] GameObject template;
    
    [SerializeField]float slideSpeed=6;
    [SerializeField]float selectedElementScaleSpeed=6;
    [SerializeField] float selectedItemScale = 1.2f;

    [SerializeField] CharacterModelSwitcher characterModelSwitcher;

    [HideInInspector]Characters character; 
    
    Rect templateRect;
    
    List <Sprite> sprites = new List<Sprite>();
    List<GameObject> spawnedElements = new List<GameObject>();
    CharacterAssets characterAssets;

    int selectedIndex;
    int selectedElementIndex;

    
    private void Start() 
    {   
        templateRect = template.GetComponent<RectTransform>().rect;

        ClearChildren();
        LoadAssets();
        SpawnElements();
        AdjustPositionOfAllElements();
        SetStartValue();

        int numberOfAssets = characterAssets.GetCharacterAssetItems().Length;
        characterModelSwitcher.AddModels(numberOfAssets,
        new int[2]{(selectedIndex-1+numberOfAssets)%numberOfAssets,(selectedIndex-2+numberOfAssets)%numberOfAssets},
        new int[2]{(selectedIndex+1)%numberOfAssets,(selectedIndex+2)%numberOfAssets});
    }

    public void SetCharacter(Characters characters)
    {
        character  = characters;
    }

    void ClearChildren()
    {
        RectTransform[] rectTransforms = transform.GetComponentsInChildren<RectTransform>();

        for (int i = 0; i<rectTransforms.Length;i++)
        {
            if (rectTransforms[i]!=template.GetComponent<RectTransform>() && rectTransforms[i] !=GetComponent<RectTransform>())
                Destroy(rectTransforms[i].gameObject);
        }
    }

    void LoadAssets()
    {   
        if (character == Characters.Child)
            characterAssets = GameData.GetData<PlayerData>("Child").characterAssets;
        else
            characterAssets = GameData.GetData<PlayerData>("Parent").characterAssets;

       CharacterAssetItem[] characterAssetItems =  characterAssets.GetCharacterAssetItems();
       for (int i= 0; i<characterAssetItems.Length;i++)
       {
            sprites.Add(characterAssetItems[i].characterPortrait);
       }
    }

    void SpawnElements()
    {
        int amountOfElements = 0;
        int index = 0;
        int numberOfElements = maxElementsInARow+2;
        selectedElementIndex =  numberOfElements/2;
        
        while (amountOfElements<numberOfElements)
        {
            GameObject spawnedElement = SpawnElement(index);
            spawnedElements.Add(spawnedElement);

            if (amountOfElements==selectedElementIndex)
            {
                selectedIndex = index;
                ResizeSelectedElement(selectedElementIndex);
            }
                
            amountOfElements++;
            index = amountOfElements % (characterAssets.GetCharacterAssetItems().Length);
         }

         template.SetActive(false);
        
    }

    void SetStartValue()
    {
        int distance = selectedIndex-characterAssets.GetActivePrefabIndex();
        while (selectedIndex!=characterAssets.GetActivePrefabIndex())
        {   
            Slide(templateRect.width);
            spawnedElements[selectedElementIndex].transform.localScale = Vector3.one; 
            SlideRightOperations();
            AdjustPositionOfAllElements();
        }
    }

    GameObject SpawnElement(int index)
    {
        GameObject spawnedElement = Instantiate(template); 

        spawnedElement.transform.SetParent(gameObject.transform);

        spawnedElement.name = characterAssets.GetCharacterAssetItemAt (index).characterPortrait.name;
        spawnedElement.GetComponent<Image>().sprite = sprites[index];
        spawnedElement.transform.localScale = template.transform.localScale;

        return spawnedElement;
    }

    public void Slide (Step step)
    {
        //Reset IncreasedSizeOfSelectedElement
        ResizeSelectedElement(selectedElementIndex);  

        if (step == Step.Next)
            SlideLeft();
        else
            SlideRight();
    }

    void SlideLeft()
    {
        StartCoroutine(_SlideLeft());
        characterModelSwitcher.Slide(Step.Next);
    }

    IEnumerator _SlideLeft()
    {
        yield return Slide(-templateRect.width);
        ResizeSelectedElement(selectedElementIndex);
        SlideLeftOperations();
        yield return null;
        AdjustPositionOfAllElements();
    }

    void SlideLeftOperations()
    {
        //Remove first item
        GameObject firstElement = spawnedElements[0];
        spawnedElements.Remove(firstElement);
        Destroy (firstElement);
        
        //Add last item
        int numberOfAssets = characterAssets.GetCharacterAssetItems().Length;
        int index = (selectedIndex+3)%numberOfAssets;
        GameObject lastElement = SpawnElement(index);
        spawnedElements.Add(lastElement);

        //Update selectedIndex
        selectedIndex = (selectedIndex+1)%numberOfAssets;

        //Increase size of selected object
        ResizeSelectedElement(selectedElementIndex);
    }

    void SlideRight()
    {
        StartCoroutine(_SlideRight());
        characterModelSwitcher.Slide(Step.Prev);
    }

    IEnumerator _SlideRight()
    {
        yield return Slide(templateRect.width);
        ResizeSelectedElement(selectedElementIndex);
        SlideRightOperations();
        yield return null;
        AdjustPositionOfAllElements();
    }

    void SlideRightOperations()
    {
        //Remove last item
        GameObject lastItem = spawnedElements[spawnedElements.Count-1];
        spawnedElements.Remove(lastItem);
        Destroy (lastItem);
        
       //Add first item
        int numberOfAssets = characterAssets.GetCharacterAssetItems().Length;
        int index = (selectedIndex-3 + numberOfAssets*3) %numberOfAssets;
        GameObject firstElement = SpawnElement(index);
        spawnedElements.Insert(0,firstElement);

        //Update SelectedIndex
        selectedIndex = (selectedIndex-1)%numberOfAssets;

        //Increase size of selected object
        ResizeSelectedElement(selectedElementIndex);
    }

    IEnumerator Slide(float xtargetPos)
    {
        float currentPos = 0;
        float startTime = Time.time;
        float targetPos = xtargetPos;

        while (Mathf.Abs(currentPos-targetPos) > 0.01f)
        {
            float t = (Time.time - startTime) /(1/slideSpeed);
            currentPos = Mathf.Lerp(currentPos, targetPos, t);
            AdjustPositionOfAllElements(currentPos);
            yield return null;
        }
    }

    void ResizeSelectedElement(int elementIndex)
    {
        StartCoroutine(_ResizeSelectedElement(elementIndex));
    }

    IEnumerator _ResizeSelectedElement(int elementIndex)
    {   
        Vector3 currentScale = spawnedElements[selectedElementIndex].transform.localScale;
        Vector3 targetScale = Vector3.one;
  
        if (Mathf.Abs(currentScale.x-selectedItemScale)>0.01f)
            targetScale = new Vector3(selectedItemScale,selectedItemScale,0);
        
        float startTime = Time.time;
        
        while (Vector3.Distance(currentScale,targetScale) > 0.01f)
        {
            float t = (Time.time - startTime) /(1/selectedElementScaleSpeed);
            currentScale = Vector3.Lerp(currentScale, targetScale, t);
            spawnedElements[elementIndex].transform.localScale = currentScale;
            yield return null;
        }
    }

    public void AdjustPositionOfAllElements(float xOffset=0)
    {
        GameObject[] shownElements=spawnedElements.ToArray();

        //Calculate Padding
        float totalWidthOfAllElements = shownElements.Length*templateRect.width;
        float totalWidthOfParent = GetComponent<RectTransform>().rect.width;
        float padding = (totalWidthOfParent-totalWidthOfAllElements)/2;

        Vector2 position = template.GetComponent<RectTransform>().anchoredPosition;
        position.x +=xOffset+paddingSide;

        for (int i= 0; i<shownElements.Length;i++)
        {
            shownElements[i].SetActive(true);          
            shownElements[i].GetComponent<RectTransform>().anchoredPosition  = position;
            position.x += shownElements[i].GetComponent<RectTransform>().rect.width+paddingSide;
        }
    }

    public void HideSelection()
    {
        for (int i =0; i<spawnedElements.Count;i++)
        {
            spawnedElements[i].SetActive(false);
        }

        characterModelSwitcher.HideAll();
        Destroy(this);
    }

}
