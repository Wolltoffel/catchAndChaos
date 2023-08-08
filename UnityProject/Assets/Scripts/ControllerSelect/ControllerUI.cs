using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControllerUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI[] playerControllerInfo;

    public void controllerSet(int playerIndex, string inputDevice)
    {
        playerControllerInfo[playerIndex-1].text = $"Player {playerIndex}: Input is {inputDevice}";
    }
}
