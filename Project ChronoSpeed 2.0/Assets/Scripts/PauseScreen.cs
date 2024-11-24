using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject buttonToSetToWhenOpeningPS;
    [SerializeField] GameObject buttonToSetToWhenOpeningOS;
    public PlayerInput mainPlayerInput;
    public void OpenPauseMenu()
    {
        pauseScreen.SetActive(true);
        mainPlayerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(buttonToSetToWhenOpeningPS);
    }

    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        mainPlayerInput.SwitchCurrentActionMap("Movement");
    }

    public void OpenOptions()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttonToSetToWhenOpeningOS);
    }

    public void CloseOptions()
    {
        pauseScreen.SetActive(true);
        optionsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonToSetToWhenOpeningPS);
    }

    public void LeaveRace()
    {
        //idk
    }
}
