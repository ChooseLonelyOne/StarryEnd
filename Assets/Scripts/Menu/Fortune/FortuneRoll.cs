using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FortuneRoll : MonoBehaviour
{
    [SerializeField] private HeaderPanel header;
    [SerializeField] private DailyReward dailyReward;

    public TextMeshProUGUI dailyText;
    public TextMeshProUGUI freeText;

    public RectTransform Arrow;
    [SerializeField] private BonusItem[] BonusItems;
    private int choice;
    private bool roll = false;

    private AdmobRewardedAd rewardAd;
    private StatsDatabase stats;

    private void Start()
    {
        stats = GameManager.Instance.data;
        rewardAd = GameManager.Instance.rewardedAd;
        rewardAd.OnAdRewarded += StatusReward;
        rewardAd.OnAdLoaded += RewardisLoaded;
    }

    public void ChoiceRandom()
    {
        if (!roll)
        {
            rewardAd.ShowRewardBasedAd();
        }
    }

    public void StatusReward(bool reward)
    {
        if (reward)
        {
            RollWheel(false);
            header.closeFortuneButton.gameObject.SetActive(true);
        }
        else
            header.closeFortuneButton.gameObject.SetActive(true);
    }

    private void RewardisLoaded(bool load)
    {
        header.closeFortuneButton.gameObject.SetActive(load);
    }

    private void RollWheel(bool daily)
    {
        choice = ProbabilityController.ChoiceRandom(BonusItems);
        StartCoroutine(
            RotateToAngle(
            new Vector3(0, 0, -1),
            -Arrow.rotation.eulerAngles.z,
            -Arrow.rotation.eulerAngles.z + (360 + Arrow.rotation.eulerAngles.z) + 360 * 3 + BonusItems[choice].Angle,
            10,
            daily
            ));
    }

    public void ChoiceRandomDaily()
    {
        if (!roll)
        {
            RollWheel(true);
        }
    }

    IEnumerator RotateToAngle(Vector3 rotateAxis, float currentAngle, float targetAngleValue, float speed, bool daily)
    {
        header.closeFortuneButton.gameObject.SetActive(false);
        roll = true;
        if (daily)
        {
            dailyText.gameObject.SetActive(true);
            freeText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }

        var itemSoundAngle = currentAngle + (360 / BonusItems.Length);
        while (true)
        {
            var step = ((targetAngleValue - currentAngle) + speed) * Time.deltaTime;
            if (currentAngle + step > targetAngleValue)
            {
                step = targetAngleValue - currentAngle;
                Arrow.Rotate(rotateAxis, step);
                break;
            }
            currentAngle += step;
            if (currentAngle >= itemSoundAngle)
            {
                itemSoundAngle = currentAngle + (360 / BonusItems.Length);
            }
            Arrow.Rotate(rotateAxis, step);

            yield return null;
        }
        stats.Emerald += BonusItems[choice].Amount;
        SaveManager.Instance.Save();
        print($"{BonusItems[choice].Angle} angles, +{BonusItems[choice].Amount} emeralds. Total: {stats.Emerald}");
        roll = false;
        if (daily)
        {
            yield return new WaitForSeconds(1f);
            dailyReward.DesaCtivateReward();
            header.FortuneHeader(true);
            yield break;
        }
        header.closeFortuneButton.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        rewardAd.OnAdRewarded -= StatusReward;
        rewardAd.OnAdLoaded -= RewardisLoaded;
    }
}
