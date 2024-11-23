using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup MainMenuCG;
    [SerializeField] private CanvasGroup JoinHostCG;
    [SerializeField] private CanvasGroup OptionsCG;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] GameObject joinServerButton;
    [SerializeField] GameObject joinButton;
    public enum MainMenuStates
    {
        MainMenu,
        JoinHost,
        Options,

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

                OptionsCG.alpha = 0f;
                OptionsCG.interactable = false;
                OptionsCG.blocksRaycasts = false;
                EventSystem.current.SetSelectedGameObject(joinServerButton);
                break;
            case MainMenuStates.JoinHost:
                MainMenuCG.alpha = 0f;
                MainMenuCG.interactable = false;
                MainMenuCG.blocksRaycasts= false;

                JoinHostCG.alpha = 1.0f;
                JoinHostCG.interactable = true;
                JoinHostCG.blocksRaycasts = true;

                OptionsCG.alpha = 0f;
                OptionsCG.interactable = false;
                OptionsCG.blocksRaycasts = false;
                EventSystem.current.SetSelectedGameObject(joinButton);
                break;
            case MainMenuStates.Options:
                MainMenuCG.alpha = 0f;
                MainMenuCG.interactable = false;
                MainMenuCG.blocksRaycasts = false;

                JoinHostCG.alpha = 0f;
                JoinHostCG.interactable = false;
                JoinHostCG.blocksRaycasts = false;

                OptionsCG.alpha = 1f;
                OptionsCG.interactable = true;
                OptionsCG.blocksRaycasts = true; 

                break;
        }
    }

    public void StartServer()
    {
        ServerManager.Singleton.StartServer();
    }
    public void StartHost()
    {
        ServerManager.Singleton.StartHost();
    }
    public void JoinHost()
    {
        ServerManager.Singleton.StartClient(ipInput.text);
    }

    public void fullscreen()
    {
        // toogle Fullscreen
        Screen.fullScreen = !Screen.fullScreen;

       
    }
}
