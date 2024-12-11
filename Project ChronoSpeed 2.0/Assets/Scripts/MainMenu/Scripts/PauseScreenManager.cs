using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseScreenManager : MonoBehaviour
{
    [SerializeField] CanvasGroup pauseGroup;
    [SerializeField] GameObject pauseFirstSelect;
    [SerializeField] CanvasGroup optionsGroup;
    [SerializeField] GameObject optionsFirstSelect;

    public void SwitchToOptions()
    {
        optionsGroup.alpha = 1.0f;
        optionsGroup.interactable = true;

        pauseGroup.alpha = 0f;
        pauseGroup.interactable = false;

        EventSystem.current.SetSelectedGameObject(optionsFirstSelect);

    }

    public void SwitchToPause()
    {
        optionsGroup.alpha = 0f;
        optionsGroup.interactable = false;

        pauseGroup.alpha = 1.0f;
        pauseGroup.interactable = true;

        EventSystem.current.SetSelectedGameObject(pauseFirstSelect);

    }
}
