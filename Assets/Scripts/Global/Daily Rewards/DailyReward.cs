using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyReward : MonoBehaviour
{
    [Space]
    [Header("Reward UI")]
    [SerializeField] private GameObject wheelPanel;
    [SerializeField] private HeaderPanel header;

    [Space]
    [Header("Timing")]
    [SerializeField] private double nextRewardDelay;
    [SerializeField] private float checkForRewardsDelay;
    private bool isRewardReady = false;

    private void Start()
    {
        Intialize();

        StopAllCoroutines();
        StartCoroutine(CheckForRewardsDelay());
    }

    private IEnumerator CheckForRewardsDelay()
    {
        while (true)
        {
            if (!isRewardReady)
            {
                DateTime currentDatetime = WorldTimeAPI.Instance.GetCurrentDateTime();
                DateTime rewardClaimDatetime = DateTime.Parse(PlayerPrefs.GetString("Reward_Claim_Datetime", currentDatetime.ToString()));

                double elapsedHours = (currentDatetime - rewardClaimDatetime).TotalHours;

                if (elapsedHours >= nextRewardDelay)
                    ActivateReward();
            }
            yield return new WaitForSeconds(checkForRewardsDelay);
        }
    }

    private void ActivateReward()
    {
        isRewardReady = true;
        header.FortuneHeader(false);
        wheelPanel.GetComponent<FortuneRoll>().ChoiceRandomDaily();
    }

    public void DesaCtivateReward()
    {
        isRewardReady = false;
        header.FortuneHeader(true);
        PlayerPrefs.SetString("Reward_Claim_Datetime", WorldTimeAPI.Instance.GetCurrentDateTime().ToString());
        wheelPanel.GetComponent<FortuneRoll>().dailyText.gameObject.SetActive(false);
        wheelPanel.GetComponent<FortuneRoll>().freeText.gameObject.SetActive(true);
    }

    private void Intialize()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("Reward_Claim_Datetime")))
        {
            PlayerPrefs.SetString("Reward_Claim_Datetime", WorldTimeAPI.Instance.GetCurrentDateTime().ToString());
        }
    }
}
