using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class LevelsDatabase : ScriptableObject, ISaveForSO
{
    public List<Level> levels;
    public int choiceLevel;

    public void Load(ScriptableObject copy)
    {
        if (copy.GetType() != typeof(LevelsDatabase))
        {
            Debug.Log("Error. Type of SO != LevelsDatabase" + copy.GetType());
            return;
        }
        LevelsDatabase data = (LevelsDatabase)copy;
        //Debug.Log(levels.Count);
        for (int i = 0; i < levels.Count; i++)
        {
            if (data.levels.Count - 1 < i)
            {
                Debug.Log("Copy levels list is end");
                return;
            }
            levels[i] = data.levels[i];
            //Debug.Log("load data " + i);
            continue;
        }
        choiceLevel = 0;
    }
}

[Serializable]
public class Level
{
    public Sprite icon;
    public Difficulty difficulty;
    public string name;
    public float bestTime;
    public bool secret;
    public bool complete;

    [Space(10)]
    public bool unlock;
    public int price;
    public int reward;
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Impossible
}


