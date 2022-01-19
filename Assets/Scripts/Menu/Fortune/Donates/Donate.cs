using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donate : MonoBehaviour
{
    private StatsDatabase data;
    private void Start()
    {
        data = GameManager.Instance.data;
    }

    public void EmeraldSmall()
    {
        data.Emerald += 300;
        SaveManager.Instance.Save();
    }
    public void EmeraldMedium()
    {
        data.Emerald += 600;
        SaveManager.Instance.Save();
    }
    public void EmeraldLarge()
    {
        data.Emerald += 1000;
        SaveManager.Instance.Save();
    }
    public void NoAds()
    {
        data.noAds = true;
        data.Emerald += 1000;
        SaveManager.Instance.Save();
    }
}
