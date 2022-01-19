using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public StatsDatabase data;
    public LevelsDatabase levelsdata;
    public AchievementsDatabase achievsData;

    public AdmobInterstitialAd interstitialAd;
    public AdmobRewardedAd rewardedAd;
    private void Awake()
    {
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

    public int countOfAttempts = 0;
}
