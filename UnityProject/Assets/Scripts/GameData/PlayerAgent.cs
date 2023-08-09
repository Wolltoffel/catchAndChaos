using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    Male, Female
}


public abstract class PlayerAgent : AData
{
    [HideInInspector]public Gender tempGender;
    [HideInInspector]public string tempScore;
    [HideInInspector]public string tempInputDevice;

}
