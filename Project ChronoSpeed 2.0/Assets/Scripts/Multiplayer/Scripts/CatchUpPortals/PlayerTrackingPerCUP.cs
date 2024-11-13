using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTrackingPerCUP : MonoBehaviour
{
    private List<StoreCatchUpPortals> turns = new List<StoreCatchUpPortals>();
    private Tracking_Manager_Script TMS;
    
    int lapTillReset = 0;

    class TrackedInfo
    {
        public GameObject Car;
        public int Lap;

        public TrackedInfo(GameObject car, int lap)
        {
            Car = car;
            Lap = lap;
        }
    }
    private List<TrackedInfo> playerPoses = new List<TrackedInfo>();
    List<GameObject> PassedThisLap = new List<GameObject>();

    private void Awake()
    {
        TMS = FindAnyObjectByType<Tracking_Manager_Script>();
        foreach(StoreCatchUpPortals s in GetComponentsInChildren<StoreCatchUpPortals>())
        {
            turns.Add(s);
        }
    }
    private void Start()
    {
        //Now that close all portals is an rpc can't use it on start cause itll crash when called on a client who hasent loaded this scene yet
        //could do a on networkstart kinda think but dont wanna
        for (int i = 0; i < turns.Count; i++)
        {
            //turns[i].ClosePortalPair();
        }
        if (!NetworkManager.Singleton.IsServer)
            Destroy(GetComponent<BoxCollider>());
    }
    private void Update()
    {
        if(NetworkManager.Singleton.IsServer)
            UpdatePositions();
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject player = ConfirmPlayer(other);
        if (player == null)
            return;


        if (IsPlayerFirstAndIsNewLap(player))
        {
            lapTillReset++;
            CloseAllPortalsRpc();
            PassedThisLap.Clear();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject player = ConfirmPlayer(other);
        if (player == null)
            return;

        if(PassedThisLap.Contains(player) && !IsPlayerLast(player))
        {
            if (PassedThisLap.Count == playerPoses.Count - 1)
                OpenAllPortalsRpc();
            else
                OpenPortalRpc(PassedThisLap.Count);
            PassedThisLap.Add(player);
        }
    }

    GameObject ConfirmPlayer(Collider other)
    {
        GameObject player;
        var FK = GetComponent<FakeCollision>();
        if (FK != null)
        {
            player = FK.myTransform.gameObject;
        }
        else
        {
            player = GetComponentInParent<Rigidbody>().gameObject;
        }

        if (IsCarAPlayer(player))
            return player;
        else
            return null;
    }
    bool IsPlayerFirstAndIsNewLap(GameObject playerCar)
    {
        if(playerCar == playerPoses[0].Car && playerPoses[0].Lap == lapTillReset)
            return true;
   
        return false;
    }

    bool IsPlayerLast(GameObject playerCar)
    {
        if(playerCar == playerPoses[PassedThisLap.Count - 1].Car)
            return true;
        return false;
    }

    bool IsCarAPlayer(GameObject car)
    {
        for(int i = 0; i < playerPoses.Count; i++)
        {
            if(car == playerPoses[i].Car)
                return true;
        }
        return false;
    }
    [Rpc(SendTo.ClientsAndHost)]
    void CloseAllPortalsRpc()
    {
        for(int  i = 0; i < turns.Count; i++)
        {
            turns[i].ClosePortalPair();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    void OpenPortalRpc(int i)
    {
        if(i < turns.Count)
            turns[i].OpenPortalPair();
    }
    [Rpc(SendTo.ClientsAndHost)]
    void OpenAllPortalsRpc()
    {
        for (int i = 0; i < turns.Count; i++)
        {
            turns[i].OpenPortalPair();
        }
    }

    void UpdatePositions()
    {
        playerPoses.Clear();
        for(int i = 0; i < TMS.FinishedCars.Count;i++)
        {
            if (TMS.FinishedCars[i].IsPlayer)
                playerPoses.Add(new TrackedInfo(TMS.FinishedCars[i].Car, TMS.FinishedCars[i].netInfo.CurLap));
        }
        for(int i = 0; i < TMS.TrackedCars.Count;i++)
        {
            if (TMS.TrackedCars[i].IsPlayer)
                playerPoses.Add(new TrackedInfo(TMS.TrackedCars[i].Car, TMS.TrackedCars[i].netInfo.CurLap));
        }
    }
}
