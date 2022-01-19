using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StopWatch : MonoBehaviour
{
    public static StopWatch instance;
    public float timeStart;
    public TextMeshProUGUI textBox;

    private bool timerActive = false;

    void Start()
    {
        instance = this;
        textBox.text = timeStart.ToString("F2") + " s";
    }

    public static void StartTime()
    {
        instance.timerActive = true;
    }
    public static void StopTime()
    {
        instance.timerActive = false;
        if (!instance.timerActive)
        {
            GameManager.Instance.data.TotalTime += instance.timeStart;
        }
    }

    public static void DefaultTime()
    {
        instance.timeStart = 0f;
        instance.textBox.text = instance.timeStart.ToString("F2") + " s";
    }

    private void Update()
    {
        if (timerActive)
        {
            timeStart += Time.deltaTime;
            textBox.text = timeStart.ToString("F2") + " s";
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.data.TotalTime += instance.timeStart;
    }
}
