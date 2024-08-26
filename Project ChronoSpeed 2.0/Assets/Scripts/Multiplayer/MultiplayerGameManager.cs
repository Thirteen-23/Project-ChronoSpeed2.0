using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    GameObject utopianCar;
    GameObject dystopianCar;



    [ServerRpc(RequireOwnership = false)]
    void SelectedCarServerRpc(ServerRpcParams srpcp = default)
    {
        var clientID = srpcp.Receive.SenderClientId;
        utopianCar.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
    }


    
}
