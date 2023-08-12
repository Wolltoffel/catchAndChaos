using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UINavigation : MonoBehaviour
{
    Selectable startObject;

    public Selectable startingSelectable;
    private void Start()
    {
        // Set the starting selectable as the initially selected UI element
        //EventSystem.current.SetSelectedGameObject(startingSelectable.gameObject);
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("A Horizontal");
        float verticalInput = Input.GetAxis("A Vertical");
       // bool bumperLeft = Input.GetButtonDown("Bumper Left");
       // bool bumperRight = Input.GetButtonDown("Bumper Right");

       Debug.Log ("Horizontal: "+horizontalInput);
       Debug.Log ("Vertical: "+verticalInput);

/*        Selectable currentSelectable =  EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

           if (currentSelectable!=null)
           {
                Selectable nextSelectable = null;

                //Left
                if (horizontalInput<0)
                    nextSelectable = currentSelectable.FindSelectableOnLeft();
                
                //Right
                else if (horizontalInput<0)
                    nextSelectable = currentSelectable.FindSelectableOnRight();

                //Up
                else if (verticalInput>0)
                    nextSelectable = currentSelectable.FindSelectableOnUp();

                 //Down
                else if (verticalInput<0)
                    nextSelectable = currentSelectable.FindSelectableOnDown();

                Debug.Log (nextSelectable);

                if (nextSelectable!=null)
                    EventSystem.current.SetSelectedGameObject(nextSelectable.gameObject);
           }*/
    }

}
