using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeaderPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Transform freeSpin;
    [SerializeField] private Transform emeraldImage;
    [SerializeField] private TextMeshProUGUI emeraldText;
    [SerializeField] private Transform achievementsButton;
    [SerializeField] private Transform settingsButton;
    [SerializeField] private Transform closeButton;

    public Transform closeFortuneButton;
    [SerializeField] private GameObject wheelOfFortune;
    private void Start()
    {
        emeraldText.text = GameManager.Instance.data.Emerald.ToString();
        UpdateEmeraldImagePos();
        GameManager.Instance.data.OnEmeraldChanged += EmeraldText;
    }

    public void EmeraldText()
    {
        StartCoroutine(UpdateEmeraldText());
    }

    private IEnumerator UpdateEmeraldText()
    {
        double firstCount = System.Convert.ToDouble(emeraldText.text);
        double secondCount = GameManager.Instance.data.Emerald;
        if (secondCount - firstCount != 0)
        {/*
            double splitSecondFirst = (secondCount - firstCount) / 10;
            double step;
            for (int i = 1; i <= 10; i++)
            {
                step = splitSecondFirst * i;

                yield return new WaitForSeconds(.15f);

                emeraldText.text = ((int)step + firstCount).ToString();
                UpdateEmeraldImagePos();
            }*/
            double step;
            for (int i = 1; i <= secondCount - firstCount; i++)
            {
                step = firstCount + i;

                yield return new WaitForSeconds(.02f);

                emeraldText.text = step.ToString();
                UpdateEmeraldImagePos();
            }
        }
    }

    private void UpdateEmeraldImagePos()
    {
        emeraldText.GetComponent<RectTransform>().sizeDelta = new Vector2(emeraldText.preferredWidth, 0);
        emeraldText.transform.localPosition = new Vector3(
            emeraldText.preferredWidth + emeraldImage.GetComponent<RectTransform>().rect.width / 1.5f,
            emeraldText.transform.localPosition.y,
            0);
    }

    public void FreeSpin()
    {
        FortuneHeader(false);
    }

    public void CloseFortuneButton()
    {
        wheelOfFortune.GetComponent<FortuneRoll>().Arrow.rotation = new Quaternion(0, 0, 0, 0);
        FortuneHeader(true);
    }

    public void FortuneHeader(bool disactive)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Menu"))
        {
            wheelOfFortune.SetActive(!disactive);
            freeSpin.gameObject.SetActive(disactive);
            achievementsButton.gameObject.SetActive(disactive);
            settingsButton.gameObject.SetActive(disactive);
            //closeButton.gameObject.SetActive(!disactive);
            closeFortuneButton.gameObject.SetActive(!disactive);
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Achievements"))
        {
            wheelOfFortune.SetActive(!disactive);
            freeSpin.gameObject.SetActive(disactive);
            closeButton.gameObject.SetActive(disactive);
            closeFortuneButton.gameObject.SetActive(!disactive);
        }
    }

    public void AchievementsButton()
    {
        SceneTransition.SwitchToScene("Achievements");
    }

    public void SettingsButton()
    {

    }

    public void CloseButton()
    {
        SceneTransition.SwitchToScene("Menu");
    }

    private void OnDestroy()
    {
        GameManager.Instance.data.OnEmeraldChanged -= EmeraldText;
    }
}
