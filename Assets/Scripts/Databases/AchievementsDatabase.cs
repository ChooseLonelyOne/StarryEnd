using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class AchievementsDatabase : ScriptableObject, ISaveForSO
{
    public List<AchievementCompleteLevel> achievementList;
    public List<AchievementCount> achievementCount;
    public void Load(ScriptableObject copy)
    {
        if (copy.GetType() != typeof(AchievementsDatabase))
        {
            Debug.Log("Error. Type of SO != LevelsDatabase");
            return;
        }
        AchievementsDatabase data = (AchievementsDatabase)copy;
        for (int i = 0; i < achievementList.Count; i++)
        {
            if (data.achievementList.Count - 1 < i)
            {
                Debug.Log("Copy achievementsList list is end");
                break;
            }
            achievementList[i] = data.achievementList[i];
        }
        for (int i = 0; i < achievementCount.Count; i++)
        {
            if (data.achievementCount.Count - 1 < i)
            {
                Debug.Log("Copy achievementsCount list is end");
                break;
            }
            achievementCount[i] = data.achievementCount[i];
        }
    }
}

[Serializable]
public class AchievementCompleteLevel
{
    [TextArea(0, 30)]
    public string description;
    public Type type = Type.Complete;
    public bool completeAchiev;
    public bool completeLevel;
    public int level;
}

[Serializable]
public class AchievementCount
{
    [TextArea(0, 30)]
    public string description;
    public Type type;
    public bool completeAchiev;
    public int count;
}

[Serializable]
public enum Type
{
    CountEmeralds,
    CountJumps,
    CountTackles,
    CountTime,
    CountDeath,
    Complete
}

