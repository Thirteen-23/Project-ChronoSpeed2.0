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
    [SerializeField] private LeadboardPlayerBar[] playerBars;

    public Dictionary<ulong, GameObject> playerPrefabRef { get; private set;}
    [HideInInspector] public static MultiplayerGameManager Singleton { get; private set; }
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
            Destroy(this);
        else
        {
            Singleton = this;
            playerPrefabRef = new Dictionary<ulong, GameObject>();
        }
    }
    

    
    //Functions
    public void AddSpawnedAI(GameObject spawnedPlayer, ulong clientID)
    {
        playerPrefabRef.Add(clientID, spawnedPlayer);
        lapManager.AddTrackedCar(spawnedPlayer, false);
    }
    public void AddSpawnedPlayer(GameObject spawnedPlayer, ulong clientID)
    {
        playerPrefabRef.Add(clientID, spawnedPlayer);
        lapManager.AddTrackedCar(spawnedPlayer, true);
    }

    bool gameGoing = true;
    private IEnumerator ShareTrackedCars()
    {
        while(gameGoing)
        {
            ulong[] playerNames = new ulong[lapManager.TrackedCars.Count + lapManager.FinishedCars.Count];
            int finishedPlayers = 0;
            for(int i = 0; i < lapManager.FinishedCars.Count; i++)
            {
                foreach(var client in playerPrefabRef)
                {
                    if (client.Value.Equals(lapManager.FinishedCars[i].Car))
                    {
                        playerNames[i] = client.Key;
                        if(ServerManager.Singleton.ClientDic.ContainsKey(client.Key))
                            finishedPlayers++;
                        break;
                    }
                }
            }
            for (int i = 0; i < lapManager.TrackedCars.Count; i++)
            {
                foreach (var client in playerPrefabRef)
                {
                    if (client.Value.Equals(lapManager.TrackedCars[i].Car))
                    {
                        playerNames[i + lapManager.FinishedCars.Count] = client.Key;
                        break;
                    }
                }
            }
            SetLeaderBoardRpc(playerNames, lapManager.FinishedCars.ToArray(), lapManager.TrackedCars.ToArray());
            if (finishedPlayers == ServerManager.Singleton.ClientDic.Count)
            {
                gameGoing = false;
                GameEndedRpc();
            }
            yield return new WaitForSeconds(1f);
        }
        

    }

    public IEnumerator StartGame()
    {
        StartCoroutine(ShareTrackedCars());
        //float endTime = Time.realtimeSinceStartup - startTime;   i think? use this to tell player how fast they did the game, maybe even lap
        for (int i = 5; i > -1; i--)
        {
            CountDownRpc(i, false);
            yield return new WaitForSeconds(1);
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

    [Rpc(SendTo.ClientsAndHost)]
    private void SetLeaderBoardRpc(ulong[] playerNames, Tracking_Manager_Script.TrackedInfo[] finishedCars, Tracking_Manager_Script.TrackedInfo[] trackingCars)
    {
        for(int i = 0; i < playerBars.Length; i++)
        {
            if (i < playerNames.Length && playerNames[i] == NetworkManager.Singleton.LocalClientId)
                playerBars[i].SetYouSign(true);
            else
                playerBars[i].SetYouSign(false);
            
            int trackingCarsI = i - finishedCars.Length;
            if (i < finishedCars.Length)
            {
                playerBars[i].placementText.text = finishedCars[i].Place.ToString();
                playerBars[i].playerNameText.text = $"Player: {playerNames[i]}";
                string timeInterval = TimeSpan.FromSeconds(finishedCars[i].raceCompletedIn).ToString("mm\\:ss\\.ff");

                playerBars[i].raceCompletionText.text = $"{timeInterval}";
                playerBars[i].SetFinishedTime(true);
                continue;
            }
            else if (trackingCarsI < trackingCars.Length)
            {
                playerBars[i].placementText.text = trackingCars[trackingCarsI].Place.ToString();
                playerBars[i].lapCountText.text = $"{trackingCars[trackingCarsI].CurLap}";
                playerBars[i].playerNameText.text = $"Player: {playerNames[i]}";
                playerBars[i].raceCompletionText.text = $"{trackingCars[trackingCarsI].raceCompletedIn}";
                playerBars[i].SetFinishedTime(false);
            }
            else
                playerBars[i].gameObject.SetActive(false);

            
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void GameEndedRpc()
    {
        gameEndedText.enabled = true;
        playerBars[0].transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(-750, -250, 0);
        leaveGameBtn.SetActive(true);
        EventSystem.current.SetSelectedGameObject(leaveGameBtn);
    }
}
