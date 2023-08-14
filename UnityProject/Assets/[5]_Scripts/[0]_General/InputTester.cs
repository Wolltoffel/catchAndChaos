using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    [SerializeField] string inputToTest;

    void Update()
    {
          if (Input.GetButtonDown(inputToTest))
          {
            Debug.Log ("Input recognised");
          }
    }

}
