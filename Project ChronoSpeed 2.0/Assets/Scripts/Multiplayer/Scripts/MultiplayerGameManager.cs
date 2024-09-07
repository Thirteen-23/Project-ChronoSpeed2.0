using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour
{
    [SerializeField] private PortalManager portalManager;
    
    [HideInInspector] public static MultiplayerGameManager Singleton { get; private set; }

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

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalStandInRPC(Vector3 position, Quaternion rotation)
    {
        portalManager.SpawnPortalStandIn(position, rotation);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SpawnPortalRPC(Vector3 firstPortPos, Quaternion firstPortRot, Vector3 secondPortPos, Quaternion secondPortRot)
    {
        portalManager.SpawnPortal(firstPortPos, secondPortPos, firstPortRot, secondPortRot);
    }
}
