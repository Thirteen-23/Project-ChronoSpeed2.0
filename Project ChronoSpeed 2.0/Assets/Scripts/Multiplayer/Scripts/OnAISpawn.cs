using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnAISpawn : NetworkBehaviour
{
    private AI aiRef;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            base.OnNetworkSpawn();
            return;
        }
        aiRef = GetComponent<AI>();
        aiRef.waypoints = FindAnyObjectByType<TrackWayPoints>();
        aiRef.nodes = aiRef.waypoints.trackNodes;
        aiRef.valueBeingRead = FindObjectOfType<Tracking_Manager_Script>();
        
    }

}
