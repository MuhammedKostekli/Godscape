using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject uiManager;

    [SerializeField] float worldEventPerSec = 10f;
    private NetworkVariable<float> timer = new NetworkVariable<float>();
    public NetworkVariable<bool> wEventTrigger = new NetworkVariable<bool>();
    private NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> currentGameYear = new NetworkVariable<int>();
    public NetworkList<int> worldEventList;
    public NetworkList<int> selectedWorldEventList;
    public NetworkVariable<int> selectedWorldEventIndex = new NetworkVariable<int>();

    private float gameStartTime;
    
    [SerializeField] int gameStartYear = 3245;
    [SerializeField] int gameYearPerSec = 6;
    [SerializeField] int worldEventCount = 20;

    public float minutes
    {       
        get
        {
            return Mathf.Floor(timer.Value / 60);
        }
    }
    public float seconds
    {
        get
        {
            return Mathf.RoundToInt(timer.Value % 60);
        }
    }
    public float gameYear
    {
        get
        {
            return currentGameYear.Value;
        }
    }
    public bool worldEvent
    {
        get
        {
            return wEventTrigger.Value;
        }
    }

    public NetworkList<int> worldEventIndexList
    {
        get
        {
            return worldEventList; 
        }
    }
    public NetworkList<int> selectedWorldEventIndexList
    {
        get
        {
            return selectedWorldEventList; 
        }
        set
        {
            selectedWorldEventList = value;
        }
    }
    public bool gameStatus
    {
        get
        {
            return isGameStarted.Value;
        }
        set {
            isGameStarted.Value = value;
        }
    }

    public float timerVar
    {
        get
        {
            return timer.Value;
        }
        set
        {
            timer.Value = value;
        }
    }
    public int selectedWorldEvent
    {
        get
        {
            return selectedWorldEventIndex.Value;
        }
        set
        {
            selectedWorldEventIndex.Value = value;
        }
    }



    private void Awake()
    {
        worldEventList = new NetworkList<int>(new List<int>());
        selectedWorldEventList = new NetworkList<int>(new List<int>());
    }
    private void Start()
    {
        currentGameYear.Value = gameStartYear;
    }
    private void Update()
    {
        if(NetworkManager.IsHost)
        {
            if (gameStatus)
            {
                timer.Value = Time.time - gameStartTime;
                //Debug.Log((int)timer.Value);
            }
        }  
    }
    public void StartGame()
    {
        gameStartTime = Time.time;
        StartCoroutine(gameYears());
        StartCoroutine(worldEvents());
    }
    public void CalculateWorldEventVoteResult()
    {
        /*if (selectedWorldEventIndexList.Count == PlayersManager.Instance.PlayersInGame)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log(selectedWorldEventIndexList);
                var templist = new List<int>();
                foreach (int element in selectedWorldEventIndexList)
                {
                    templist.Add(element);
                }
                Debug.Log(templist.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First());
                selectedWorldEventIndex.Value = templist.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                Debug.Log("World Event Occurred:" + selectedWorldEventIndex.Value);
                uiManager.GetComponent<UIManager>().showWorldEventAnnouncement();
            }

        }*/
        Debug.Log(selectedWorldEventIndexList.Count);
        if (selectedWorldEventIndexList.Count >= 1)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                selectedWorldEventIndex.Value = selectedWorldEventIndexList[0];
                Debug.Log("World Event Occurred:" + selectedWorldEventIndex.Value);
            }
            uiManager.GetComponent<UIManager>().showWorldEventAnnouncement();
        }
    }
    IEnumerator worldEvents()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("routine working");
            yield return new WaitUntil(() => (int)(timer.Value) % worldEventPerSec == 0);
            {
                if (NetworkManager.IsHost)
                {
                    worldEventList.Clear();
                    selectedWorldEventList.Clear();
                    while (worldEventList.Count <= 3)
                    {
                        System.Random rnd = new System.Random();
                        int index = rnd.Next(worldEventCount);
                        if (!worldEventList.Contains(index))
                        {
                            worldEventList.Add(index);
                        }
                    }
                    wEventTrigger.Value = true;
                }
                Debug.Log("world event");                             
            }
        }
    }


    IEnumerator gameYears()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => (int)(timer.Value) % gameYearPerSec == 0);
            {
                currentGameYear.Value++;
            }
        }
    }
}

