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
    public List<GamePlayerObj> startPlayerList = new List<GamePlayerObj>();
    public int playerIndex;
    public NetworkList<GamePlayer> playerInfoList;

    private float gameStartTime;
    [SerializeField] int gameStartYear = 3245;
    [SerializeField] int gameYearPerSec = 6;
    [SerializeField] int worldEventCount = 20;
    [SerializeField] int increaseResourcePerYear = 50;
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
    public NetworkList<GamePlayer> playerInfoListVar
    {
        get
        {
            return playerInfoList;
        }
        set
        {
            playerInfoList = value;
        }
    }

    private void Awake()
    {
        worldEventList = new NetworkList<int>(new List<int>());
        selectedWorldEventList = new NetworkList<int>(new List<int>());
        playerInfoList= new NetworkList<GamePlayer>(new List<GamePlayer>());
    }

    private void Start()
    {
        currentGameYear.Value = gameStartYear;
        startPlayerList.Add(new GamePlayerObj(0, "Mien", 0, 0, 0, 0, 0, 0));
        startPlayerList.Add(new GamePlayerObj(1, "Nedes", 0, 0, 0, 0, 0, 0));
        startPlayerList.Add(new GamePlayerObj(2, "Vealla", 0, 0, 0, 0, 0, 0));
        startPlayerList.Add(new GamePlayerObj(3, "Cistris", 0, 0, 0, 0, 0, 0));
        
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
            player = playerInfoListVar[playerIndex];
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
        if (playerInfoListVar.Count == 0)
        {
            playerInfoListVar.Add(new GamePlayer(0, "Mien", 0, 0, 0, 0, 0, 0));
            playerInfoListVar.Add(new GamePlayer(1, "Nedes", 0, 0, 0, 0, 0, 0));
            playerInfoListVar.Add(new GamePlayer(2, "Vealla", 0, 0, 0, 0, 0, 0));
            playerInfoListVar.Add(new GamePlayer(3, "Cistris", 0, 0, 0, 0, 0, 0));
        }
    }

    public void generateMapInfos()
    {
        foreach (var obj2 in gameMap.GetComponentsInChildren<MeshFilter>().Select((value, i) => new { i, value }))
        {
            var x = obj2.i % 50;
            var y = obj2.i / 50;
            var resourceType = "";
            var godIndex = -1;
            switch (obj2.value.sharedMesh.name)
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
                    Debug.Log(obj2.value.sharedMesh.name);
                    break;

            }
            if (resourceType != "Sea" && obj2.value.gameObject.tag != "Untagged")
            {
                if (int.Parse(obj2.value.gameObject.tag) == 0)
                {
                    godIndex = 0;
                }
                //obj.value.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("God" + obj.value.gameObject.tag + "Color") as Material;
                if (int.Parse(obj2.value.gameObject.tag) == 1)
                {
                    godIndex = 1;
                }
                if (int.Parse(obj2.value.gameObject.tag) == 2)
                {
                    godIndex = 2;
                }
                if (int.Parse(obj2.value.gameObject.tag) == 3)
                {
                    godIndex = 3;
                }
            }
            if (godIndex != -1)
            {
                switch (resourceType)
                {
                    case "Army":
                        startPlayerList[godIndex].militaryPoints += resourcePerTile;
                        break;
                    case "Trade":
                        startPlayerList[godIndex].tradePoints += resourcePerTile;
                        break;
                    case "Culture":
                        startPlayerList[godIndex].culturePoints += resourcePerTile;
                        break;
                    case "Technology":
                        startPlayerList[godIndex].techPoints += resourcePerTile;
                        break;
                    case "Production":
                        startPlayerList[godIndex].productionPoints += resourcePerTile;
                        break;
                    default:
                        Debug.Log(resourceType);
                        break;

                }
            }
            TileInfo tl = obj2.value.gameObject.AddComponent(typeof(TileInfo)) as TileInfo;
            tl.godIndex = godIndex;
            tl.x = x;
            tl.y = y;
            tl.resourceType = resourceType;
        }
        for(int i = 0; i < startPlayerList.Count;i++)
        {
            var obj = startPlayerList[i];
            playerInfoListVar[i] = new GamePlayer(playerInfoListVar[obj.godIndex].godIndex, playerInfoListVar[obj.godIndex].godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
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
                if(selectedWorldEventIndex.Value == 0)
                {
                    StartCoroutine(tsunami());
                }
                else if (selectedWorldEventIndex.Value == 1)
                {
                    StartCoroutine(earthquake());
                }
                else if (selectedWorldEventIndex.Value == 2)
                {
                    StartCoroutine(storm());
                }
                else if (selectedWorldEventIndex.Value == 3)
                {
                    StartCoroutine(blizzard());
                }
                else if (selectedWorldEventIndex.Value == 4)
                {
                    StartCoroutine(wildfire());
                }
                else if (selectedWorldEventIndex.Value == 5)
                {
                    StartCoroutine(worldWar());
                }
                else if (selectedWorldEventIndex.Value == 6)
                {
                    StartCoroutine(civilWar());
                }
                else if (selectedWorldEventIndex.Value == 7)
                {
                    StartCoroutine(seedOfGods());
                }
                else if (selectedWorldEventIndex.Value == 8)
                {
                    StartCoroutine(archeology());
                }
                else if (selectedWorldEventIndex.Value == 9)
                {
                    StartCoroutine(camouflage());
                }
                else if (selectedWorldEventIndex.Value == 10)
                {
                    StartCoroutine(smartComputing());
                }
                else if (selectedWorldEventIndex.Value == 11)
                {
                    StartCoroutine(pyrium());
                }
                else if (selectedWorldEventIndex.Value == 12)
                {
                    StartCoroutine(ringOfFire());
                }
                else if (selectedWorldEventIndex.Value == 13)
                {
                    StartCoroutine(hackathlon());
                }
                else if (selectedWorldEventIndex.Value == 14)
                {
                    StartCoroutine(tradeX());
                }
                else if (selectedWorldEventIndex.Value == 15)
                {
                    StartCoroutine(eonCrisis());
                }
                else if (selectedWorldEventIndex.Value == 16)
                {
                    StartCoroutine(economicalCrisis());
                }
                else if (selectedWorldEventIndex.Value == 17)
                {
                    StartCoroutine(agnosticism());
                }
                else if (selectedWorldEventIndex.Value == 18)
                {
                    StartCoroutine(immigration());
                }
                else if (selectedWorldEventIndex.Value == 19)
                {
                    StartCoroutine(seedOfGodsInfected());
                }
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
                if (NetworkManager.IsHost)
                {
                    for (var i = 0; i < playerInfoListVar.Count; i++)
                    {
                        var obj = playerInfoListVar[i];
                        playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints+increaseResourcePerYear, obj.culturePoints + increaseResourcePerYear, obj.tradePoints + increaseResourcePerYear, obj.techPoints + increaseResourcePerYear, obj.productionPoints + increaseResourcePerYear, obj.happiness + increaseResourcePerYear);
                    }
                }
            }
        }
    }

    // World Events

    // Disasters
    IEnumerator tsunami()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i=0; i<playerInfoListVar.Count;i++)
        {
            tmpVars[i] = playerInfoListVar[i].tradePoints;
            if (playerInfoListVar[i].tradePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].tradePoints - (int)(playerInfoListVar[i].tradePoints * 0.4);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec*10);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, tmpVars[i], obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator earthquake()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].productionPoints;
            Debug.Log(tmpVars[i]);
            if (playerInfoListVar[i].productionPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].productionPoints - (int)(playerInfoListVar[i].productionPoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, changeValue, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, tmpVars[i], obj.happiness);

        }
    }

    IEnumerator storm()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].techPoints;
            if (playerInfoListVar[i].techPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].techPoints - (int)(playerInfoListVar[i].techPoints * 0.2);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, changeValue, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 10);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, tmpVars[i], obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator blizzard()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].militaryPoints;
            if (playerInfoListVar[i].militaryPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].militaryPoints - (int)(playerInfoListVar[i].militaryPoints * 0.4);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 10);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, tmpVars[i], obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator wildfire()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].culturePoints;
            if (playerInfoListVar[i].culturePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].culturePoints - (int)(playerInfoListVar[i].culturePoints * 0.4);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 10);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, tmpVars[i], obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    // Wars
    IEnumerator worldWar()
    {
        var lowIndex = 0;
        var lowValue = playerInfoListVar[0].militaryPoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if(playerInfoListVar[i].militaryPoints <= lowValue)
            {
                lowIndex = i;
                lowValue = playerInfoListVar[i].militaryPoints;
            }
        }

        int changeValue = playerInfoListVar[lowIndex].tradePoints - (int)(playerInfoListVar[lowValue].tradePoints * 0.2);
        var obj = playerInfoListVar[lowIndex];
        playerInfoListVar[lowIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    IEnumerator civilWar()
    {
        var lowIndex = 0;
        var lowValue = playerInfoListVar[0].militaryPoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].militaryPoints <= lowValue)
            {
                lowIndex = i;
                lowValue = playerInfoListVar[i].militaryPoints;
            }
        }

        int changeValue = playerInfoListVar[lowIndex].productionPoints - (int)(playerInfoListVar[lowValue].productionPoints * 0.2);
        var obj = playerInfoListVar[lowIndex];
        playerInfoListVar[lowIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, changeValue, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    // Discoveries
    IEnumerator seedOfGods()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].productionPoints;
            if (playerInfoListVar[i].productionPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].productionPoints + (int)(playerInfoListVar[i].productionPoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, changeValue, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints, tmpVars[i], obj.happiness);

        }
    }

    IEnumerator archeology()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].culturePoints;
            if (playerInfoListVar[i].culturePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].culturePoints + (int)(playerInfoListVar[i].culturePoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, tmpVars[i], obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator camouflage()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].militaryPoints;
            if (playerInfoListVar[i].militaryPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].militaryPoints + (int)(playerInfoListVar[i].militaryPoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, tmpVars[i], obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator smartComputing()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].techPoints;
            if (playerInfoListVar[i].techPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].techPoints + (int)(playerInfoListVar[i].techPoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, changeValue, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, tmpVars[i], obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator pyrium()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].tradePoints;
            if (playerInfoListVar[i].tradePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].tradePoints + (int)(playerInfoListVar[i].tradePoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, tmpVars[i], obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    // Cultural Events
    IEnumerator ringOfFire()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].culturePoints;
            if (playerInfoListVar[i].culturePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].culturePoints + (int)(playerInfoListVar[i].culturePoints * 0.6);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 15);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, tmpVars[i], obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator hackathlon()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].techPoints;
            if (playerInfoListVar[i].techPoints >= 0)
            {
                int changeValue = playerInfoListVar[i].techPoints + (int)(playerInfoListVar[i].techPoints * 0.5);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, changeValue, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 30);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, tmpVars[i], obj.productionPoints, obj.happiness);

        }
    }

    IEnumerator tradeX()
    {
        int[] tmpVars = new int[playerInfoListVar.Count];
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            tmpVars[i] = playerInfoListVar[i].tradePoints;
            if (playerInfoListVar[i].tradePoints >= 0)
            {
                int changeValue = playerInfoListVar[i].tradePoints + (int)(playerInfoListVar[i].tradePoints * 0.8);
                var obj = playerInfoListVar[i];
                playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
            }
        }
        yield return new WaitForSeconds(gameYearPerSec * 20);
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            var obj = playerInfoListVar[i];
            playerInfoListVar[i] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, tmpVars[i], obj.techPoints, obj.productionPoints, obj.happiness);

        }
    }

    // Crisis
    IEnumerator eonCrisis()
    {
        var lowIndex = 0;
        var lowValue = playerInfoListVar[0].techPoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].techPoints <= lowValue)
            {
                lowIndex = i;
                lowValue = playerInfoListVar[i].techPoints;
            }
        }

        int changeValue = playerInfoListVar[lowIndex].techPoints - (int)(playerInfoListVar[lowValue].techPoints * 0.2);
        var obj = playerInfoListVar[lowIndex];
        playerInfoListVar[lowIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints,obj.tradePoints, changeValue, obj.productionPoints, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    IEnumerator economicalCrisis()
    {
        var highIndex = 0;
        var highValue = playerInfoListVar[0].tradePoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].tradePoints >= highValue)
            {
                highIndex = i;
                highValue = playerInfoListVar[i].tradePoints;
            }
        }

        int changeValue = playerInfoListVar[highIndex].tradePoints - (int)(playerInfoListVar[highIndex].tradePoints * 0.2);
        var obj = playerInfoListVar[highIndex];
        playerInfoListVar[highIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    IEnumerator agnosticism()
    {
        var highIndex = 0;
        var highValue = playerInfoListVar[0].techPoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].techPoints >= highValue)
            {
                highIndex = i;
                highValue = playerInfoListVar[i].techPoints;
            }
        }

        int changeValue = playerInfoListVar[highIndex].techPoints - (int)(playerInfoListVar[highIndex].techPoints * 0.2);
        var obj = playerInfoListVar[highIndex];
        playerInfoListVar[highIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, changeValue, obj.productionPoints, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    IEnumerator immigration()
    {
        var lowIndex = 0;
        var lowValue = playerInfoListVar[0].culturePoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].culturePoints <= lowValue)
            {
                lowIndex = i;
                lowValue = playerInfoListVar[i].culturePoints;
            }
        }

        int changeValue = playerInfoListVar[lowIndex].culturePoints - (int)(playerInfoListVar[lowValue].culturePoints * 0.2);
        var obj = playerInfoListVar[lowIndex];
        playerInfoListVar[lowIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints,obj.techPoints, obj.productionPoints, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }

    IEnumerator seedOfGodsInfected()
    {
        var highIndex = 0;
        var highValue = playerInfoListVar[0].productionPoints;
        for (var i = 0; i < playerInfoListVar.Count; i++)
        {
            if (playerInfoListVar[i].productionPoints >= highValue)
            {
                highIndex = i;
                highValue = playerInfoListVar[i].productionPoints;
            }
        }

        int changeValue = playerInfoListVar[highIndex].productionPoints - (int)(playerInfoListVar[highIndex].productionPoints * 0.2);
        var obj = playerInfoListVar[highIndex];
        playerInfoListVar[highIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints,obj.techPoints, changeValue, obj.happiness);
        yield return new WaitForSeconds(gameYearPerSec * 1);
    }
}

