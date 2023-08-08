using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    None,Mom, Child
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/PlayerAgent")]
public class PlayerAgent : AData
{
    public string score;
    public Character character;

    public string inputDevice;
}
