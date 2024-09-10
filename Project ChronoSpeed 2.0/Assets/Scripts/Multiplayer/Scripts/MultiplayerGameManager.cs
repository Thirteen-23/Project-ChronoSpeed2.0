using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class MultiplayerGameManager : NetworkBehaviour
{
    [SerializeField] private PortalManager portalManager;
    [SerializeField] private Tracking_Manager_Script lapManager;
    [SerializeField] private TMP_Text startCountdownText;
    [SerializeField] private LeadboardPlayerBar[] playerBars;

    class PlayerGameStats
    {
        public int RacePosition;
        public int LapCount;
        public bool FinishedRacing;
        public float RaceDuration;
        public ulong ClientId;

        public PlayerGameStats(ulong clientId)
        {
            ClientId = clientId;
        }
    }

    
    Dictionary<GameObject, PlayerGameStats> gameStats = new Dictionary<GameObject, PlayerGameStats>();
    [HideInInspector] public static MultiplayerGameManager Singleton { get; private set; }
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
            Destroy(Singleton);
        else
        {
            Singleton = this;
        }
    }
    
    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            
        }
    }

    bool _dictionaryChanged = false;
    private void Update()
    {
        if(IsServer)
        {
            if(_dictionaryChanged)
            {
                _dictionaryChanged = false;
                SetLeaderBoardRpc();
            }    
        }
    }
    float startTime;
    //Functions
    public void AddPlayerToDictionary(GameObject playerCar)
    {
        for(int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            if(NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.gameObject == playerCar)
            {
                gameStats.Add(playerCar, new PlayerGameStats((ulong)i));
                return;
            }
        }

        //Means its an ai car, or somethings gone wrong
        //TODO: make this pick a random name later - also add names later
        gameStats.Add(playerCar, new PlayerGameStats(100));
    }
    public void AlterDictonaryValue(GameObject playerCar, int racePosition, int lap)
    {
        gameStats[playerCar].RacePosition = racePosition;
        gameStats[playerCar].LapCount = lap;
        _dictionaryChanged = true;
    }
    public IEnumerator StartGame()
    {
       
       //float endTime = Time.realtimeSinceStartup - startTime;   i think? use this to tell player how fast they did the game, maybe even lap
       for(int i = 5; i > -1; i--)
        {
            CountDownRpc(i, false);
            yield return new WaitForSeconds(1);
        }
        CountDownRpc(0, true);
        startTime = Time.timeSinceLevelLoad;
    }


    //RPCs
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalStandInRpc(Vector3 position, Quaternion rotation)
    {
        portalManager.SpawnPortalStandIn(position, rotation);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalRpc(Vector3 firstPortPos, Quaternion firstPortRot, Vector3 secondPortPos, Quaternion secondPortRot)
    {
        portalManager.SpawnPortal(firstPortPos, secondPortPos, firstPortRot, secondPortRot);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void CountDownRpc(int time, bool RaceStart)
    {
        if (time > 0) startCountdownText.text = time.ToString();
        else startCountdownText.text = "GO!!!!";

        if(RaceStart)
        {
            startCountdownText.enabled = false;
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerInput>().enabled = true;
        }
        else startCountdownText.enabled = true;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void PlayerFinishedRpc(RpcParams srpcp = default)
    {
        var player = NetworkManager.Singleton.ConnectedClients[srpcp.Receive.SenderClientId].PlayerObject.gameObject;
        gameStats[player].FinishedRacing = true;
        gameStats[player].RaceDuration = Time.timeSinceLevelLoad - startTime; 
        _dictionaryChanged = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetLeaderBoardRpc()
    {
        for(int i = 0; i < playerBars.Length; i++)
        {
            foreach(var player in gameStats)
            {
                if(player.Value.RacePosition == i + 1)
                {
                    playerBars[i].playerNameText.text = player.Value.ClientId.ToString();
                    playerBars[i].lapCountText.text = player.Value.LapCount.ToString();

                    playerBars[i].SetFinishedTime(player.Value.FinishedRacing);
                    //TODO: make this minutes and seconds not a single float
                    playerBars[i].raceCompletionText.text = player.Value.RaceDuration.ToString();
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddLeaderBoardRpc()
    {
        playerBars = new LeadboardPlayerBar[playerBars.Length];
    }
}
