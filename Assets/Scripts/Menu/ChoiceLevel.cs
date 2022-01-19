using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoiceLevel : MonoBehaviour
{
    [Header("Info Panel")]
    [SerializeField] private SpriteRenderer backgroundInfo;
    [SerializeField] private TextMeshProUGUI numberInfo;
    [SerializeField] private TextMeshProUGUI difficultyInfo;
    [SerializeField] private TextMeshProUGUI bestTimeInfo;
    [SerializeField] private GameObject blockPanelInfo;
    [SerializeField] private TextMeshProUGUI priceBlockPanelInfo;
    [SerializeField] private Image bigLockInfo;
    [SerializeField] private HeaderPanel header;

    [Header("Buttons")]
    [SerializeField] private Transform leftButton;
    [SerializeField] private Transform rightButton;
    [SerializeField] private Transform playButton;

    private LevelsDatabase levelsData;

    private void Start()
    {
        levelsData = GameManager.Instance.levelsdata;
        ShowLevelInfo(levelsData.choiceLevel);
    }

    private void ShowLevelInfo(int index)
    {
        numberInfo.text =               (index + 1).ToString();
        difficultyInfo.text =           levelsData.levels[index].difficulty.ToString();
        priceBlockPanelInfo.text =      "x" + levelsData.levels[index].price.ToString();
        bestTimeInfo.text =             (levelsData.levels[index].bestTime != 0) ? levelsData.levels[index].bestTime.ToString("F2") : "NONE";

        backgroundInfo.sprite =         levelsData.levels[index].icon;

        switch (levelsData.levels[index].unlock)
        {
            case true:
                playButton.GetComponent<Button>().interactable = true;
                blockPanelInfo.SetActive(false);
                bigLockInfo.gameObject.SetActive(false);
                break;
            case false:
                playButton.GetComponent<Button>().interactable = false;
                blockPanelInfo.SetActive(true);
                bigLockInfo.gameObject.SetActive(true);
                break;
        }
    }

    public void LeftArrowButton()
    {
        if (levelsData.choiceLevel - 1 >= 0)
        {
            levelsData.choiceLevel--;
            ShowLevelInfo(levelsData.choiceLevel);
        }
    }

    public void RightArrowButton()
    {
        if (levelsData.choiceLevel + 1 < levelsData.levels.Count)
        {
            levelsData.choiceLevel++;
            ShowLevelInfo(levelsData.choiceLevel);
        }
    }

    public void BlockPanelInfo()
    {
        if (GameManager.Instance.data.Emerald > levelsData.levels[levelsData.choiceLevel].price)
        {
            GameManager.Instance.data.Emerald -= levelsData.levels[levelsData.choiceLevel].price;
            levelsData.levels[levelsData.choiceLevel].unlock = true;
            SaveManager.Instance.Save();
            ShowLevelInfo(levelsData.choiceLevel);
        }
    }

    public void PlayButton()
    {
        SceneTransition.SwitchToScene(levelsData.levels[levelsData.choiceLevel].name);
    }
}
