using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatReporter : MonoBehaviour
{
    [SerializeField] private string statName;

    public void IncreaseStat()
    {
        GameManager.instance.SetStat(statName, 1);
    }

    public void DecreaseStat()
    {
        GameManager.instance.SetStat(statName, -1);
    }
}
