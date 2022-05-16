using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TextMeshProUGUI gameYearText;

    [SerializeField]
    private Canvas worldEventCanvas;

    [SerializeField]
    private Canvas announcementCanvas;

    [SerializeField]
    private Canvas menuCanvas;

    [SerializeField]
    private Canvas headerCanvas;

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

    private void Awake()
    {
        Cursor.visible = true;      
    }

    private void Update()
    {
        playersInGameText.text = $"{PlayersManager.Instance.PlayersInGame} Player";
        gameYearText.text = $"{GameManager.Instance.gameYear}";

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