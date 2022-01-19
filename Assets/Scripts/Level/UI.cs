using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Controls Buttons")]
    public Transform left;
    public Transform right;
    public Transform up;
    public Transform down;

    [Header("DeadButtons")]
    [SerializeField] private Transform restartButtonInDead;
    [SerializeField] private Transform retryForAdButton;
    [SerializeField] private Transform retryForMoneyButton;

    [Header("PauseButtons")]
    [SerializeField] private Transform pauseButton;
    [SerializeField] private Transform continueButton;
    [SerializeField] private Transform menuButtonInPause;

    [Header("EndLevelButtons")]
    [SerializeField] private Transform nextLevelButton;
    [SerializeField] private Transform menuButtonInEndLevel;
    [SerializeField] private Transform restartButtonInEndLevel;
    [SerializeField] private Transform newRecordText;
    [SerializeField] private Transform unlockNewLevel;
    [SerializeField] private Transform getRewardText;

    [Header("GameObjects")]
    [SerializeField] private Player player;
    [SerializeField] private Transform deadPanel;
    [SerializeField] private Transform pausePanel;
    [SerializeField] private Transform endLevelPanel;
    [SerializeField] private Transform controlers;

    [SerializeField] private int priceForRetry;
    [SerializeField] private int deathAd = 5;

    private readonly Vector3 pressed = new Vector3(0.8f, 0.8f, 0.8f);
    private readonly Vector3 notPressed = new Vector3(1f, 1f, 1f);
    private readonly Color colorPressed = new Color(1f, 1f, 1f, .7f);
    private readonly Color colorNotPressed = new Color(1f, 1f, 1f, .45f);

    public int countOfDeath = 1;
    private AdmobRewardedAd rewardAdmob;
    private AdmobInterstitialAd interstitialAdmob;
    private LevelsDatabase levelsData;
    private StatsDatabase statsData;

    private void Start()
    {
        statsData = GameManager.Instance.data;
        levelsData = GameManager.Instance.levelsdata;
        interstitialAdmob = GameManager.Instance.interstitialAd;
        rewardAdmob = GameManager.Instance.rewardedAd;

        rewardAdmob.OnAdRewarded += StatusReward;
        rewardAdmob.OnAdLoaded += RewardIsLoaded;

        player.OnDead += Dead;
        player.OnEndLevel += EndLevel;
        for (var i = 0; i < levelsData.levels.Count; i++)
        {
            if (levelsData.levels[i].name == SceneTransition.CurrentScene())
            {
                levelsData.choiceLevel = i;
            }
        }
    }

    #region Dead
    private void Dead()
    {
        StartCoroutine(ShowDeadPanel());
    }

    private IEnumerator ShowDeadPanel()
    {
        player.rb.bodyType = RigidbodyType2D.Static;
        controlers.gameObject.SetActive(false);
        var continueText = retryForMoneyButton.GetChild(0).transform;
        var continueEmerald = continueText.transform.GetChild(0).transform;

        continueText.GetComponent<TextMeshProUGUI>().text = "CONTINUE x " + (priceForRetry * countOfDeath);
        continueEmerald.localPosition = new Vector3(
            continueText.GetComponent<TextMeshProUGUI>().preferredWidth / 2 + continueEmerald.GetComponent<RectTransform>().rect.width / 2 + 10,
            0,
            0);

        retryForMoneyButton.GetComponent<Button>().interactable = true;
        if (statsData.Emerald < priceForRetry * countOfDeath)
        {
            retryForMoneyButton.GetComponent<Button>().interactable = false;
            //print("interactable false");
        }
        SaveManager.Instance.Save();

        yield return new WaitForSeconds(1f);

        if (player.selectedPoint < 1)
        {
            StopWatch.DefaultTime();
            StartCoroutine(AfterDead(0));
            yield break;
        }
        deadPanel.gameObject.SetActive(true);
    }

    private IEnumerator AfterDead(int point)
    {
        CountAttempts();
        controlers.gameObject.SetActive(true);
        AllButtonsUp();
        deadPanel.gameObject.SetActive(false);
        player.transform.position = player.spawnPoint[point].position;
        player.playerGo.GetComponent<Animator>().SetTrigger("isRespawn");
        player.playerGo.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(.5f);

        player.animator.SetBool("onDead", false);
        player.animator.SetBool("onWall", false);
        player.animator.SetBool("onLedge", false);
        player.animator.SetBool("onGround", true);
        player.animator.SetBool("onTackle", false);
        player.animator.SetFloat("moveX", 0f);
        player.playerGo.localScale = new Vector3(0, 1, 1);
        player.rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(.1f);

        player.StartCheckMoves();
    }
    #endregion

    #region ControlButtons
    public void LeftButtonDown()
    {
        left.localScale = pressed;
        left.GetComponent<Image>().color = colorPressed;
        player.left = true;
    }

    public void LeftButtonUp()
    {
        left.localScale = notPressed;
        left.GetComponent<Image>().color = colorNotPressed;
        player.left = false;
    }

    public void RightButtonDown()
    {
        right.localScale = pressed;
        right.GetComponent<Image>().color = colorPressed;
        player.right = true;
    }

    public void RightButtonUp()
    {
        right.localScale = notPressed;
        right.GetComponent<Image>().color = colorNotPressed;
        player.right = false;
    }

    public void UpButtonDown()
    {
        up.localScale = pressed;
        up.GetComponent<Image>().color = colorPressed;
        player.up = true;
    }

    public void UpButtonUp()
    {
        up.localScale = notPressed;
        up.GetComponent<Image>().color = colorNotPressed;
        player.up = false;
    }

    public void DownButtonDown()
    {
        down.localScale = pressed;
        down.GetComponent<Image>().color = colorPressed;
        player.down = true;
    }

    public void DownButtonUp()
    {
        down.localScale = notPressed;
        down.GetComponent<Image>().color = colorNotPressed;
        player.down = false;
    }

    private void AllButtonsUp()
    {
        down.localScale = notPressed;
        down.GetComponent<Image>().color = colorNotPressed;
        up.localScale = notPressed;
        up.GetComponent<Image>().color = colorNotPressed;
        right.localScale = notPressed;
        right.GetComponent<Image>().color = colorNotPressed;
        left.localScale = notPressed;
        left.GetComponent<Image>().color = colorNotPressed;
    }
    #endregion

    #region RestartButtons
    public void RetryForMoneyButton()
    {
        if (statsData.Emerald >= (priceForRetry * countOfDeath))
        {
            statsData.Emerald -= priceForRetry * countOfDeath;
            StartCoroutine(AfterDead(player.selectedPoint));
            countOfDeath++;
            SaveManager.Instance.Save();
        }
    }

    public void RetryForAdButton()
    {
        rewardAdmob.ShowRewardBasedAd();
    }

    private void StatusReward(bool reward)
    {
        if (!reward)
        {
            retryForAdButton.GetComponent<Button>().interactable = false;
            print("Can't get reward");
            return;
        }
        StartCoroutine(AfterDead(player.selectedPoint));
        retryForAdButton.GetComponent<Button>().interactable = false;
        deadPanel.gameObject.SetActive(false);
    }

    private void RewardIsLoaded(bool load)
    {
        retryForAdButton.GetComponent<Button>().interactable = load;
    }

    private void CountAttempts()
    {
        if (GameManager.Instance.countOfAttempts >= deathAd)
        {
            interstitialAdmob.ShowInterstitialAd();
            GameManager.Instance.countOfAttempts = 0;
        }
    }
    #endregion

    #region PauseButtons
    public void PauseButton()
    {
        controlers.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
        player.canMove = false;
        Time.timeScale = 0f;
    }

    public void ContinueButton()
    {
        controlers.gameObject.SetActive(true);
        pausePanel.gameObject.SetActive(false);
        player.canMove = true;
        Time.timeScale = 1f;
    }
    #endregion

    #region EndLevel
    private void EndLevel()
    {
        controlers.gameObject.SetActive(false);
        endLevelPanel.gameObject.SetActive(true);
        getRewardText.GetComponent<TextMeshProUGUI>().text = "+" + levelsData.levels[levelsData.choiceLevel].reward;

        player.rb.velocity = new Vector2(0, 0);
        player.startMove = player.canMove = false;
        StopWatch.StopTime();

        newRecordText.gameObject.SetActive(false);
        if (levelsData.levels[levelsData.choiceLevel].bestTime == 0 || StopWatch.instance.timeStart < levelsData.levels[levelsData.choiceLevel].bestTime)
        {
            print("new record");
            newRecordText.gameObject.SetActive(true);
            levelsData.levels[levelsData.choiceLevel].bestTime = StopWatch.instance.timeStart;
        }

        levelsData.levels[levelsData.choiceLevel].complete = true;
        statsData.Emerald += levelsData.levels[levelsData.choiceLevel].reward;
        print("end");

        unlockNewLevel.gameObject.SetActive(false);
        if (levelsData.choiceLevel + 1 < levelsData.levels.Count)
        {
            if (!levelsData.levels[levelsData.choiceLevel + 1].unlock)
            {
                levelsData.levels[levelsData.choiceLevel + 1].unlock = true;
                unlockNewLevel.GetComponent<TextMeshProUGUI>().text = ($"Unlock new level: {levelsData.choiceLevel + 2}");
                unlockNewLevel.gameObject.SetActive(true);
            }
            nextLevelButton.GetComponent<Button>().interactable = true;
            print("end level");
            return;
        }
        nextLevelButton.GetComponent<Button>().interactable = false;
        SaveManager.Instance.Save();
    }

    public void NextLevelButton()
    {
        SceneTransition.SwitchToScene(levelsData.levels[levelsData.choiceLevel + 1].name);
        SaveManager.Instance.Save();
    }
    #endregion

    #region GeneralMethods
    public void RestartButton()
    {
        endLevelPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
        deadPanel.gameObject.SetActive(false);
        player.selectedPoint = 0;
        StopWatch.DefaultTime();
        StartCoroutine(AfterDead(0));
        countOfDeath = 1;
        SaveManager.Instance.Save();
    }

    public void MenuButton()
    {
        Time.timeScale = 1f;
        player.rb.bodyType = RigidbodyType2D.Static;
        SaveManager.Instance.Save();
        SceneTransition.SwitchToScene("Menu");
    }
    #endregion

    private void OnDestroy()
    {
        player.OnDead -= Dead;
        player.OnEndLevel -= EndLevel;

        rewardAdmob.OnAdRewarded -= StatusReward;
        rewardAdmob.OnAdLoaded -= RewardIsLoaded;
    }
}
