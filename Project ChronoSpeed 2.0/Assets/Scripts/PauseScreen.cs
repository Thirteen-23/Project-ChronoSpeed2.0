using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] CanvasGroup pauseGroup;
    [SerializeField] CanvasGroup optionsGroup;
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

    public void LeaveRace()
    {
        //idk
    }
    public void SwitchToOptions()
    {
        optionsGroup.alpha = 1.0f;
        optionsGroup.interactable = true;

        pauseGroup.alpha = 0f;
        pauseGroup.interactable = false;

        EventSystem.current.SetSelectedGameObject(buttonToSetToWhenOpeningOS);

    }

    public void SwitchToPause()
    {
        optionsGroup.alpha = 0f;
        optionsGroup.interactable = false;

        pauseGroup.alpha = 1.0f;
        pauseGroup.interactable = true;

        EventSystem.current.SetSelectedGameObject(buttonToSetToWhenOpeningPS);

    }
}
