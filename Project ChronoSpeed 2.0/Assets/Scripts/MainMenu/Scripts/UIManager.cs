using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup MainMenuCG;
    [SerializeField] private CanvasGroup JoinHostCG;
    [SerializeField] private TMP_InputField ipInput;
    public enum MainMenuStates
    {
        MainMenu,
        JoinHost,

    }

    /// <summary>
    /// 0 for Starting Menu,
    /// 1 for Join Host Menu,
    /// 
    /// </summary>
    /// <param name="pluh"></param>
    public void SwitchUI(int pluh)
    {
        MainMenuStates newState = (MainMenuStates)pluh;
        switch(newState)
        {
            case MainMenuStates.MainMenu:
                MainMenuCG.alpha = 1.0f;
                MainMenuCG.interactable = true;
                MainMenuCG.blocksRaycasts = true;

                JoinHostCG.alpha = 0f;
                JoinHostCG.interactable = false;
                JoinHostCG.blocksRaycasts = false;
                break;
            case MainMenuStates.JoinHost:
                MainMenuCG.alpha = 0f;
                MainMenuCG.interactable = false;
                MainMenuCG.blocksRaycasts= false;

                JoinHostCG.alpha = 1.0f;
                JoinHostCG.interactable = true;
                JoinHostCG.blocksRaycasts = true;
                break;
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void JoinHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipInput.text, (ushort)15000);
        NetworkManager.Singleton.StartClient();
    }
}
