using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup MainMenuCG;
    [SerializeField] private CanvasGroup JoinHostCG;
    [SerializeField] private CanvasGroup OptionsCG;
    [SerializeField] private CanvasGroup CarSelectCG;
    [SerializeField] GameObject mainMenuMainButton;
    [SerializeField] GameObject joinHostMainButton;
    [SerializeField] GameObject optionsMainButton;
    [SerializeField] GameObject carSelectMainButton;
    public enum MainMenuStates
    {
        MainMenu,
        JoinHost,
        Options,
        CarSelect,
        LoadScreen,
        Count
    }

    /// <summary>
    /// 0 for Starting Menu,
    /// 1 for Join Host Menu,
    /// 
    /// </summary>
    /// <param name="menu"></param>
    public void SwitchUI(int menu, int alpha)
    {
        bool switchTo = alpha == 1;
        MainMenuStates newState = (MainMenuStates)menu;
        switch(newState)
        {
            case MainMenuStates.MainMenu:
                MainMenuCG.alpha = alpha;
                MainMenuCG.interactable = switchTo;
                MainMenuCG.blocksRaycasts = switchTo;

                if(switchTo)
                    EventSystem.current.SetSelectedGameObject(mainMenuMainButton);
                break;
            case MainMenuStates.JoinHost:
                JoinHostCG.alpha = alpha;
                JoinHostCG.interactable = switchTo;
                JoinHostCG.blocksRaycasts = switchTo;
                if (switchTo)
                    EventSystem.current.SetSelectedGameObject(joinHostMainButton);
                break;
            case MainMenuStates.Options:

                OptionsCG.alpha = alpha;
                OptionsCG.interactable = switchTo;
                OptionsCG.blocksRaycasts = switchTo;
                if (switchTo)
                    EventSystem.current.SetSelectedGameObject(optionsMainButton);
                break;
            case MainMenuStates.CarSelect:
                CarSelectCG.alpha = alpha;
                CarSelectCG.interactable = switchTo;
                CarSelectCG.blocksRaycasts = switchTo;
                if (switchTo)
                    EventSystem.current.SetSelectedGameObject(carSelectMainButton);
                break;
        }
    }

    
}
