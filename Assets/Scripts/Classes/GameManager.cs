using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] float worldEventPerSec = 120f;
    private NetworkVariable<float> timer = new NetworkVariable<float>();
    private NetworkVariable<bool> wEventTrigger = new NetworkVariable<bool>();
    private NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> currentGameYear = new NetworkVariable<int>();

    private float gameStartTime;
    [SerializeField] int gameStartYear = 3245;
    [SerializeField] int gameYearPerSec = 6;

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

    private void Start()
    {
        currentGameYear.Value = gameStartYear;
    }
    private void Update()
    {
        if (gameStatus)
        {
            timer.Value = Time.time - gameStartTime;
            //Debug.Log((int)timer.Value);
        }
        
    }
    public void StartGame()
    {
        gameStartTime = Time.time;
        StartCoroutine(gameYears());
        StartCoroutine(worldEvents());
    }
    IEnumerator worldEvents()
    {
        while (true)
        {
            Debug.Log("routine working");
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => (int)(timer.Value) % worldEventPerSec == 0);
            {
                wEventTrigger.Value = true;
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

