using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera;

    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    // Header Text
    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TextMeshProUGUI gameYearText;

    [SerializeField]
    private TextMeshProUGUI godNameText;

    [SerializeField]
    private TextMeshProUGUI militaryResText;

    [SerializeField]
    private TextMeshProUGUI cultureResText;

    [SerializeField]
    private TextMeshProUGUI tradeResText;

    [SerializeField]
    private TextMeshProUGUI techResText;

    [SerializeField]
    private TextMeshProUGUI productionResText;

    [SerializeField]
    private TextMeshProUGUI winnerText;

    // Canvas
    [SerializeField]
    private Canvas worldEventCanvas;

    [SerializeField]
    private Canvas announcementCanvas;

    [SerializeField]
    private Canvas menuCanvas;

    [SerializeField]
    private Canvas headerCanvas;

    [SerializeField]
    private Canvas playerTileActionsCanvas;

    [SerializeField]
    private Canvas opponentTileActionsCanvas;

    [SerializeField]
    private Canvas winnerCanvas;

    [SerializeField]
    private List<Button> worldEventSelectionButtons;

    [SerializeField]
    private Button organizeEventButton;

    [SerializeField]
    private Button espionageButton;

    [SerializeField]
    private Button researchButton;

    [SerializeField]
    private Button claimWarButton;

    [SerializeField]
    private Button rumorButton;
    [SerializeField] private Sprite tsunamiSprite;
    [SerializeField] private Sprite earthqSprite;
    [SerializeField] private Sprite stormSprite;
    [SerializeField] private Sprite blzSprite;
    [SerializeField] private Sprite wildfSprite;
    [SerializeField] private Sprite wwSprite;
    [SerializeField] private Sprite cwSprite;
    [SerializeField] private Sprite seedSprite;
    [SerializeField] private Sprite archeologySprite;
    [SerializeField] private Sprite camoSprite;
    [SerializeField] private Sprite computeSprite;
    [SerializeField] private Sprite pyriumSprite;
    [SerializeField] private Sprite ringSprite;
    [SerializeField] private Sprite hackSprite;
    [SerializeField] private Sprite tradexSprite;
    [SerializeField] private Sprite eonSprite;
    [SerializeField] private Sprite econSprite;
    [SerializeField] private Sprite agnoSprite;
    [SerializeField] private Sprite immigSprite;
    [SerializeField] private Sprite infectedSprite;

    [SerializeField] GameObject gameManager;

    [SerializeField] int eventCost;

    public bool isActiveAction = true;

    public bool isActiveOpponentAction = true;
    List<Sprite> worldEventSprites = new List<Sprite>()
    {
         
};
    private List<string> worldEventNameList = new List<string>()  {
                        "Tsunami",
                        "Earthquake",
                        "Storm",
                        "Blizzard",
                        "Wildfire",
                        "World War",
                        "Civil War",
                        "Seed of Gods",
                        "Archeology",
                        "Camouflage",
                        "Smart Computing",
                        "Pyrium",
                        "Ring Of Fire",
                        "Hackathlon",
                        "TradeX",
                        "Eon Crisis",
                        "Economical Crisis",
                        "Agnosticism",
                        "Immigration",
                        "Seed Of Gods Infected!"
                    };
    private bool localWorldEventTrigger = false;
    private int currentWorldEventSelection = -1;
    [SerializeField] float worldEventSelectionSec = 10f;
    private float worldEventStartTime;

    Coroutine worldEventRoutine = null;

    public TileInfo currentSelectedTileInfo; 

    private void Awake()
    {
        Cursor.visible = true;      
    }

    private void Update()
    {
        playersInGameText.text = $"{PlayersManager.Instance.PlayersInGame} Player";
        // Game is running
        if (GameManager.Instance.gameYear >= 3345)
        {
            GameManager.Instance.gameStatus = false;
            winnerCanvas.GetComponent<Canvas>().enabled = true;
            var winnerIndex = 0;
            var winnerTotalResource = 0;
            for (int i = 0; i < GameManager.Instance.playerInfoListVar.Count; i++){
                var obj = GameManager.Instance.playerInfoListVar[i];
                var curtotal = obj.militaryPoints + obj.techPoints + obj.culturePoints + obj.tradePoints + obj.productionPoints;
                if (i == 0)
                {
                    winnerTotalResource = curtotal;
                }
                else
                {
                    if(curtotal >= winnerTotalResource)
                    {
                        winnerIndex = i;
                        winnerTotalResource = curtotal;
                    }
                }
            }
            if(winnerIndex == GameManager.Instance.playerIndex)
            {
                winnerText.text = "ERA IS COMPLETED! YOU ARE WINNER " + GameManager.Instance.playerInfoListVar[winnerIndex].godName.ToString();
            }
            else
            {
                winnerText.text = "ERA IS COMPLETED! YOU ARE LOSER " + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].godName.ToString();
            }

        }
        if (GameManager.Instance.gameStatus)
        {
            gameYearText.text = $"{GameManager.Instance.gameYear}";
            godNameText.text = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].godName.ToString();
            militaryResText.text = "" + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints.ToString();
            cultureResText.text = "" + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].culturePoints.ToString();
            tradeResText.text = "" + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].tradePoints.ToString();
            techResText.text = "" + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].techPoints.ToString();
            productionResText.text = "" + GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].productionPoints.ToString();

            if (!GameManager.Instance.worldEvent)
            {
                checkTileSelection();
            }
            
        }

        if (isActiveAction)
        {
            organizeEventButton.interactable = true;
            espionageButton.interactable = true;
            researchButton.interactable = true;
        }
        else
        {
            organizeEventButton.interactable = false;
            espionageButton.interactable = false;
            researchButton.interactable = false;
        }

        if (isActiveOpponentAction)
        {
            claimWarButton.interactable = true;
            rumorButton.interactable = true;
        }
        else
        {
            claimWarButton.interactable = false;
            rumorButton.interactable = false;
        }

        // World Event Check
        if (GameManager.Instance.worldEvent && !localWorldEventTrigger)
        {
            StartWorldEvent();
        }
    }

    private void Start()
    {
        worldEventSpritesFunc();
        startServerButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
            {
      
            }
            else
            {

            }
        });

        startHostButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartHost())
            {
                menuCanvas.GetComponent<Canvas>().enabled = false;
                headerCanvas.GetComponent<Canvas>().enabled = true;
            }
            else
            {

            }
        });

        startClientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                menuCanvas.GetComponent<Canvas>().enabled = false;
                headerCanvas.GetComponent<Canvas>().enabled = true;
            }
            else
            {

            }
        });

        foreach (Button btn in worldEventSelectionButtons)
        {
            btn.onClick.AddListener(() =>
            {

                if (currentWorldEventSelection == -1)
                {
                    currentWorldEventSelection = GameManager.Instance.worldEventIndexList[int.Parse(btn.tag)];
                    btn.image.color = Color.grey;
                    //Debug.Log(currentWorldEventSelection);
                }
            });
        }

        organizeEventButton.onClick.AddListener(() =>
        {
            var changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].culturePoints - eventCost;
            if(changeValue > 0 && currentSelectedTileInfo.resourceType != "Culture")
            {
                isActiveAction = false;
                showPlayerActionAnnouncement("Organize Event", "Successful");
                StartCoroutine(organizeEvent(changeValue));
            }
            else
            {
                isActiveAction = true;
                showPlayerActionAnnouncement("Organize Event", "Failed! Not enough resource");
            }
        });

        espionageButton.onClick.AddListener(() =>
        {
            var changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].tradePoints - eventCost;
            if (changeValue > 0 && currentSelectedTileInfo.resourceType != "Trade")
            {
                isActiveAction = false;
                showPlayerActionAnnouncement("Espionage", "Successful");
                StartCoroutine(espionage(changeValue));
            }
            else
            {
                isActiveAction = true;
                showPlayerActionAnnouncement("Espionage", "Failed! Not enough resource");
            }
        });

        researchButton.onClick.AddListener(() =>
        {
            StartCoroutine(research());
        });

        claimWarButton.onClick.AddListener(() =>
        {
            StartCoroutine(claimWar());
        });

        rumorButton.onClick.AddListener(() =>
        {
            StartCoroutine(rumor());
        });

    }

    private void checkTileSelection()
    {
        // Tile Selection Check
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
            {
                currentSelectedTileInfo = hit.transform.gameObject.GetComponent<TileInfo>();
                if (currentSelectedTileInfo.godIndex == GameManager.Instance.playerIndex)
                {
                    playerTileActionsCanvas.GetComponent<Canvas>().enabled = true;
                    playerTileActionsCanvas.transform.Find("TileResourceText").GetComponent<TextMeshProUGUI>().text = currentSelectedTileInfo.resourceType;
                    opponentTileActionsCanvas.GetComponent<Canvas>().enabled = false;
                }
                else if (currentSelectedTileInfo.godIndex != -1)
                {
                    playerTileActionsCanvas.GetComponent<Canvas>().enabled = false;
                    opponentTileActionsCanvas.transform.Find("GodNameText").GetComponent<TextMeshProUGUI>().text = GameManager.Instance.playerInfoList[currentSelectedTileInfo.godIndex].godName.ToString();
                    opponentTileActionsCanvas.GetComponent<Canvas>().enabled = true;
                }
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            playerTileActionsCanvas.GetComponent<Canvas>().enabled = false;
            opponentTileActionsCanvas.GetComponent<Canvas>().enabled = false;
        }
    }


    private void StartWorldEvent()
    {
        currentWorldEventSelection = -1;
        worldEventStartTime = Time.time;
        foreach (Button btn in worldEventSelectionButtons)
        {
            btn.image.color = Color.white;
        }
        worldEventRoutine = StartCoroutine(finishWorldEvents());
        worldEventCanvas.GetComponent<Canvas>().enabled = true;
        localWorldEventTrigger = true;
        UpdateWorldEventCanvas();
    }

    private void UpdateWorldEventCanvas()
    {
        for(int i=0; i<worldEventSelectionButtons.Count; i++)
        {
            worldEventSelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = worldEventNameList[GameManager.Instance.worldEventIndexList[i]];
            worldEventSelectionButtons[i].GetComponent<Image>().sprite = worldEventSprites[GameManager.Instance.worldEventIndexList[i]];
        }
    }

    IEnumerator finishWorldEvents()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => (int)(Time.time - worldEventStartTime) % worldEventSelectionSec == 0);
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    GameManager.Instance.wEventTrigger.Value = false;
                    if (currentWorldEventSelection != -1)
                    {
                        GameManager.Instance.selectedWorldEventIndexList.Add(currentWorldEventSelection);
                    }
                    else
                    {
                        GameManager.Instance.selectedWorldEventIndexList.Add(GameManager.Instance.worldEventIndexList[1]);
                    }
                    localWorldEventTrigger = false;
                    worldEventCanvas.GetComponent<Canvas>().enabled = false;
                    Debug.Log("world event finished");
                    StopCoroutine(worldEventRoutine);
                    gameManager.GetComponent<GameManager>().CalculateWorldEventVoteResult();
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    if (currentWorldEventSelection != -1)
                    {
                        GameManager.Instance.selectedWorldEventIndexList.Add(currentWorldEventSelection);
                    }
                    else
                    {
                        GameManager.Instance.selectedWorldEventIndexList.Add(GameManager.Instance.worldEventIndexList[1]);
                    }
                    localWorldEventTrigger = false;
                    worldEventCanvas.GetComponent<Canvas>().enabled = false;
                    Debug.Log("world event finished");
                    StopCoroutine(worldEventRoutine);
                    gameManager.GetComponent<GameManager>().CalculateWorldEventVoteResult();
                }
               
            }
        }
    }

    public void showWorldEventAnnouncement()
    {
        var text = "World Event is happening: \n" + worldEventNameList[GameManager.Instance.selectedWorldEventIndex.Value].ToUpper();
        Debug.Log(text);
        StartCoroutine(showAnnouncementRoutine(text));
    }

    public void showPlayerActionAnnouncement(string actionName, string txt)
    {
        var text = "Action is happening: \n" + actionName + "\n" + txt;
        Debug.Log(text);
        StartCoroutine(showAnnouncementRoutine(text));
    }

    public void showWinWarAnnouncement(string godName, bool isWin)
    {
        if (isWin)
        {
            var text = "You WON war against " + godName + ". \n Your military resource increased.";
            Debug.Log(text);
            StartCoroutine(showAnnouncementRoutine(text));
        }
        else
        {
            var text = "You LOST war against " + godName + ". \n Your military resource decreased.";
            Debug.Log(text);
            StartCoroutine(showAnnouncementRoutine(text));
        }
    }

    public void showRumorAnnouncement(string godName, bool isWin)
    {
        if (isWin)
        {
            var text = "Rumor is succesfully COMPLETED against " + godName + ". \n Opponent trade resource decreased.";
            Debug.Log(text);
            StartCoroutine(showAnnouncementRoutine(text));
        }
        else
        {
            var text = "Rumor is FAILED against " + godName;
            Debug.Log(text);
            StartCoroutine(showAnnouncementRoutine(text));
        }
    }


    IEnumerator showAnnouncementRoutine(string text)
    {
        Debug.Log(text);
        announcementCanvas.GetComponent<Canvas>().enabled = true;
        announcementCanvas.GetComponentInChildren<TextMeshProUGUI>().text = text;
        yield return new WaitForSeconds(6);
        announcementCanvas.GetComponent<Canvas>().enabled = false;

    }

    IEnumerator organizeEvent(int changedValue)
    {
        yield return new WaitForSeconds(6);
        var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
        var tmp = new GamePlayer();
        var newValue = 0;
        switch (currentSelectedTileInfo.resourceType)
        {
            case "Army":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, newValue, changedValue, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
                break;
            case "Trade":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].tradePoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changedValue, newValue, obj.techPoints, obj.productionPoints, obj.happiness);
                break;
            case "Technology":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].techPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changedValue, obj.tradePoints, newValue, obj.productionPoints, obj.happiness);
                break;
            case "Production":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].productionPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changedValue, obj.tradePoints, obj.techPoints, newValue, obj.happiness);
                break;
            default:
                break;

        }
        GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = tmp;
        isActiveAction = true;
    }

    IEnumerator espionage(int changedValue)
    {
        yield return new WaitForSeconds(6);
        var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
        var tmp = new GamePlayer();
        var newValue = 0;
        switch (currentSelectedTileInfo.resourceType)
        {
            case "Army":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, newValue, obj.culturePoints, changedValue, obj.techPoints, obj.productionPoints, obj.happiness);
                break;
            case "Culture":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].culturePoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, newValue, changedValue, obj.techPoints, obj.productionPoints, obj.happiness);
                break;
            case "Technology":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].techPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changedValue, newValue, obj.productionPoints, obj.happiness);
                break;
            case "Production":
                newValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].productionPoints + eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changedValue, obj.techPoints, newValue, obj.happiness);
                break;
            default:
                break;

        }
        GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = tmp;
        isActiveAction = true;
    }

    IEnumerator research()
    {
        isActiveAction = false;
        yield return new WaitForSeconds(6);
        var changeValue = 0;
        var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
        var tmp = new GamePlayer();
        switch (currentSelectedTileInfo.resourceType)
        {
            case "Army":
                changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints - eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints + eventCost, obj.productionPoints, obj.happiness);
                break;
            case "Culture":
                changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].culturePoints - eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints, obj.techPoints + eventCost, obj.productionPoints, obj.happiness);
                break;
            case "Trade":
                changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].tradePoints - eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints + eventCost, obj.productionPoints, obj.happiness);
                break;
            case "Production":
                changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].productionPoints - eventCost;
                tmp = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, obj.tradePoints, obj.techPoints + eventCost, changeValue, obj.happiness);
                break;
            default:
                break;

        }
        if (changeValue > 0 && currentSelectedTileInfo.resourceType != "Technology")
        {
            showPlayerActionAnnouncement("Research", "Successful");
            GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = tmp;
            isActiveAction = true;
        }
        else
        {
            showPlayerActionAnnouncement("Research", "Failed! Not enough resource");
            isActiveAction = true;
        }
    }

    IEnumerator claimWar()
    {
        isActiveOpponentAction = false;

        var opponentIndex = currentSelectedTileInfo.godIndex;
        var opponentArmyPoint = GameManager.Instance.playerInfoListVar[opponentIndex].militaryPoints;
        var playerArmyPoint = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints;


        var userWinRate = ((float)(playerArmyPoint) / (playerArmyPoint + opponentArmyPoint))*100;
        System.Random rnd = new System.Random();
        int value = rnd.Next(100);
        yield return new WaitForSeconds(6);
        if (value <= userWinRate)
        {
            showWinWarAnnouncement(GameManager.Instance.playerInfoListVar[opponentIndex].godName.ToString(), true);

            int changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints + (int)(GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints * 0.4);
            var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
            GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

            changeValue = GameManager.Instance.playerInfoListVar[opponentIndex].militaryPoints - (int)(GameManager.Instance.playerInfoListVar[opponentIndex].militaryPoints * 0.4);
            obj = GameManager.Instance.playerInfoListVar[opponentIndex];
            GameManager.Instance.playerInfoListVar[opponentIndex] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
        }
        else
        {
            showWinWarAnnouncement(GameManager.Instance.playerInfoListVar[opponentIndex].godName.ToString(), false);

            int changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints - (int)(GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].militaryPoints * 0.4);
            var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
            GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

            changeValue = GameManager.Instance.playerInfoListVar[opponentIndex].militaryPoints + (int)(GameManager.Instance.playerInfoListVar[opponentIndex].militaryPoints * 0.4);
            obj = GameManager.Instance.playerInfoListVar[opponentIndex];
            GameManager.Instance.playerInfoListVar[opponentIndex] = new GamePlayer(obj.godIndex, obj.godName, changeValue, obj.culturePoints, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);
        }

        yield return new WaitForSeconds(30);
        isActiveOpponentAction = true;
    }

    IEnumerator rumor()
    {
        isActiveOpponentAction = false;

        int changeValue = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex].culturePoints - 500;
        var obj = GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex];
        GameManager.Instance.playerInfoListVar[GameManager.Instance.playerIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, changeValue, obj.tradePoints, obj.techPoints, obj.productionPoints, obj.happiness);

        var opponentIndex = currentSelectedTileInfo.godIndex;

        var userWinRate = 20;
        System.Random rnd = new System.Random();
        int value = rnd.Next(100);
        yield return new WaitForSeconds(6);
        if (value <= userWinRate)
        {
            showRumorAnnouncement(GameManager.Instance.playerInfoListVar[opponentIndex].godName.ToString(), true);

            changeValue = GameManager.Instance.playerInfoListVar[opponentIndex].tradePoints - (int)(GameManager.Instance.playerInfoListVar[opponentIndex].tradePoints * 0.5);
            obj = GameManager.Instance.playerInfoListVar[opponentIndex];
            GameManager.Instance.playerInfoListVar[opponentIndex] = new GamePlayer(obj.godIndex, obj.godName, obj.militaryPoints, obj.culturePoints, changeValue, obj.techPoints, obj.productionPoints, obj.happiness);
        }
        else
        {
            showRumorAnnouncement(GameManager.Instance.playerInfoListVar[opponentIndex].godName.ToString(), false);
        }

        yield return new WaitForSeconds(30);
        isActiveOpponentAction = true;
    }

    void worldEventSpritesFunc()
    {
        worldEventSprites.Add(tsunamiSprite);
        worldEventSprites.Add(earthqSprite);
        worldEventSprites.Add(stormSprite);
        worldEventSprites.Add(blzSprite);
        worldEventSprites.Add(wildfSprite);
        worldEventSprites.Add(wwSprite);
        worldEventSprites.Add(cwSprite);
        worldEventSprites.Add(seedSprite);
        worldEventSprites.Add(archeologySprite);
        worldEventSprites.Add(camoSprite);
        worldEventSprites.Add(computeSprite);
        worldEventSprites.Add(pyriumSprite);
        worldEventSprites.Add(ringSprite);
        worldEventSprites.Add(hackSprite);
        worldEventSprites.Add(tradexSprite);
        worldEventSprites.Add(eonSprite);
        worldEventSprites.Add(econSprite);
        worldEventSprites.Add(agnoSprite);
        worldEventSprites.Add(immigSprite);
        worldEventSprites.Add(infectedSprite);

    }
}