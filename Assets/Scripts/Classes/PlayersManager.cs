using System;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : Singleton<PlayersManager>

{
    [SerializeField] GameObject gameManager;
    [SerializeField] int playersCountForStart = 4;
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log($"{id} just connected...");
            playersInGame.Value++;
            if(playersInGame.Value == playersCountForStart)
            {
                gameManager.GetComponent<GameManager>().gameStatus = true;
                gameManager.GetComponent<GameManager>().StartGame();
            }
        
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            Debug.Log($"{id} just disconnected...");
            playersInGame.Value--;

        };


    }
}

