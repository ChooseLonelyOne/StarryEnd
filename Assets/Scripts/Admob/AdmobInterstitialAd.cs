using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdmobInterstitialAd : MonoBehaviour
{
    private InterstitialAd interstitialAd;

    private void Start()
    { 
        RequestInterStitial();

        // Called when an ad request has successfully loaded.
        interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitialAd.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd.IsLoaded() && !GameManager.Instance.data.noAds)
        {
            interstitialAd.Show();
        }
    }

    private void RequestInterStitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-1182465837355734/5155906339";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        interstitialAd = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
    }

    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        print("HandleAdLoaded event received");
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        print("HandleFailedToReceiveAd event received with message: " + e.Message);
        RequestInterStitial();
    }

    private void HandleOnAdOpened(object sender, EventArgs e)
    {
        print("HandleAdOpened event received");
    }

    private void HandleOnAdClosed(object sender, EventArgs e)
    {
        RequestInterStitial();
        print("HandleAdClosed event received");
    }

    private void HandleOnAdLeavingApplication(object sender, EventArgs e)
    {
        print("HandleAdLeavingApplication event received");
    }

    private void OnDestroy()
    {
        // Called when an ad request has successfully loaded.
        interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitialAd.OnAdOpening -= HandleOnAdOpened;
        // Called when the ad is closed.
        interstitialAd.OnAdClosed -= HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitialAd.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
    }
}
