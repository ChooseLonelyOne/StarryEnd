using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.EventSystems;

public class AdmobRewardedAd : MonoBehaviour
{
    public Action<bool> OnAdRewarded;
    public Action<bool> OnAdLoaded;
    private RewardBasedVideoAd rewardBasedVideoAd;

    private bool rewarded = false;

    private void Start()
    {
        rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdOpening += HandleOnAdOpening;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;

        LoadRewardBaseAd();
    }

    public void ShowRewardBasedAd()
    {
        if (rewardBasedVideoAd.IsLoaded())
        {
            rewardBasedVideoAd.Show();
        }
        else
        {
            print("Dude, your ad's not loaded yet");
        }
    }

    private void LoadRewardBaseAd()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-1182465837355734/1897325781";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        print("ad loaded");
        OnAdLoaded?.Invoke(true);
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //try a reload
        OnAdLoaded?.Invoke(false);
        LoadRewardBaseAd(); 
        print("failed to load");
    }

    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        //pause the action
        print("ad opening");
    }

    public void HandleOnAdStarted(object sender, EventArgs args)
    {
        //mute audio
        print("ad started");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        //Crank the party back up
        OnAdLoaded?.Invoke(false);
        LoadRewardBaseAd();
        if(!rewarded)
            OnAdRewarded?.Invoke(rewarded);
        rewarded = false;
        print("ad closed");
    }

    public void HandleOnAdRewarded(object sender, Reward args)
    {
        //reward the user
        LoadRewardBaseAd();
        rewarded = true;
        OnAdRewarded?.Invoke(rewarded);
        print("ad rewarded");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        print("ad leaving app");
    }

    private void OnDestroy()
    {
        rewardBasedVideoAd.OnAdClosed -= HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
        rewardBasedVideoAd.OnAdLoaded -= HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdOpening -= HandleOnAdOpening;
        rewardBasedVideoAd.OnAdRewarded -= HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdStarted -= HandleOnAdStarted;
    }
}
