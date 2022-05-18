using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    #pragma warning disable 0436
    [SerializeField] GameObject uiManager;
    [SerializeField] GameObject gameMap;

    [SerializeField] float worldEventPerSec = 10f;
    [SerializeField] int resourcePerTile = 100;
    private NetworkVariable<float> timer = new NetworkVariable<float>();
    public NetworkVariable<bool> wEventTrigger = new NetworkVariable<bool>();
    private NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> currentGameYear = new NetworkVariable<int>();
    public NetworkList<int> worldEventList;
    public NetworkList<int> selectedWorldEventList;
    public NetworkVariable<int> selectedWorldEventIndex = new NetworkVariable<int>();
    public GamePlayer player;
    public int playerIndex;
    public List<GamePlayer> playerInfoList = new List<GamePlayer>();


    private float gameStartTime;
    [SerializeField] int gameStartYear = 3245;
    [SerializeField] int gameYearPerSec = 6;
    [SerializeField] int worldEventCount = 20;
    private bool isUserSpesificMapCreated = false;

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
        generatePlayerInfos();
        generateMapInfos();
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

        if(!isUserSpesificMapCreated && gameStatus)
        {
            isUserSpesificMapCreated = true;
            playerIndex = (int)NetworkManager.LocalClientId;
            generateUserSpesificMap();
            player = playerInfoList[playerIndex];
        }
    }

    public void StartGame()
    {
        gameStartTime = Time.time;
        StartCoroutine(gameYears());
        StartCoroutine(worldEvents());
    }

    public void generatePlayerInfos()
    {
        playerInfoList.Add(new GamePlayer(0, "Mien", 0, 0, 0, 0, 0, 0));
        playerInfoList.Add(new GamePlayer(0, "Nedes", 0, 0, 0, 0, 0, 0));
        playerInfoList.Add(new GamePlayer(0, "Vealla", 0, 0, 0, 0, 0, 0));
        playerInfoList.Add(new GamePlayer(0, "Cistris", 0, 0, 0, 0, 0, 0));
    }

    public void generateMapInfos()
    {
        foreach (var obj in gameMap.GetComponentsInChildren<MeshFilter>().Select((value, i) => new { i, value }))
        {
            var x = obj.i % 50;
            var y = obj.i / 50;
            var resourceType = "";
            var godIndex = -1;
            switch (obj.value.sharedMesh.name)
            {
                case string a when a.Contains("Sea"):
                    resourceType = "Sea";
                    break;
                case string a when a.Contains("Army"):
                    resourceType = "Army";
                    break;
                case string a when a.Contains("Trade"):
                    resourceType = "Trade";
                    break;
                case string a when a.Contains("Culture"):
                    resourceType = "Culture";
                    break;
                case string a when a.Contains("Tech"):
                    resourceType = "Technology";
                    break;
                case string a when a.Contains("Heretic"):
                    resourceType = "Heretic";
                    break;
                case string a when a.Contains("Production"):
                    resourceType = "Production";
                    break;
                case string a when a.Contains("Volcano"):
                    resourceType = "Volcano";
                    break;
                default:
                    Debug.Log(obj.value.sharedMesh.name);
                    break;

            }
            if (resourceType != "Sea" && obj.value.gameObject.tag != "Untagged")
            {
                if (int.Parse(obj.value.gameObject.tag) == 0)
                {
                    godIndex = 0;
                }
                //obj.value.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("God" + obj.value.gameObject.tag + "Color") as Material;
                if (int.Parse(obj.value.gameObject.tag) == 1)
                {
                    godIndex = 1;
                }
                if (int.Parse(obj.value.gameObject.tag) == 2)
                {
                    godIndex = 2;
                }
                if (int.Parse(obj.value.gameObject.tag) == 3)
                {
                    godIndex = 3;
                }
            }
            if (godIndex != -1)
            {
                switch (resourceType)
                {
                    case "Army":
                        playerInfoList[godIndex].militaryPoints += resourcePerTile;
                        break;
                    case "Trade":
                        playerInfoList[godIndex].tradePoints += resourcePerTile;
                        break;
                    case "Culture":
                        playerInfoList[godIndex].culturePoints += resourcePerTile;
                        break;
                    case "Technology":
                        playerInfoList[godIndex].techPoints += resourcePerTile;
                        break;
                    case "Production":
                        playerInfoList[godIndex].productionPoints += resourcePerTile;
                        break;
                    default:
                        Debug.Log(resourceType);
                        break;

                }
            }
            TileInfo tl = obj.value.gameObject.AddComponent(typeof(TileInfo)) as TileInfo;
            tl.godIndex = godIndex;
            tl.x = x;
            tl.y = y;
            tl.resourceType = resourceType;
        }
    }

    public void generateUserSpesificMap()
    {
        var id = playerIndex;
        foreach (var obj in gameMap.GetComponentsInChildren<TileInfo>().Select((value, i) => new { i, value }))
        {
            if(obj.value.godIndex != id && obj.value.godIndex != -1)
            {
                obj.value.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("God" + (obj.value.godIndex+1) + "Color") as Material;
            }
        }
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

