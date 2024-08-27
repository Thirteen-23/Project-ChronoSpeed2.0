using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerCarSelection : NetworkBehaviour
{
    [SerializeField] private CarCharacterStorage carDatabase;
    [SerializeField] private CarSpotlight mainPlayerSpawnPositon;
    [SerializeField] private CarSpotlight[] carSpawnPositions; //set 0 to be behind main player, then 1 to the left of 0, 2 to the right of 0, 3 to the left of 1 etc...
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Transform carSelectButtonHolder;

    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                HandleClientConnected(client.ClientId);
        }
        
        if(IsClient)
        {
            CarCharacter[] allCars = carDatabase.GetAllCars();

            foreach(var car in allCars)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, carSelectButtonHolder);
                selectButtonInstance.SetCharacter(this, car);
            }

            players.OnListChanged += HandlePlayersStateChanged;
        }
       
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
        
        if(IsClient)
        {
            players.OnListChanged -= HandlePlayersStateChanged;
        }
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

    public void Select(CarCharacter carc)
    {
        characterNameText.text = carc.CarName;
        characterInfoPanel.SetActive(true);
        CarSelectServerRpc(carc.Id);
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

    [ServerRpc(RequireOwnership = false)]
    public void LockInServerRpc(bool lockinState, ServerRpcParams srpcp = default)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == srpcp.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(players[i].ClientID, players[i].CharacterID, lockinState);
                break;
            }
        }

        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].LockedIn)
            {
                //Every player is locked in
                if(i + 1 == players.Count)
                {
                    StartGameCountdown();
                }
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerator StartGameCountdown()
    {
        //count three seconds or something
        yield return new WaitForSeconds(3.0f);
        StartGame();
    }
    private void StartGame()
    {

    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for(int i = 0; i< carSpawnPositions.Length; i++)
        {
            if(players.Count > i)
            {
                carSpawnPositions[i].UpdateDisplay(players[i]);
            }
            else
            {
                carSpawnPositions[i].DisableDisplay();
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