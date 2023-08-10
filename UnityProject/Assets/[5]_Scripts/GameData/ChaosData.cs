using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADataObject", menuName = "Custom/AData/ChaosData")]
public class ChaosData : AData
{
    [SerializeField] private int targetChaos = 100;
    [SerializeField] private int currentChaos;

    /// <summary>
    /// Gets the raw values for chaos.
    /// </summary>
    public int CurrentChaosRaw { get => currentChaos; }

    /// <summary>
    /// Gets the values for the current chaos between 0 and 1.
    /// </summary>
    public float CurrentChaos{ get => (float)currentChaos/targetChaos;}

    /// <summary>
    /// Returns true if the chaos counter is maxed out
    /// </summary>
    public bool EndGame { get => CurrentChaosRaw >= targetChaos; }

    public int ModifyChaos(int i)
    {
        currentChaos += i;
        currentChaos = Mathf.Clamp(currentChaos, 0, targetChaos);
        return CurrentChaosRaw;
    }

    /// <summary>
    /// Resets Values to the default Values
    /// </summary>
    public void ResetValues()
    {
        currentChaos = 0;
    }
}
