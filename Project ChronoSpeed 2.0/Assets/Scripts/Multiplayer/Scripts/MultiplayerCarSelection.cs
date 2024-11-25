using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiplayerCarSelection : NetworkBehaviour
{
    [SerializeField] private CarCharacterStorage carDatabase;
    [SerializeField] private Image[] playerImages;

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite selectingSprite;
    [SerializeField] private Sprite noPlayerSprite;
    [SerializeField] private Sprite readyButtonPressed;

    [SerializeField] private Button ReadyButton;
    [SerializeField] private Button BackButton;

    private NetworkList<CharacterSelectState> players;
    /// <summary>
    /// 0 = modern button, 1 = dystopian, 2 = utopian
    /// </summary>
    public Button[] carSelectButtons;

    public bool carSelected = false;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                HandleClientConnected(client.ClientId);
        }

        if (IsClient)
        {
            CarCharacter[] allCars = carDatabase.GetAllCars();

            players.OnListChanged += HandlePlayersStateChanged;
        }

    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        if (IsClient)
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
        for (int i = 0; i < players.Count; i++)
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
        if (carSelected)
            return;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == NetworkManager.Singleton.LocalClientId)
            {
                if (players[i].LockedIn) { return; }

                if (players[i].CharacterID == carc.Id) { return; }
            }
        }

        CarSelectServerRpc(carc.Id);
        carSelected = true;
    }

    public void UnSelect()
    {
        carSelected = false;

        ExecuteEvents.Execute(BackButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == NetworkManager.Singleton.LocalClientId)
            {
                EventSystem.current.SetSelectedGameObject(carSelectButtons[i - 1].gameObject);
            }
        }
    }

    public void ReadyUp()
    {
        ExecuteEvents.Execute(ReadyButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CarSelectServerRpc(int charid, ServerRpcParams srpcp = default)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == srpcp.Receive.SenderClientId)
            {
                if (!carDatabase.IsValidCharacterId(charid)) { return; }

                players[i] = new CharacterSelectState(players[i].ClientID, charid);
                break;
            }
        }
    }

    public void ReadyUP(Image readyUpBtn)
    {
        for(int i =0; i<players.Count; i++)
        {
            if (NetworkManager.Singleton.LocalClientId == players[i].ClientID)
            {
                if (!carDatabase.IsValidCharacterId(players[i].CharacterID)) return;
                readyUpBtn.sprite = readyButtonPressed;

                //Very temp solution
                Destroy(ReadyButton);
                //readyUpBtn.color = Color.green;
                LockInServerRpc();
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void LockInServerRpc(ServerRpcParams srpcp = default)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == srpcp.Receive.SenderClientId)
            {
                if (players[i].LockedIn == true) return;

                players[i] = new CharacterSelectState(players[i].ClientID, players[i].CharacterID, true);
                break;
            }
        }

        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].LockedIn)
            {
                //Every player is locked in
                if (i + 1 == players.Count)
                {
                    ServerSetupGame();
                }
            }
            else
                break;
        }
    }
    private void ServerSetupGame()
    {
        foreach (var player in players)
        {
            ServerManager.Singleton.SetCharacter(player.ClientID, player.CharacterID);
        }

        ServerManager.Singleton.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        int iDifference = 0;
        for (int i = 0; i < playerImages.Length; i++)
        {
            if (players.Count > i + iDifference)
            {
                if (NetworkManager.Singleton.LocalClientId == players[i + iDifference].ClientID)
                {
                    //so it ignores you in the checks of stuff, but still goes through the 3 images
                    iDifference++;
                    i--;
                    continue;
                }
                else
                {
                    if (players[i + iDifference].LockedIn)
                    {
                        playerImages[i].sprite = readySprite;
                    }
                    else
                    {
                        playerImages[i].sprite = selectingSprite;
                    }
                }
            }
            else
                playerImages[i].sprite = noPlayerSprite;
        }

    }

    public override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
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