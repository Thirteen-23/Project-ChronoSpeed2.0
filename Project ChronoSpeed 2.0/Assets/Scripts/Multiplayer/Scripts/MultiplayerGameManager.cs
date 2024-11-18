using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using static AI;

public class MultiplayerGameManager : NetworkBehaviour
{
    [SerializeField] private PortalManager portalManager;
    [SerializeField] private Tracking_Manager_Script lapManager;
    [SerializeField] private TMP_Text startCountdownText;
    [SerializeField] private TMP_Text gameEndedText;
    [SerializeField] private GameObject leaveGameBtn;
    [SerializeField] private LeadboardPlayerBar mainPlayerLB;
    [SerializeField] private LeadboardPlayerBar[] othersLB = new LeadboardPlayerBar[3];

    public Dictionary<ulong, GameObject> playerPrefabRef { get; private set;}
    [HideInInspector] public static MultiplayerGameManager Singleton { get; private set; }

    NetworkList<Tracking_Manager_Script.TrackedInfo.NetworkInfo> leaderboardInfoToShare;
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
            Destroy(this);
        else
        {
            Singleton = this;
            playerPrefabRef = new Dictionary<ulong, GameObject>();
        }

        leaderboardInfoToShare = new NetworkList<Tracking_Manager_Script.TrackedInfo.NetworkInfo>();
    }

    private void FixedUpdate()
    {
        //FixedUpdateSoNetworkGetsHassledLessIdk, doing network rpc and stuff on update feels scary since connecting two fucking objects together adds like 13ms of lag or whatnot
        if(IsServer)
            ShareTrackedCars();
        SetLeaderBoard();
    }

    //Functions
    public void AddSpawnedAI(GameObject spawnedPlayer, ulong clientID)
    {
        playerPrefabRef.Add(clientID, spawnedPlayer);
        lapManager.AddTrackedCar(spawnedPlayer, false, clientID);
    }

    int MaxPlayers = 0;
    public void AddSpawnedPlayer(GameObject spawnedPlayer, ulong clientID)
    {
        playerPrefabRef.Add(clientID, spawnedPlayer);
        lapManager.AddTrackedCar(spawnedPlayer, true, clientID);
        MaxPlayers++;
    }

    bool gameGoing = true;
    private void ShareTrackedCars()
    {  
        //Call this in update
        leaderboardInfoToShare.Clear();

        int realPlayersFinishedCount = 0;
        bool gameShouldFinish = false;

        for (int i = 0; i < lapManager.FinishedCars.Count; i++)
        {
            leaderboardInfoToShare.Add(lapManager.FinishedCars[i].netInfo);

            if (lapManager.FinishedCars[i].IsPlayer)
                realPlayersFinishedCount++;
            if (realPlayersFinishedCount == MaxPlayers)
                gameShouldFinish = true;
            realPlayersFinishedCount = 0;
        }

        for (int i = 0; i < lapManager.TrackedCars.Count; i++) 
        {
            leaderboardInfoToShare.Add(lapManager.TrackedCars[i].netInfo);
        }
        

        if (gameShouldFinish)
        {
            gameGoing = false;
            GameEndedRpc();
        }
        

    }
    public AudioSource countdown; 
    public IEnumerator StartGame()
    {
        
        //float endTime = Time.realtimeSinceStartup - startTime;   i think? use this to tell player how fast they did the game, maybe even lap
        for (int i = 5; i > -1; i--)
        {
            CountDownRpc(i, false);
            yield return new WaitForSeconds(1);
            if( i == 4)
            {
                countdown.Play();
            }
        }
        CountDownRpc(0, true);
        
        lapManager.startTime = Time.timeSinceLevelLoad;
    }

    public void LeaveGame()
    {
        ServerManager.Singleton.EndSessionRpc();
    }
    //RPCs
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalStandInRpc(Vector3 position, Quaternion rotation)
    {
        portalManager.SpawnPortalStandIn(position, rotation);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalRpc(Vector3 firstPortPos, Quaternion firstPortRot, Vector3 secondPortPos, Quaternion secondPortRot, float portalLast)
    {
        portalManager.SpawnPortal(firstPortPos, secondPortPos, firstPortRot, secondPortRot, portalLast);
    }

    
    [Rpc(SendTo.ClientsAndHost)]
    public void CountDownRpc(int time, bool RaceStart)
    {
        if (time > 0) startCountdownText.text = time.ToString();
        else startCountdownText.text = "GO!!!!";

        if (RaceStart)
        {
            startCountdownText.enabled = false;
            var player = GameObject.FindGameObjectWithTag("Player");
            var input = player.GetComponent<PlayerInput>();
          
            input.enabled = true;
            input.SwitchCurrentActionMap("Movement");
            

            if (IsServer)
            {
                GameObject[] AIs = GameObject.FindGameObjectsWithTag("AI");

                foreach (var curAI in AIs)
                {

                    curAI.GetComponent<AI>().difficultness = AI.aI_Difficulty.normal;
                   
                }
            }
        }
        else
        {
            
            startCountdownText.enabled = true;
            
        }
    }

    bool reachedLastLap = false;
    [SerializeField] AudioSource m_BattleMusic;
    [SerializeField] AudioSource m_ItsTheFinalCountdown;
    [SerializeField] float m_AudioSpeed = 1;
    private void SetLeaderBoard()
    {
        int mainPlayerPos = 0;
        for (int i = 0; i < leaderboardInfoToShare.Count; i++)
        {
            if (leaderboardInfoToShare[i].ClientID == NetworkManager.Singleton.LocalClientId)
            {
                mainPlayerLB.placementText.text = leaderboardInfoToShare[i].Place.ToString();
                mainPlayerLB.lapCountText.text = $"{leaderboardInfoToShare[i].CurLap}";
                mainPlayerLB.playerNameText.text = $"Player: {leaderboardInfoToShare[i].ClientID}";

                string timeInterval = TimeSpan.FromSeconds(leaderboardInfoToShare[i].raceCompletedIn).ToString("mm\\:ss\\.ff");
                mainPlayerLB.raceCompletionText.text = $"{timeInterval}";

                mainPlayerPos = leaderboardInfoToShare[i].Place;

                if (leaderboardInfoToShare[i].raceCompletedIn > 0)
                {
                    mainPlayerLB.SetFinishedTime(true);

                   
                }

                if (leaderboardInfoToShare[i].CurLap == 2 && reachedLastLap == false)
                {
                    reachedLastLap = true;
                    m_BattleMusic.volume -= m_AudioSpeed * 1;
                    m_ItsTheFinalCountdown.volume = 0;
                    m_ItsTheFinalCountdown.Play();
                    while(m_ItsTheFinalCountdown.volume < 0.5f)
                    {
                        m_ItsTheFinalCountdown.volume += m_AudioSpeed * 1;
                        
                    }

                }
            }
        }

        if (mainPlayerPos > 3 && mainPlayerPos != 0)
        {
            for (int i = 0; i < 3; i++)
            {
                othersLB[i].placementText.text = leaderboardInfoToShare[i].Place.ToString();
                othersLB[i].lapCountText.text = $"{leaderboardInfoToShare[i].CurLap}";
                othersLB[i].playerNameText.text = $"Player: {leaderboardInfoToShare[i].ClientID}";

                string timeInterval = TimeSpan.FromSeconds(leaderboardInfoToShare[i].raceCompletedIn).ToString("mm\\:ss\\.ff");
                othersLB[i].raceCompletionText.text = $"{timeInterval}";

                if (leaderboardInfoToShare[i].raceCompletedIn > 0)
                    othersLB[i].SetFinishedTime(true);


            }
        }
        else
        {
            int spot = 0;
            for(int i = 0; i < 4; i++)
            {
                if (i == mainPlayerPos - 1)
                    continue;
                else
                {
                    if(spot > 2) { continue; }
                    othersLB[spot].placementText.text = leaderboardInfoToShare[i].Place.ToString();
                    othersLB[spot].lapCountText.text = $"{leaderboardInfoToShare[i].CurLap}";
                    othersLB[spot].playerNameText.text = $"Player: {leaderboardInfoToShare[i].ClientID}";

                    string timeInterval = TimeSpan.FromSeconds(leaderboardInfoToShare[i].raceCompletedIn).ToString("mm\\:ss\\.ff");
                    othersLB[spot].raceCompletionText.text = $"{timeInterval}";

                    if (leaderboardInfoToShare[i].raceCompletedIn > 0)
                        othersLB[spot].SetFinishedTime(true);

                    spot++;
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void GameEndedRpc()
    {
        gameEndedText.enabled = true;
        mainPlayerLB.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(-750, -250, 0);
        leaveGameBtn.SetActive(true);
        EventSystem.current.SetSelectedGameObject(leaveGameBtn);
    }
}
