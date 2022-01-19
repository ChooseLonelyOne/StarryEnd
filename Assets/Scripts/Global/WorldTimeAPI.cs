using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.UI;
public class WorldTimeAPI : MonoBehaviour
{
    #region Singleton class: WorldTimeAPI
    public static WorldTimeAPI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //json container
    struct TimeData 
    {
        //public string client ip:
        //...
        public string datetime;
        //...
    }

    const string API_URL = "http://worldtimeapi.org/api/ip";

    [HideInInspector] public bool isTimeLoaded = false;

    private DateTime _currenDateTime = DateTime.Now;

    private void Start()
    {
        StartCoroutine(GetRealDateTimeFromAPI());
    }

    public DateTime GetCurrentDateTime()
    {
        //here we dont need to get the datetime from the server again
        //just add elapsed time since the game start to _currendDateTime

        return _currenDateTime.AddSeconds(Time.realtimeSinceStartup);
    }

    private IEnumerator GetRealDateTimeFromAPI()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);
        //print("getting real datetime...");

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            //error
            print("Error: " + webRequest.error);
        }
        else
        {
            //success
            TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);
            //timeData.datetime value is : 2021-05-23T14:56:02.907463+05:00

            _currenDateTime = ParseDateTime(timeData.datetime);
            isTimeLoaded = true;

            //print("Done");
        }
    }
    //2021-05-23T14:56:02.907463+05:00
    private DateTime ParseDateTime(string datetime)
    {
        string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;
        string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;

        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }

    /* API(json)
     * {"abbreviation":"+05",
     * "client_ip":"136.169.224.8",
     * "datetime":"2021-05-23T14:56:02.907463+05:00",
     * "day_of_week":0,
     * "day_of_year":143,
     * "dst":false,
     * "dst_from":null,
     * "dst_offset":0,
     * "dst_until":null,
     * "raw_offset":18000,
     * "timezone":"Asia/Yekaterinburg",
     * "unixtime":1621763762,
     * "utc_datetime":"2021-05-23T09:56:02.907463+00:00",
     * "utc_offset":"+05:00",
     * "week_number":20}
     * 
     * We only need "datetime" property
     */
}
