using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollSelection : MonoBehaviour
{
    [SerializeField] int maxElementsInARow;
    [SerializeField] GameObject portraitTemplate;
    List <Sprite> sprites = new List<Sprite>();
    CharacterAssets characterAssets;
    List<GameObject> spawnedElements = new List<GameObject>();

    int selectedIndex;
    int selectedElementIndex;


    private void Start() 
    {
        LoadSprites();
        //SpawnSprites();
        //UpdateShownElements();
        SpawnElements();
        AdjustPositionOfAllElements();
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
                //spawnedElement.transform.localScale = Vector3.one*1.5f;
            }
                
            amountOfElements++;
            index = amountOfElements % (characterAssets.GetCharacterAssetItems().Length);
         }

         portraitTemplate.SetActive(false);
        
    }

    void SetStartValue()
    {
        int distance = selectedIndex-characterAssets.GetActivePrefabIndex();
    }

    GameObject SpawnElement(int index)
    {
        GameObject spawnedElement = Instantiate(portraitTemplate); 

        spawnedElement.transform.SetParent(gameObject.transform);

        spawnedElement.name = characterAssets.GetCharacterAssetItemAt (index).characterPortrait.name;
        spawnedElement.GetComponent<Image>().sprite = sprites[index];
        spawnedElement.transform.localScale = portraitTemplate.transform.localScale;

        return spawnedElement;
    }

    public void SlideLeft()
    {
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one;
             /* //Remove first item
        GameObject firstElement = spawnedElements[0];
        spawnedElements.Remove(firstElement);
        Destroy (firstElement);
        
        //Add last item
        int numberOfAssets = characterAssets.GetCharacterAssetItems().Length;
        int index = (selectedIndex+1)%numberOfAssets;

        GameObject lastElement = SpawnElement(index);
        spawnedElements.Insert(0,lastElement);

        selectedIndex = (selectedIndex-1)%numberOfAssets;
        if (selectedIndex<0)
            selectedIndex+=numberOfAssets;


        //spawnedElements[selectedElementIndex].transform.localScale = Vector3.one*1.5f;

        AdjustPositionOfAllElements();*/

        
    }

    public void SlideRight()
    {
        //spawnedElements[selectedElementIndex].transform.localScale = Vector3.one*1.5f;
        StartCoroutine(MoveElementsRight());
    }

    IEnumerator MoveElementsRight()
    {
        float currentPos = 0;
        float startTime = Time.time;
        float targetPos = 400f;

        while (Mathf.Abs(currentPos-targetPos) > 0.01f)
        {
            float t = (Time.time - startTime) / 0.4f;
            currentPos = Mathf.Lerp(currentPos, targetPos, t);
            AdjustPositionOfAllElements(currentPos);
            yield return null;
        }

        //AdjustPositionOfAllElements(currentPos);
        
        spawnedElements[selectedElementIndex].transform.localScale = Vector3.one;

       //Remove last item
        GameObject lastItem = spawnedElements[spawnedElements.Count-1];
        spawnedElements.Remove(lastItem);
        Destroy (lastItem);
        
       //Add first item
        int numberOfAssets = characterAssets.GetCharacterAssetItems().Length;
        int index = (selectedIndex-3 + numberOfAssets*3) %numberOfAssets;

        GameObject firstElement = SpawnElement(index);
        spawnedElements.Insert(0,firstElement);

        selectedIndex = (selectedIndex-1)%numberOfAssets;

        AdjustPositionOfAllElements();

        //spawnedElements[selectedElementIndex].transform.localScale = Vector3.one*1.5f;

    }



    GameObject[] GetShownElements()
    {
        GameObject[] shownElements = new GameObject[maxElementsInARow+2];

        int activePrefabIndex = characterAssets.GetActivePrefabIndex();

        int totalElements = spawnedElements.Count;

        int elementsBefore = (maxElementsInARow)/2;
        int elementsAfter = maxElementsInARow-elementsBefore-1;

        //Before (not visible)
        int beforeIndex = CalculateNewIndex(activePrefabIndex,totalElements,-elementsBefore-1);
        shownElements[0] = spawnedElements[beforeIndex];

        //Before
        for (int i= 0; i<=elementsBefore;i++)
        {   
           int index = CalculateNewIndex(activePrefabIndex,totalElements,-(i+1));
           shownElements[i+1] = spawnedElements[index];
        }

        //Active Element
        shownElements[elementsBefore+1] = spawnedElements[activePrefabIndex];

        //After
        for (int i = 1; i < elementsAfter; i++)
        {   
            int index = CalculateNewIndex(activePrefabIndex, totalElements, i+1);
            shownElements[elementsBefore + i+2] = spawnedElements[index];
        }

        //After (not visible)
        int afterIndex = CalculateNewIndex(activePrefabIndex,totalElements,elementsAfter+1);
        shownElements[shownElements.Length-1] = spawnedElements[afterIndex];

        for (int i = 0; i<shownElements.Length;i++)
        {
            Debug.Log (shownElements[i]+ " "+i);
        }

        return shownElements;
    }

    int CalculateNewIndex(int activeIndex,int totalElements,int steps)
    {
        int newIndex = (activeIndex + steps) % totalElements;

        if (newIndex < 0)
            newIndex += totalElements;

            return newIndex;
    }

    public void AdjustPositionOfAllElements(float xOffset=0)
    {
        GameObject[] shownElements=spawnedElements.ToArray();

        //Calculate Padding
        float totalWidthOfAllElements = shownElements.Length*shownElements[0].GetComponent<RectTransform>().rect.width;
        float totalWidthOfParent = GetComponent<RectTransform>().rect.width;
        float padding = (totalWidthOfParent-totalWidthOfAllElements)/2;

        Vector2 position = portraitTemplate.GetComponent<RectTransform>().anchoredPosition;
        position.x -= portraitTemplate.GetComponent<RectTransform>().rect.width;
        position.x +=+xOffset;
        //position.x+=padding;

        for (int i= 0; i<shownElements.Length;i++)
        {
            shownElements[i].SetActive(true);          
            shownElements[i].GetComponent<RectTransform>().anchoredPosition  = position;
            position.x += shownElements[i].GetComponent<RectTransform>().rect.width;
        }
    }

}
