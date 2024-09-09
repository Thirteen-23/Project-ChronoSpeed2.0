using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MultiplayerGameManager : NetworkBehaviour
{
    [SerializeField] private PortalManager portalManager;
    [SerializeField] private TMP_Text StartCountdownText;

    [SerializeField] private

    struct PlayerGameStats
    { 
        public bool FinishedRacing { get; private set; }
        public float RaceDuration { get; private set; }
        public ulong ClientId { get; private set; }

        public PlayerGameStats(ulong clientId = 0, bool finishedRacing = false, float raceDuraction = 0)
        {
            ClientId = clientId;
            FinishedRacing = finishedRacing;
            RaceDuration = raceDuraction;
        }
    }
    List<PlayerGameStats> gameStats = new List<PlayerGameStats>();
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

    float startTime;
    //Functions
    public IEnumerator StartGame()
    {
       startTime = Time.realtimeSinceStartup;
       foreach(var pluh in ServerManager.Singleton.ClientDic)
       {
            gameStats.Add(new PlayerGameStats(pluh.Value.ClientId));
       }
       //float endTime = Time.realtimeSinceStartup - startTime;   i think? use this to tell player how fast they did the game, maybe even lap
       for(int i = 5; i > -1; i--)
        {
            CountDownRpc(i, false);
            yield return new WaitForSeconds(1);
        }
        CountDownRpc(0, true);
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
        if (time > 0) StartCountdownText.text = time.ToString();
        else StartCountdownText.text = "GO!!!!";

        if(RaceStart)
        {
            StartCountdownText.enabled = false;
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerInput>().enabled = true;
        }
        else StartCountdownText.enabled = true;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void PlayerFinishedRpc(RpcParams srpcp = default)
    {
        for(int i = 0; i > gameStats.Count; i++) 
        {
            if (gameStats[i].ClientId == srpcp.Receive.SenderClientId)
            {
                gameStats[i] = new PlayerGameStats(gameStats[i].ClientId, true, Time.realtimeSinceStartup - startTime);
                return;
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetLeaderBoardRpc(float plplokokplplokkoplplokokplplokokplplokkolplpokpl, RpcParams rpcParams)
    {

    }
}
