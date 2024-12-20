using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

//not fake one

public class ServerManager : NetworkBehaviour
{
    [SerializeField] private string raceSceneName;
    [SerializeField] private int MaxPlayers = 4;
    MainMenuManager menuManager;
    
    public bool gameHasStarted;
    public Dictionary<ulong, ClientData> ClientDic { get; private set; }

    public static ServerManager Singleton { get; private set; }
    private void Awake()
    {        
        Singleton = this;
        DontDestroyOnLoad(Singleton);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    int TestIfLived = 0;
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == "MainMenu")
        {
            menuManager = FindAnyObjectByType<MainMenuManager>();
            
            if (TestIfLived > 0)
            {
                SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
                Destroy(gameObject);
            }
            TestIfLived++;
        }
    }

    public void StartClient(string ip)
    {
        ClientDic = null;

        NetworkManager.Singleton.OnClientDisconnectCallback += HandelDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback += HandelConnect;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)15000);
        Debug.Log("Client started: " + NetworkManager.Singleton.StartClient());
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientDic = new Dictionary<ulong, ClientData>();

        Debug.Log("Host started: " + NetworkManager.Singleton.StartHost());
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
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (ClientDic.ContainsKey(clientId))
        {
            if (ClientDic.Remove(clientId))
            {
                if(SceneManager.GetActiveScene().name == raceSceneName)
                {
                    FindAnyObjectByType<MultiplayerGameManager>().MaxPlayers--;
                }
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    private void HandelDisconnect(ulong clientId)
    {
        Debug.Log("FailedToConnectLol");
    }

    private void HandelConnect(ulong clientId)
    {
        Debug.Log("SuccesfullyConnected");
        menuManager.SwitchCameraArea(2);
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
        NetworkManager.Singleton.SceneManager.LoadScene(raceSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    CarPlayerPrefabSpawner cpps;
    GameObject miragePrefab;
    int startPos = 0;
    public void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            if (sceneEvent.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                cpps = FindAnyObjectByType<CarPlayerPrefabSpawner>();
                miragePrefab = FindAnyObjectByType<VFXManager>().FuckingBlinkPrefabCauseUnityNetcodeDoesntLikeMe;

                startPos = 0;

                for (; startPos < 12 - ClientDic.Count; startPos++)
                {
                    GameObject aiObject;
                    if (startPos % 3 == 1)
                    aiObject = Instantiate(cpps.DystopiaAiCar, cpps.startingPositions[startPos]);
                    else if (startPos % 2 == 1)
                    aiObject = Instantiate(cpps.PresentAiCar, cpps.startingPositions[startPos]);
                    else
                    aiObject = Instantiate(cpps.UtopiaAiCar, cpps.startingPositions[startPos]);

                   
                    aiObject.GetComponent<NetworkObject>().Spawn();
                    MultiplayerGameManager.Singleton.AddSpawnedAI(aiObject.GetComponentInChildren<Rigidbody>().gameObject, (ulong)(1000 + startPos));
                    
                }
            }
            if (ClientDic.TryGetValue(sceneEvent.ClientId, out ClientData data))
            {
                var car = cpps.carDatabase.GetCarById(data.CharacterId);
                if (car != null)
                {
                    var playerObject = Instantiate(car.CarPlayable, cpps.startingPositions[startPos]);
                    MultiplayerGameManager.Singleton.AddSpawnedPlayer(playerObject, data.ClientId);
                    playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(data.ClientId);
                    startPos++;

                    var mirage = Instantiate(miragePrefab);
                    mirage.GetComponent<NetworkObject>().SpawnWithOwnership(data.ClientId);

                    mirage.transform.parent = playerObject.transform;
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


    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void EndSessionRpc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted -= OnNetworkReady;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            gameHasStarted = false;

            GetComponent<NetworkObject>().Despawn(false);
        }
        StartCoroutine(LeaveCoroutine());
    }

    public IEnumerator LeaveCoroutine()
    {
        
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();

            yield return new WaitForSeconds(0.2f);

            Destroy(NetworkManager.Singleton.gameObject);
            
            yield return new WaitForSeconds(0.5f);
        }

        //SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        //Singleton = null;

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        //Destroy(gameObject);
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
