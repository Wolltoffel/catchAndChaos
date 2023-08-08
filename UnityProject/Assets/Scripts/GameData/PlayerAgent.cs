using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Mom, Child
}

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData")]
public class PlayerAgent : AData
{
    public string score;
    public Character character;
}
