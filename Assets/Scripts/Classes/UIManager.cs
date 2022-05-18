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
    private List<Button> worldEventSelectionButtons;

    [SerializeField] GameObject gameManager;

    private List<string> worldEventNameList = new List<string>()  {
                        "Tsunami",
                        "Earthquake",
                        "Storm",
                        "Invasion",
                        "Invasion 2",
                        "Civil War",
                        "World War",
                        "Seed of Gods",
                        "Eon Fertilizer",
                        "Eon Batteries",
                        "Ring of Fire",
                        "Hackathlon",
                        "TradeX",
                        "Eon Crisis",
                        "Economical Crisis",
                        "False Gods",
                        "Agnosticism",
                        "Immigration",
                        "Cable Network",
                        "Seed of Gods infected",
                        "Necrosis"
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
        if (GameManager.Instance.gameStatus)
        {
            gameYearText.text = $"{GameManager.Instance.gameYear}";
            godNameText.text = GameManager.Instance.player.godName.ToString();
            militaryResText.text = "Army\n" + GameManager.Instance.player.militaryPoints.ToString();
            cultureResText.text = "Culture\n" + GameManager.Instance.player.culturePoints.ToString();
            tradeResText.text = "Trade\n" + GameManager.Instance.player.tradePoints.ToString();
            techResText.text = "Technology\n" + GameManager.Instance.player.techPoints.ToString();
            productionResText.text = "Production\n" + GameManager.Instance.player.productionPoints.ToString();

            if (!GameManager.Instance.worldEvent)
            {
                checkTileSelection();
            }
            
        }

        // World Event Check
        if (GameManager.Instance.worldEvent && !localWorldEventTrigger)
        {
            StartWorldEvent();
        }
    }

    private void Start()
    {
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


    IEnumerator showAnnouncementRoutine(string text)
    {
        Debug.Log(text);
        announcementCanvas.GetComponent<Canvas>().enabled = true;
        announcementCanvas.GetComponentInChildren<TextMeshProUGUI>().text = text;
        yield return new WaitForSeconds(3);
        announcementCanvas.GetComponent<Canvas>().enabled = false;

    }
}