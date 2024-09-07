using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private string characterSelectionSceneName = "CharacterSelect";
    [SerializeField] private string raceSceneName = "RaceTrack";
    [SerializeField] private int MaxPlayers = 12;
    public static ServerManager Singleton { get; private set; }

    private bool gameHasStarted;
    public Dictionary<ulong, ClientData> ClientDic { get; private set; }    
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
            Destroy(Singleton);
        else
        {
            Singleton = this;
            DontDestroyOnLoad(Singleton);
        }
    }

    public void StartClient(string ip)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)15000);
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientDic = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientDic = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
    }

    ///maybe
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(ClientDic.Count > MaxPlayers || gameHasStarted)
        {
            response.Approved = false; 
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientDic[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectionSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if(ClientDic.ContainsKey(clientId))
        {
            if(ClientDic.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if(ClientDic.TryGetValue(clientId, out ClientData data))
        {
            data.CharacterId = characterId;
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;
        NetworkManager.Singleton.SceneManager.LoadScene(raceSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}

[Serializable]
public class ClientData
{
    public ulong ClientId;
    public int CharacterId = -1;

    public ClientData(ulong clientId) 
    {
        ClientId = clientId;
    }
}
