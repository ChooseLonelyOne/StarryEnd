using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatsDatabase : ScriptableObject, ISaveForSO
{
    public Action OnEmeraldChanged;
    public void Load(ScriptableObject copy)
    {
        if (copy.GetType() != typeof(StatsDatabase))
        {
            Debug.Log("Error. Type of SO != StatsDatabase");
            return;
        }
        StatsDatabase data = (StatsDatabase)copy;
        noAds = data.noAds;
        emerald = data.emerald;
        deaths = data.deaths;
        jumps = data.jumps;
        tackles = data.tackles;
        totalTime = data.totalTime;
    }

    public bool noAds = false;

    [SerializeField] private int emerald = 0;
    public int Emerald
    {
        get { return emerald; }
        set
        {
            emerald = value;
            OnEmeraldChanged?.Invoke();
        }
    }

    [SerializeField] private int deaths;
    public int Deaths
    {
        get { return deaths; }
        set { deaths = value; }
    }

    [SerializeField] private int jumps;
    public int Jumps
    { 
        get { return jumps; }
        set { jumps = value; }
    }

    [SerializeField] private int tackles;
    public int Tackles
    { 
        get { return tackles; }
        set { tackles = value; }
    }

    [SerializeField] private float totalTime;
    public float TotalTime 
    { 
        get { return totalTime; }
        set { totalTime = value; }
    }

}
