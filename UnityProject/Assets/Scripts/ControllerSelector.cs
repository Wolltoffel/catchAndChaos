using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSelector : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return WaitForKeyInput();

        yield return null;
    }

    IEnumerator WaitForKeyInput()
    {
        while (true)
        {
           if (Input.GetButton("J1A"))
                Debug.Log ("Pressed");

            yield return null;
        }


    }
}
