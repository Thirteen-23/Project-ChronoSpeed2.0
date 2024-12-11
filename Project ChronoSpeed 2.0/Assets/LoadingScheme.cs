using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScheme : MonoBehaviour
{
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject abilities;

    private void Start()
    {
        if (OptionsManager.instance.IsControlsLoading)
        {
            controls.SetActive(true);
            abilities.SetActive(false);
        }
        else
        {
            controls.SetActive(false);
            abilities.SetActive(true);
        }
    }
}
