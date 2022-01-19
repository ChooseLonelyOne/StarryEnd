using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsList : MonoBehaviour
{
    [SerializeField] private Transform exampleAchievPanel;

    private AchievementsDatabase achievsData;
    private StatsDatabase statsData;
    private LevelsDatabase levelsData;

    private readonly Color completeAchiev = new Color(1, 1, 1, .65f);
    private readonly Color notCompleteAchiev = new Color(1, 1, 1, 1);
    void Start()
    {
        achievsData = GameManager.Instance.achievsData;
        statsData = GameManager.Instance.data;
        levelsData = GameManager.Instance.levelsdata;
        UpdateAchievementsDatabase();
    }

    private void UpdateAchievementsDatabase()
    {
        foreach (AchievementCount achiev in achievsData.achievementCount)
        {
            switch (achiev.type)
            {
                case Type.CountEmeralds:
                    if (statsData.Emerald >= achiev.count && !achiev.completeAchiev)
                        achiev.completeAchiev = true;
                    break;

                case Type.CountJumps:
                    if (statsData.Jumps >= achiev.count)
                        achiev.completeAchiev = true;
                    break;

                case Type.CountTackles:
                    if (statsData.Tackles >= achiev.count)
                        achiev.completeAchiev = true;
                    break;

                case Type.CountTime:
                    if (statsData.TotalTime >= achiev.count)
                        achiev.completeAchiev = true;
                    break;

                case Type.CountDeath:
                    if (statsData.Deaths >= achiev.count)
                        achiev.completeAchiev = true;
                    break;
            }
        }

        for (int i = 0; i < achievsData.achievementList.Count; i++)
        {
            achievsData.achievementList[i].completeLevel = levelsData.levels[i].complete;
            if (achievsData.achievementList[i].completeLevel)
            {
                achievsData.achievementList[i].completeAchiev = true;
            }
        }
        SpawnPanels();
    }

    private void SpawnPanels()
    {
        foreach (AchievementCompleteLevel achiev in achievsData.achievementList)
        {
            Transform ach = Instantiate(exampleAchievPanel, transform);
            TextMeshProUGUI countText = ach.GetChild(1).GetComponent<TextMeshProUGUI>();
            Toggle toggleComplete = ach.GetChild(2).GetComponent<Toggle>();

            ach.GetChild(0).GetComponent<TextMeshProUGUI>().text = achiev.description + " " + (achiev.level + 1);
            ach.gameObject.SetActive(true);

            countText.gameObject.SetActive(false);
            toggleComplete.gameObject.SetActive(true);

            if (achiev.completeAchiev)
            {
                toggleComplete.isOn = true;
                ach.GetComponent<Image>().color = completeAchiev;
                continue;
            }

            ach.GetComponent<Image>().color = notCompleteAchiev;
            toggleComplete.isOn = levelsData.levels[achiev.level].complete;
        }


        foreach (AchievementCount achiev in achievsData.achievementCount)
        {
            Transform ach = Instantiate(exampleAchievPanel, transform);
            TextMeshProUGUI countText = ach.GetChild(1).GetComponent<TextMeshProUGUI>();
            Toggle toggleComplete = ach.GetChild(2).GetComponent<Toggle>();

            ach.GetChild(0).GetComponent<TextMeshProUGUI>().text = achiev.description;
            ach.gameObject.SetActive(true);

            countText.gameObject.SetActive(true);
            toggleComplete.gameObject.SetActive(false);

            if (achiev.completeAchiev)
            {
                ach.GetComponent<Image>().color = completeAchiev;
                countText.text = string.Format("{0} / {1}", achiev.count, achiev.count);
                continue;
            }

            switch (achiev.type)
            {
                case Type.CountEmeralds:
                    countText.text = string.Format("{0} / {1}", statsData.Emerald, achiev.count);
                    break;
                case Type.CountJumps:
                    countText.text = string.Format("{0} / {1}", statsData.Jumps, achiev.count);
                    break;
                case Type.CountTackles:
                    countText.text = string.Format("{0} / {1}", statsData.Tackles, achiev.count);
                    break;
                case Type.CountTime:
                    countText.text = string.Format("{0} / {1}", statsData.TotalTime, achiev.count);
                    break;
                case Type.CountDeath:
                    countText.text = string.Format("{0} / {1}", statsData.Deaths, achiev.count);
                    break;
            }
            ach.GetComponent<Image>().color = notCompleteAchiev;
        }
    }
}
