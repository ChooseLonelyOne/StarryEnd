using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Instance
    public static SaveManager Instance;
    private void Awake()
    {
        copyLevelsData = ScriptableObject.CreateInstance<LevelsDatabase>();
        copyStatsData = ScriptableObject.CreateInstance<StatsDatabase>();
        copyAchievsData = ScriptableObject.CreateInstance<AchievementsDatabase>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    [Header("Logic")]
    [SerializeField] private string saveFileName = "data.ss";
    [SerializeField] private bool loadOnStart = true;

    [SerializeField] private LevelsDatabase levelsData;
    [SerializeField] private StatsDatabase statsData;
    [SerializeField] private AchievementsDatabase achievsData;
    private LevelsDatabase copyLevelsData;
    private StatsDatabase copyStatsData;
    private AchievementsDatabase copyAchievsData;

    private void Start()
    {
        if (loadOnStart)
        {
            Load();
        }
    }

    private void OnDisable()
    {
        Save();
    }

    public void Load()
    {
        LoadLevelsData();
        LoadStatsData();
        LoadAchievsData();
        /*for (int i = 0; i < objectsToPersist.Count; i++)
        {
            if (objectsToPersist[i].GetType() == typeof(LevelsDatabase))
            {
                LoadLevelsData();
            }
        }*/
    }

    public void Save()
    {
        SaveLevelsData();
        SaveStatsData();
        SaveAchievsData();
        /*for (int i = 0; i < objectsToPersist.Count; i++)
        {
            if (objectsToPersist[i].GetType() == typeof(LevelsDatabase))
            {
                SaveLevelsData();
            }
        }*/
    }

    private void LoadLevelsData()
    {
        if (!File.Exists(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, levelsData.name)))
        {
            //do nothing
            print("Can't load");
            return;
        }
        print(levelsData.levels.Count + " before loading, "/* + copyLevelsData.levels.Count + " before loading"*/);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, levelsData.name), FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), copyLevelsData);
        print(levelsData.levels.Count + " after overwrite, " + copyLevelsData.levels.Count + " after overwrite");
        levelsData.Load(copyLevelsData);
        file.Close();
        //print("Load " + objectsToPersist[i].name);
    }
    private void SaveLevelsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, levelsData.name));
        var json = JsonUtility.ToJson(levelsData);
        bf.Serialize(file, json);
        file.Close();
    }

    private void LoadStatsData()
    {
        if (!File.Exists(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, statsData.name)))
        {
            //do nothing
            print("Can't load");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, statsData.name), FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), copyStatsData);
        statsData.Load(copyStatsData);
        file.Close();
        //print("Load " + objectsToPersist[i].name);
    }
    private void SaveStatsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, statsData.name));
        var json = JsonUtility.ToJson(statsData);
        bf.Serialize(file, json);
        file.Close();
    }


    private void LoadAchievsData()
    {
        if (!File.Exists(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, achievsData.name)))
        {
            //do nothing
            print("Can't load");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, achievsData.name), FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), copyAchievsData);
        achievsData.Load(copyAchievsData);
        file.Close();
        //print("Load " + objectsToPersist[i].name);
    }
    private void SaveAchievsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.pso", saveFileName, achievsData.name));
        var json = JsonUtility.ToJson(achievsData);
        bf.Serialize(file, json);
        file.Close();
    }
}
