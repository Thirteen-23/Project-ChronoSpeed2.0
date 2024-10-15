using Unity.Netcode;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public GameObject FuckingBlinkPrefabCauseUnityNetcodeDoesntLikeMe;

    public enum VFXTypes
    {
        electricBall,
    }

    public static void AlterVFXState(GameObject playerRef, VFXTypes type, bool setTo)
    {
        SendInfoToEveryone(playerRef.GetComponent<NetworkObject>().NetworkObjectId, type, setTo);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public static void SendInfoToEveryone(ulong objectID, VFXTypes type, bool setTo)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectID].GetComponent<VFXContainer>().SetVFX(type, setTo);

    }
}
