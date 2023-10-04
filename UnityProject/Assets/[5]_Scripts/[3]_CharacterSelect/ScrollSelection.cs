using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollSelection : MonoBehaviour
{
    [SerializeField] int maxElementsInARow;
    [SerializeField] int paddingSide;
    [SerializeField] GameObject template;
    
    Rect templateRect;
    
    List <Sprite> sprites = new List<Sprite>();
    List<GameObject> spawnedElements = new List<GameObject>();
    CharacterAssets characterAssets;

    int selectedIndex;
    int selectedElementIndex;


    private void Start() 
    {
        templateRect = template.GetComponent<RectTransform>().rect;

        LoadSprites();
        SpawnElements();
        AdjustPositionOfAllElements();
        SetStartValue();
    }

    void LoadSprites()
    {
        characterAssets = GameData.GetData<PlayerData>("Child").characterAssets;
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
                spawnedElement.transform.localScale = Vector3.one*1.2f;
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
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one;    

        if (step == Step.Next)
            _SlideLeft();
        else
            _SlideRight();
    }

    void _SlideLeft()
    {
        StartCoroutine(SlideLeft());
    }

    IEnumerator SlideLeft()
    {
       
        yield return Slide(-templateRect.width);
        SlideLeftOperations();
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
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one*1.2f;
    }

    void _SlideRight()
    {
        StartCoroutine(SlideRight());
    }

    IEnumerator SlideRight()
    {
        yield return Slide(templateRect.width);
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one;
        SlideRightOperations();
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
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one*1.2f;
    }

    IEnumerator Slide(float xtargetPos)
    {
        float currentPos = 0;
        float startTime = Time.time;
        float targetPos = xtargetPos;

        while (Mathf.Abs(currentPos-targetPos) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentPos = Mathf.Lerp(currentPos, targetPos, t);
            AdjustPositionOfAllElements(currentPos);
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
        position.x -= templateRect.width;
        position.x +=xOffset;
        //position.x+=padding;

        for (int i= 0; i<shownElements.Length;i++)
        {
            shownElements[i].SetActive(true);          
            shownElements[i].GetComponent<RectTransform>().anchoredPosition  = position;
            position.x += shownElements[i].GetComponent<RectTransform>().rect.width+paddingSide;
        }
    }

}
