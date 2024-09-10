using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private string characterSelectionSceneName = "CharacterSelect";
    [SerializeField] private string raceSceneName = "RaceTrack";
    [SerializeField] private int MaxPlayers = 12;

    private bool gameHasStarted;
    public Dictionary<ulong, ClientData> ClientDic { get; private set; }

    public static ServerManager Singleton { get; private set; }
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
        Debug.Log(NetworkManager.Singleton.StartClient());
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
        if (ClientDic.Count > MaxPlayers || gameHasStarted)
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
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectionSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (ClientDic.ContainsKey(clientId))
        {
            if (ClientDic.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientDic.TryGetValue(clientId, out ClientData data))
        {
            data.CharacterId = characterId;
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        NetworkManager.Singleton.SceneManager.LoadScene(raceSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    CarPlayerPrefabSpawner cpps;
    int startPos = 0;
    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            if (sceneEvent.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                cpps = FindAnyObjectByType<CarPlayerPrefabSpawner>();

                //spawn ai cause you know how many players there are
                //MultiplayerGameManager.Singleton.AddPlayerToDictionary(aiObject);
            }
            if (ClientDic.TryGetValue(sceneEvent.ClientId, out ClientData data))
            {
                var car = cpps.carDatabase.GetCarById(data.CharacterId);
                if (car != null)
                {
                    var playerObject = Instantiate(car.CarPlayable, cpps.startingPositions[startPos]);
                    MultiplayerGameManager.Singleton.AddPlayerToDictionary(playerObject);
                    playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(data.ClientId);
                    startPos++;
                }
            }
        }
        else if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            //i assume this is why you can call start coroutine on something else, otherwise i dont really know why
            MultiplayerGameManager.Singleton.StartCoroutine(MultiplayerGameManager.Singleton.StartGame());
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;
        }
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
