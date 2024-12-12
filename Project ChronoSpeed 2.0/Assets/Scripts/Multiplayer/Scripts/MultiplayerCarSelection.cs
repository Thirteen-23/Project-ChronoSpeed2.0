using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MultiplayerCarSelection : NetworkBehaviour
{
    [SerializeField] private CarCharacterStorage carDatabase;
    [SerializeField] private Image[] playerImages;
    [SerializeField] private GameObject L2R2Image;

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite selectingSprite;
    [SerializeField] private Sprite noPlayerSprite;
    [SerializeField] private Sprite readyButtonPressed;
    [SerializeField] private Sprite readyButtonDisabled;
    [SerializeField] private Sprite readyButtonInteractable;

    [SerializeField] private Image ReadyButton;

    [SerializeField] private Image[] VertArrows;
    [SerializeField] private Image[] HorArrows;

    [SerializeField] private GameObject DystopiaCarSelect;
    [SerializeField] private GameObject PresentCarSelect;
    [SerializeField] private GameObject UtopiaCarSelect;

    [SerializeField] private GameObject DystopiaSkinSelect;
    [SerializeField] private GameObject PresentSkinSelect;
    [SerializeField] private GameObject UtopiaSkinSelect;

    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject ControlsLoadingButton;

    private NetworkList<CharacterSelectState> players;

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
        bool doesNotExist = false;

        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].ClientID == clientid)
                doesNotExist = true;
        }
        if(!doesNotExist) players.Add(new CharacterSelectState(clientid));
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


    enum CurrentBackPhase
    {
        CarSelect,
        SkinSelect,
        LoadingImage
    }
    CurrentBackPhase currentBackPhase = CurrentBackPhase.CarSelect;
    public void BackPress(InputAction.CallbackContext context)
    {
        if (context.performed != true) return;

        if (currentBackPhase == CurrentBackPhase.CarSelect)
        {
            if(IsServer)
            {
                ServerManager.Singleton.EndSessionRpc();
            }
            else
            {
                NetworkManager.Singleton.Shutdown();
            }
            //GoToMainScreen
        }
        else if (currentBackPhase == CurrentBackPhase.SkinSelect)
        {
            currentBackPhase = CurrentBackPhase.CarSelect;

            ReadyButton.sprite = readyButtonDisabled;
            L2R2Image.SetActive(false);

            for(int i  = 0; i < VertArrows.Length; i++)
            {
                VertArrows[i].enabled = false;
            }
            for (int i = 0; i < HorArrows.Length; i++)
            {
                HorArrows[i].enabled = true;
            }

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ClientID == NetworkManager.Singleton.LocalClientId)
                {
                    if(players[i].CharacterID < 7)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(PresentCarSelect);
                    }
                    else if (players[i].CharacterID < 12)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(DystopiaCarSelect);
                    }
                    else if (players[i].CharacterID < 15)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(UtopiaCarSelect);
                    }
                }
            }
        }
        else if (currentBackPhase == CurrentBackPhase.LoadingImage)
        {
            //currentBackPhase = CurrentBackPhase.SkinSelect;
            //for (int i = 0; i < VertArrows.Length; i++)
            //{
            //    VertArrows[i].enabled = true;
            //}

            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].ClientID == NetworkManager.Singleton.LocalClientId)
            //    {
            //        if (players[i].CharacterID < 7)
            //        {
            //            EventSystem.current.SetSelectedGameObject(null);
            //            EventSystem.current.SetSelectedGameObject(PresentSkinSelect);
            //        }
            //        else if (players[i].CharacterID < 12)
            //        {
            //            EventSystem.current.SetSelectedGameObject(null);
            //            EventSystem.current.SetSelectedGameObject(DystopiaSkinSelect);
            //        }
            //        else if (players[i].CharacterID < 15)
            //        {
            //            EventSystem.current.SetSelectedGameObject(null);
            //            EventSystem.current.SetSelectedGameObject(UtopiaSkinSelect);
            //        }
            //    }
            //}
        }
    }

    public void SelectCar(int carType)
    {
        currentBackPhase = CurrentBackPhase.SkinSelect;

        ReadyButton.sprite = readyButtonInteractable;
        L2R2Image.SetActive(true);

        for (int i = 0; i < VertArrows.Length; i++)
        {
            VertArrows[i].enabled = true;
        }
        for (int i = 0; i < HorArrows.Length; i++)
        {
            HorArrows[i].enabled = false;
        }

        if (carType == 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(PresentSkinSelect);
        }
        else if (carType == 1)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(DystopiaSkinSelect);
        }
        else if (carType == 2)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(UtopiaSkinSelect);
        }
    }
    public void SelectSkin(CarCharacter carc)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == NetworkManager.Singleton.LocalClientId)
            {
                if (players[i].LockedIn) { return; }

                if (players[i].CharacterID == carc.Id) { return; }
            }
        }

        //UI Shit OMGOMGOMGOMGOMGOMG
        
        

        CarSelectServerRpc(carc.Id);
    }

    public void ReadyUp(InputAction.CallbackContext context)
    {
        if (!context.performed && currentBackPhase != CurrentBackPhase.SkinSelect)
            return;
        

        for (int i = 0; i < players.Count; i++)
        {
            if (NetworkManager.Singleton.LocalClientId == players[i].ClientID)
            {
                if (!carDatabase.IsValidCharacterId(players[i].CharacterID)) return;
                currentBackPhase = CurrentBackPhase.LoadingImage;
                ReadyButton.sprite = readyButtonPressed;

                for(int j = 0; j < VertArrows.Length;  j++)
                {
                    VertArrows[j].enabled = false;
                }

                LoadingScreen.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(ControlsLoadingButton);

                //readyUpBtn.color = Color.green;
                LockInServerRpc();
                break;
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
                if (!carDatabase.IsValidCharacterId(charid)) { return; }

                players[i] = new CharacterSelectState(players[i].ClientID, charid);
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
                    StartCoroutine(ServerSetupGame());
                }
            }
            else
                break;
        }
    }
    private IEnumerator ServerSetupGame()
    {
        foreach (var player in players)
        {
            ServerManager.Singleton.SetCharacter(player.ClientID, player.CharacterID);
        }

        //doing it out here cause cant be assed making new function
        ServerManager.Singleton.gameHasStarted = true;
        NetworkManager.Singleton.SceneManager.OnSceneEvent += ServerManager.Singleton.SceneManager_OnSceneEvent;

        for (int i = 0; i < 5; i++)
        {
            if(i > 2 && i < 4)
            {
                players.OnListChanged -= HandlePlayersStateChanged;
            }
            yield return new WaitForSeconds(1);
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