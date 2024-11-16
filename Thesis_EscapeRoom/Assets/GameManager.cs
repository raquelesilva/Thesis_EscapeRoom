using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    [SerializeField] List<Stats> stats;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        SettingsManager sm = FindObjectOfType<SettingsManager>();
        if(sm != null)
        {
            sm.LoadSettings();
        }
    }

    public void SetStat(string statName, int increase)
    {
        int statIndex = -1;
        statIndex = stats.FindIndex(stat => statName.ToLower() == stat.statName.ToLower());

        if(statIndex != -1)
        {
            stats[statIndex].statCount += increase;
        }
        else
        {
            Stats newStat = new Stats(statName);
            newStat.statCount += increase;
            stats.Add(newStat);
        }
    }
}

[System.Serializable] 
public class Stats
{
    public string statName;
    public int statCount;

    public Stats(string statName)
    {
        this.statName = statName;
        this.statCount = 0;
    }
}
