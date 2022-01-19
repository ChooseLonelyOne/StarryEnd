using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public TextMeshProUGUI loadingPercentage;
    public Image loadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayOpeningAnimation = false;

    private Animator animator;
    private AsyncOperation loadingSceneOperation;

    private AdmobInterstitialAd interstitialAd;

    public static void SwitchToScene(string sceneName)
    {
        instance.animator.SetTrigger("sceneClosing");
        int random = Random.Range(1, 4);
        //print(random);

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        instance.loadingSceneOperation.allowSceneActivation = false;
        if (random == 3)
            instance.interstitialAd.ShowInterstitialAd();
    }

    public static void RestartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static string CurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        interstitialAd = GameManager.Instance.interstitialAd;
        instance = this;

        animator = GetComponent<Animator>();

        if (shouldPlayOpeningAnimation) animator.SetTrigger("sceneOpening");
    }

    private void Update()
    {
        if (loadingSceneOperation != null)
        {
            loadingPercentage.text = Mathf.RoundToInt((loadingSceneOperation.progress + .1f) * 100) + " %";
            loadingProgressBar.fillAmount = (loadingSceneOperation.progress + .1f);
            //print(loadingSceneOperation.progress);
        }
    }

    public void OnAnimationOver()
    {
        shouldPlayOpeningAnimation = true;
        loadingSceneOperation.allowSceneActivation = true;
    }
}
