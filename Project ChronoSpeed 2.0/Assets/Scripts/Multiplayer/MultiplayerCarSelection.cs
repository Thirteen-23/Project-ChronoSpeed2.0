using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerCarSelection : NetworkBehaviour
{
    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {

        }
        else
        {

        }
        NetworkManager.Singleton.OnClientConnectedCallback +=  HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    private void HandleClientConnected(ulong clientid)
    {
        players.Add(new CharacterSelectState(clientid));
    }

    private void HandleClientDisconnected(ulong clientid)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == clientid)
            {
                players.RemoveAt(i);
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CarSelectServerRpc(int charid, ServerRpcParams srpcp = default)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == srpcp.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(players[i].ClientID, charid);
                break;
            }
        }
    }
}


public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientID;
    public int CharacterID;
    public bool LockedIn;

    public CharacterSelectState(ulong clientid, int characterid = -1, bool lockedin = false)
    {
        ClientID = clientid;
        CharacterID = characterid;
        LockedIn = lockedin;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref CharacterID);
        serializer.SerializeValue(ref LockedIn);
    }

    public bool Equals(CharacterSelectState other)
    {
        return ClientID == other.ClientID && CharacterID == other.CharacterID && LockedIn == other.LockedIn;
    }
}
