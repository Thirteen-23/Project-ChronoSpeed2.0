using Unity.Netcode;
using UnityEngine;

public class VFXManager : NetworkBehaviour
{
    public GameObject FuckingBlinkPrefabCauseUnityNetcodeDoesntLikeMe;

    private static VFXManager instance;

    public override void OnNetworkSpawn()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        base.OnNetworkSpawn();
    }
    
    // fix for later maybe for use idk

    /*public static void AlterVFXState(GameObject playerRef, VFXTypes type, bool setTo)
    {
        instance.SendInfoToEveryoneRpc(playerRef.GetComponent<NetworkObject>().NetworkObjectId, type, setTo);
    }

    [Rpc(SendTo.NotMe, RequireOwnership = false)]
    public void SendInfoToEveryoneRpc(ulong objectID, VFXTypes type, bool setTo)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectID].GetComponent<VFXContainer>().SetVFX(type, setTo);
    }
    */
}
