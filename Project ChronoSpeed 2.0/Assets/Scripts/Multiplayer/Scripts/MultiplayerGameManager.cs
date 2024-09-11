using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerGameManager : NetworkBehaviour
{
    [SerializeField] private PortalManager portalManager;
    [SerializeField] private Tracking_Manager_Script lapManager;
    [SerializeField] private TMP_Text startCountdownText;
    [SerializeField] private LeadboardPlayerBar[] playerBars;

    
    Dictionary<ulong, GameObject> playerPrefabRef = new Dictionary<ulong, GameObject>();
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
    

    
    private void Update()
    {
        if(IsServer)
        {
              
        }
    }

    //Functions
    public void AddSpawnedPlayer(GameObject spawnedPlayer, ulong clientID)
    {
        playerPrefabRef.Add(clientID, spawnedPlayer);
        lapManager.TrackedCars.Add(new Tracking_Manager_Script.TrackedInfo(spawnedPlayer));
    }
    private IEnumerator ShareTrackedCars()
    {
        while(true)
        {
            bool foundOne = false;
            ulong[] playerNames = new ulong[lapManager.TrackedCars.Count + lapManager.FinishedCars.Count];
            for(int i = 0; i < lapManager.FinishedCars.Count; i++)
            {
                foreach(var client in playerPrefabRef)
                {
                    if (client.Value.Equals(lapManager.FinishedCars[i].Car))
                    {
                        playerNames[i] = client.Key;
                        foundOne = true;
                        break;
                    }
                }
                if(!foundOne)
                {
                    playerNames[i] = 1000; //All ai will be 1000 untill i make names a thing
                }
            }
            for (int i = 0; i < lapManager.TrackedCars.Count; i++)
            {
                foreach (var client in playerPrefabRef)
                {
                    if (client.Value.Equals(lapManager.TrackedCars[i].Car))
                    {
                        playerNames[i] = client.Key;
                        foundOne = true;
                        break;
                    }
                }
                if (!foundOne)
                {
                    playerNames[i] = 1000; //All ai will be 1000 untill i make names a thing
                }
            }

            SetLeaderBoardRpc(playerNames, lapManager.FinishedCars.ToArray(), lapManager.TrackedCars.ToArray());
            yield return new WaitForSeconds(1f);
        }
        

    }

    public IEnumerator StartGame()
    {

        //float endTime = Time.realtimeSinceStartup - startTime;   i think? use this to tell player how fast they did the game, maybe even lap
        for (int i = 5; i > -1; i--)
        {
            CountDownRpc(i, false);
            yield return new WaitForSeconds(1);
        }
        CountDownRpc(0, true);
        StartCoroutine(ShareTrackedCars());
        lapManager.startTime = Time.timeSinceLevelLoad;
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

        if(RaceStart)
        {
            startCountdownText.enabled = false;
            var player = GameObject.FindGameObjectWithTag("Player");
            var input = player.GetComponent<PlayerInput>();
            input.enabled = true;
        }
        else startCountdownText.enabled = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetLeaderBoardRpc(ulong[] playerNames, Tracking_Manager_Script.TrackedInfo[] finishedCars, Tracking_Manager_Script.TrackedInfo[] trackingCars)
    {
        for(int i = 0; i < playerBars.Length; i++)
        {
            if (i < finishedCars.Length)
            {
                playerBars[i].playerNameText.text = $"Player: {playerNames[i]}";
                string timeInterval = TimeSpan.FromSeconds(finishedCars[i].raceCompletedIn).ToString("mm\\:ss\\.ff");

                playerBars[i].raceCompletionText.text = $"{timeInterval}";
                playerBars[i].SetFinishedTime(true);
            }
            else if (i < trackingCars.Length)
            {
                playerBars[i].lapCountText.text = $"{trackingCars[i].CurLap}";
                playerBars[i].playerNameText.text = $"Player: {playerNames[i]}";
                playerBars[i].raceCompletionText.text = $"{trackingCars[i].raceCompletedIn}";
                playerBars[i].SetFinishedTime(false);
            }
            else
                playerBars[i].gameObject.SetActive(false);

            if (i < playerNames.Length && playerNames[i] == NetworkManager.Singleton.LocalClientId)
                playerBars[i].SetYouSign(true);
            else
                playerBars[i].SetYouSign(false);
        }
    }

    //[Rpc(SendTo.ClientsAndHost)]
    //private void AddLeaderBoardRpc()
    //{
    //    playerBars = new LeadboardPlayerBar[playerBars.Length];
    //}
}
